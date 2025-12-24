namespace Contracts.Core.Models;

/// <summary>
/// Aggregated validation result for a single file.
/// </summary>
public record ValidationResult(string FilePath, IReadOnlyList<Violation> Violations)
{
    /// <summary>
    /// True when there are no error-level violations.
    /// </summary>
    public bool IsSuccess => !HasErrors;

    /// <summary>
    /// Indicates whether any error-level violations were found.
    /// </summary>
    public bool HasErrors => Violations.Any(v => v.Severity == ViolationSeverity.Error);

    /// <summary>
    /// Indicates whether any warning-level violations were found.
    /// </summary>
    public bool HasWarnings => Violations.Any(v => v.Severity == ViolationSeverity.Warning);

    public static ValidationResult Success(string filePath) => new(filePath, Array.Empty<Violation>());

    public static ValidationResult FromViolations(string filePath, IReadOnlyList<Violation> violations) => new(filePath, violations);
}

/// <summary>
/// Represents a single validation violation.
/// </summary>
public record Violation(
    string Code,
    string Message,
    ViolationSeverity Severity,
    string? FilePath = null,
    int? LineNumber = null,
    string? Section = null);

/// <summary>
/// Severity for validation violations.
/// </summary>
public enum ViolationSeverity
{
    Info,
    Warning,
    Error
}
