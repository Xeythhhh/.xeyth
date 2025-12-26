using System.Text.RegularExpressions;

namespace Automation.Git;

internal static partial class CommitMessageValidator
{
    private static readonly HashSet<string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "feat",
        "fix",
        "docs",
        "style",
        "refactor",
        "perf",
        "test",
        "chore",
        "ci",
        "build",
        "revert"
    };

    private static readonly HashSet<string> AllowedScopes = new(StringComparer.OrdinalIgnoreCase)
    {
        "framework",
        "planning",
        "maintenance",
        "git",
        "contracts",
        "automation",
        "workspace",
        "docs"
    };

    internal static ValidationResult Validate(string message, IEnumerable<string> contextFiles)
    {
        var result = new ValidationResult();
        var lines = GetContentLines(message);
        var headerLine = lines.FirstOrDefault(line => !string.IsNullOrWhiteSpace(line));

        if (headerLine is null)
        {
            result.Errors.Add("Commit message is empty.");
            return result;
        }

        if (IsBypassCandidate(headerLine))
        {
            return result;
        }

        var match = HeaderRegex().Match(headerLine);
        if (!match.Success)
        {
            result.Errors.Add("Commit message must follow '<type>(<scope>): <subject>' format.");
            return result;
        }

        var type = match.Groups["type"].Value;
        var scope = match.Groups["scope"].Value;
        var subject = match.Groups["subject"].Value.Trim();

        ValidateType(type, result);
        ValidateScope(scope, result);
        ValidateSubject(subject, result);
        ValidateBody(lines, headerLine, result);
        ValidateContextFiles(contextFiles, lines, result);

        return result;
    }

    private static void ValidateType(string type, ValidationResult result)
    {
        if (!AllowedTypes.Contains(type))
        {
            result.Errors.Add($"Invalid type '{type}'. Allowed: {string.Join(", ", AllowedTypes)}");
        }
    }

    private static void ValidateScope(string scope, ValidationResult result)
    {
        if (!string.IsNullOrWhiteSpace(scope) && !AllowedScopes.Contains(scope))
        {
            result.Errors.Add($"Invalid scope '{scope}'. Allowed: {string.Join(", ", AllowedScopes)}");
        }
    }

    private static void ValidateSubject(string subject, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            result.Errors.Add("Subject is required.");
            return;
        }

        if (subject.Length > 50)
        {
            result.Errors.Add("Subject must be 50 characters or fewer.");
        }

        var first = subject.TrimStart().FirstOrDefault();
        if (first != default && !char.IsLower(first))
        {
            result.Errors.Add("Subject must start with a lowercase letter.");
        }

        if (subject.TrimEnd().EndsWith(".", StringComparison.Ordinal))
        {
            result.Errors.Add("Subject must not end with a period.");
        }
    }

    private static void ValidateBody(IReadOnlyList<string> lines, string headerLine, ValidationResult result)
    {
        var list = lines as IList<string> ?? lines.ToList();
        var headerIndex = list.IndexOf(headerLine);
        var bodyLines = list
            .Skip(headerIndex + 1)
            .SkipWhile(string.IsNullOrWhiteSpace)
            .ToList();

        if (bodyLines.Any(line => line.Length > 72))
        {
            result.Warnings.Add("Body lines should wrap at 72 characters.");
        }
    }

    private static void ValidateContextFiles(IEnumerable<string> contextFiles, IReadOnlyCollection<string> lines, ValidationResult result)
    {
        var hasContextChanges = contextFiles.Any();
        if (!hasContextChanges)
        {
            return;
        }

        var mentionsContext = lines.Any(line => line.TrimStart().StartsWith("Context:", StringComparison.OrdinalIgnoreCase));
        if (!mentionsContext)
        {
            result.Warnings.Add("Context files changed but not referenced (add a 'Context:' footer).");
        }
    }

    private static List<string> GetContentLines(string message)
    {
        return message
            .Split(new[] { '\r', '\n' }, StringSplitOptions.None)
            .Select(line => line.TrimEnd())
            .Where(line => !line.TrimStart().StartsWith("#", StringComparison.Ordinal))
            .ToList();
    }

    [GeneratedRegex("^(?<type>[a-z]+)(\\((?<scope>[^)]+)\\))?(?<breaking>!)?: (?<subject>.+)$")]
    private static partial Regex HeaderRegex();

    private static bool IsBypassCandidate(string header)
    {
        return header.StartsWith("Merge", StringComparison.OrdinalIgnoreCase)
            || header.StartsWith("Revert", StringComparison.OrdinalIgnoreCase)
            || header.StartsWith("fixup!", StringComparison.OrdinalIgnoreCase)
            || header.StartsWith("squash!", StringComparison.OrdinalIgnoreCase);
    }
}

internal sealed class ValidationResult
{
    internal List<string> Errors { get; } = new();
    internal List<string> Warnings { get; } = new();

    internal bool HasErrors => Errors.Count > 0;
    internal bool HasWarnings => Warnings.Count > 0;

    internal bool ShouldBlock(ValidationLevel level)
    {
        return level == ValidationLevel.Strict && HasErrors;
    }
}
