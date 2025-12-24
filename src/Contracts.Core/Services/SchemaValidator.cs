using System.Text.RegularExpressions;
using Contracts.Core.Models;

namespace Contracts.Core.Services;

public class SchemaValidator : ISchemaValidator
{
    private static readonly Regex HeadingRegex = new("^(#+)\\s+(.*)$", RegexOptions.Compiled);

    public IReadOnlyList<Violation> Validate(string filePath, string content, ContractMetadata contract)
    {
        if (contract.Schema is null)
        {
            return Array.Empty<Violation>();
        }

        var lines = SplitLines(content);
        var sections = ParseSections(lines);
        var violations = new List<Violation>();

        if (contract.Schema.RequiredSections is { Count: > 0 })
        {
            foreach (var requiredSection in contract.Schema.RequiredSections)
            {
                var match = sections.FirstOrDefault(section =>
                    section.Level == requiredSection.Level &&
                    string.Equals(section.Name, requiredSection.Name, StringComparison.OrdinalIgnoreCase));

                if (match is null)
                {
                    violations.Add(new Violation(
                        Code: "missing-section",
                        Message: $"Missing required section '{requiredSection.Name}' at level {requiredSection.Level}.",
                        Severity: ViolationSeverity.Error,
                        FilePath: filePath));
                }
            }
        }

        if (contract.Schema.RequiredFields is { Count: > 0 })
        {
            foreach (var group in contract.Schema.RequiredFields)
            {
                var matchingSections = sections
                    .Where(section => string.Equals(section.Name, group.Section, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (matchingSections.Count == 0)
                {
                    continue; // Missing section already captured above.
                }

                foreach (var requiredField in group.Fields)
                {
                    if (!TryCreateRegex(requiredField.Pattern, out var regex))
                    {
                        violations.Add(new Violation(
                            Code: "invalid-pattern",
                            Message: $"Invalid regex pattern '{requiredField.Pattern}' for field '{requiredField.Name}'.",
                            Severity: ViolationSeverity.Error,
                            FilePath: filePath,
                            Section: group.Section));
                        continue;
                    }

                    var match = FindFirstMatch(lines, matchingSections, regex!);

                    if (match is null)
                    {
                        violations.Add(new Violation(
                            Code: "missing-field",
                            Message: $"Section '{group.Section}' is missing field '{requiredField.Name}' matching pattern '{requiredField.Pattern}'.",
                            Severity: ViolationSeverity.Error,
                            FilePath: filePath,
                            LineNumber: matchingSections[0].StartLine + 1,
                            Section: group.Section));
                    }
                }
            }
        }

        return violations;
    }

    private static IReadOnlyList<string> SplitLines(string content) =>
        content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

    private static IReadOnlyList<SectionBlock> ParseSections(IReadOnlyList<string> lines)
    {
        var sections = new List<SectionBlock>();

        for (var i = 0; i < lines.Count; i++)
        {
            var match = HeadingRegex.Match(lines[i]);
            if (!match.Success)
            {
                continue;
            }

            var level = match.Groups[1].Value.Length;
            var name = match.Groups[2].Value.Trim().TrimEnd('#').Trim();
            sections.Add(new SectionBlock(name, level, i, i));
        }

        if (sections.Count == 0)
        {
            return sections;
        }

        var finalized = new List<SectionBlock>(sections.Count);
        for (var index = 0; index < sections.Count; index++)
        {
            var start = sections[index].StartLine;
            var end = index + 1 < sections.Count ? sections[index + 1].StartLine - 1 : lines.Count - 1;
            finalized.Add(sections[index] with { EndLine = end });
        }

        return finalized;
    }

    private static MatchLocation? FindFirstMatch(
        IReadOnlyList<string> lines,
        IReadOnlyList<SectionBlock> sections,
        Regex regex)
    {
        foreach (var section in sections)
        {
            for (var lineIndex = section.StartLine; lineIndex <= section.EndLine && lineIndex < lines.Count; lineIndex++)
            {
                if (regex.IsMatch(lines[lineIndex]))
                {
                    return new MatchLocation(lineIndex + 1, section.Name);
                }
            }
        }

        return null;
    }

    private sealed record SectionBlock(string Name, int Level, int StartLine, int EndLine);

    private sealed record MatchLocation(int LineNumber, string Section);

    private static bool TryCreateRegex(string pattern, out Regex? regex)
    {
        try
        {
            regex = new Regex(pattern, RegexOptions.Compiled);
            return true;
        }
        catch (ArgumentException)
        {
            regex = null;
            return false;
        }
    }
}
