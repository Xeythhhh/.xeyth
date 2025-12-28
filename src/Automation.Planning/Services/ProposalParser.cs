using System.Text.RegularExpressions;
using Automation.Planning.Models;
using Automation.Cli.Common;

namespace Automation.Planning.Services;

public sealed class ProposalParser
{
    private static readonly Regex MetadataLine = new(@"^\*\*(?<key>[A-Za-z0-9 ]+)\*\*:\s*(?<value>.+)$", RegexOptions.Compiled);

    public Proposal Parse(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException(ErrorMessages.RequiredValue("Path"));
        }

        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException(ErrorMessages.FileNotFound(fullPath));
        }

        var content = File.ReadAllText(fullPath);
        var lines = SplitLines(content);

        var metadata = ExtractMetadata(lines);
        var title = ExtractTitle(lines) ?? Path.GetFileNameWithoutExtension(fullPath);
        var status = ParseStatus(metadata.GetValueOrDefault("Status"));

        return new Proposal(
            Name: Path.GetFileNameWithoutExtension(fullPath),
            Title: title,
            Status: status,
            Path: fullPath,
            Submitted: metadata.GetValueOrDefault("Submitted"),
            Author: metadata.GetValueOrDefault("Author"),
            RelatedTask: metadata.GetValueOrDefault("Related Task"),
            Content: content);
    }

    private static Dictionary<string, string> ExtractMetadata(IEnumerable<string> lines)
    {
        var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in lines.Select(l => l.Trim()))
        {
            var match = MetadataLine.Match(line);
            if (!match.Success)
            {
                continue;
            }

            var key = match.Groups["key"].Value.Trim();
            var value = match.Groups["value"].Value.Trim();

            if (!metadata.ContainsKey(key))
            {
                metadata[key] = value;
            }
        }

        return metadata;
    }

    private static string? ExtractTitle(IEnumerable<string> lines)
    {
        foreach (var line in lines.Select(l => l.Trim().TrimStart('\uFEFF')))
        {
            if (!line.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            var title = line.TrimStart('#', ' ');
            if (title.StartsWith("Proposal:", StringComparison.OrdinalIgnoreCase))
            {
                title = title["Proposal:".Length..].Trim();
            }

            return string.IsNullOrWhiteSpace(title) ? null : title;
        }

        return null;
    }

    private static ProposalStatus ParseStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ProposalStatus.Pending;
        }

        if (Enum.TryParse<ProposalStatus>(value, ignoreCase: true, out var status))
        {
            return status;
        }

        return ProposalStatus.Unknown;
    }

    private static string[] SplitLines(string content)
    {
        return content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
    }
}
