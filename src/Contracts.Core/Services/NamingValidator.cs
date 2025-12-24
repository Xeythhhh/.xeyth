using System.Text.RegularExpressions;
using Contracts.Core.Models;

namespace Contracts.Core.Services;

public class NamingValidator : INamingValidator
{
    public IReadOnlyList<Violation> Validate(string filePath, ContractMetadata contract)
    {
        var violations = new List<Violation>();
        var fileName = Path.GetFileName(filePath);

        if (contract.Naming is not null)
        {
            if (!TryCreateRegex(contract.Naming.Pattern, out var namingRegex))
            {
                violations.Add(new Violation(
                    Code: "invalid-pattern",
                    Message: $"Invalid regex pattern '{contract.Naming.Pattern}' for naming rule.",
                    Severity: ViolationSeverity.Error,
                    FilePath: filePath));
            }
            else if (!namingRegex!.IsMatch(fileName))
            {
                violations.Add(new Violation(
                    Code: "invalid-name",
                    Message: $"File name '{fileName}' does not match naming pattern '{contract.Naming.Pattern}'.",
                    Severity: ViolationSeverity.Error,
                    FilePath: filePath));
            }
        }

        if (contract.Archiving is not null)
        {
            if (!TryCreateRegex(contract.Archiving.Pattern, out var archiveRegex))
            {
                violations.Add(new Violation(
                    Code: "invalid-pattern",
                    Message: $"Invalid regex pattern '{contract.Archiving.Pattern}' for archiving rule.",
                    Severity: ViolationSeverity.Error,
                    FilePath: filePath));
                return violations;
            }

            var isArchived = filePath
                .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Any(segment => string.Equals(segment, contract.Archiving.Directory, StringComparison.OrdinalIgnoreCase));

            var looksArchived = archiveRegex!.IsMatch(fileName);

            if (isArchived && !looksArchived)
            {
                violations.Add(new Violation(
                    Code: "invalid-archive-name",
                    Message: $"Archived file '{fileName}' must match pattern '{contract.Archiving.Pattern}'.",
                    Severity: ViolationSeverity.Error,
                    FilePath: filePath));
            }

            if (looksArchived && !isArchived)
            {
                violations.Add(new Violation(
                    Code: "archive-directory-mismatch",
                    Message: $"File '{fileName}' matches archive naming but is not inside '{contract.Archiving.Directory}' directory.",
                    Severity: ViolationSeverity.Warning,
                    FilePath: filePath));
            }
        }

        return violations;
    }

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
