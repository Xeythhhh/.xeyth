namespace Contracts.Core.Models;

/// <summary>
/// File targeting configuration using glob patterns.
/// </summary>
public record TargetConfiguration
{
    /// <summary>
    /// Glob patterns to match files this contract applies to.
    /// </summary>
    public required List<string> Patterns { get; init; }

    /// <summary>
    /// Glob patterns to exclude from matching.
    /// </summary>
    public List<string>? Exclude { get; init; }
}

/// <summary>
/// Schema definition with required sections and fields.
/// </summary>
public record SchemaDefinition
{
    /// <summary>
    /// Required markdown sections (headings).
    /// </summary>
    public List<RequiredSection>? RequiredSections { get; init; }

    /// <summary>
    /// Required fields within sections.
    /// </summary>
    public List<RequiredFieldGroup>? RequiredFields { get; init; }
}

/// <summary>
/// Required markdown section definition.
/// </summary>
public record RequiredSection
{
    public required string Name { get; init; }
    public required int Level { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// Group of required fields within a specific section.
/// </summary>
public record RequiredFieldGroup
{
    public required string Section { get; init; }
    public required List<RequiredField> Fields { get; init; }
}

/// <summary>
/// Required field definition with validation pattern.
/// </summary>
public record RequiredField
{
    public required string Name { get; init; }
    public required string Pattern { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// Naming conventions for files.
/// </summary>
public record NamingConvention
{
    public required string Pattern { get; init; }
    public string? Description { get; init; }
    public List<string>? Examples { get; init; }
}

/// <summary>
/// Archiving rules for completed files.
/// </summary>
public record ArchivingRules
{
    public required string Directory { get; init; }
    public required string Pattern { get; init; }
    public string? Description { get; init; }
    public List<string>? Examples { get; init; }
}

/// <summary>
/// Related file pattern definition.
/// </summary>
public record RelatedFilePattern
{
    public required string Pattern { get; init; }
    public string? Description { get; init; }
    public bool Required { get; init; }
    public string? Validation { get; init; }
}

/// <summary>
/// Validation configuration with custom rules.
/// </summary>
public record ValidationConfiguration
{
    public List<ValidationRule>? Rules { get; init; }
}

/// <summary>
/// Custom validation rule definition.
/// </summary>
public record ValidationRule
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// Contract metadata information.
/// </summary>
public record ContractMeta
{
    public string? Version { get; init; }
    public string? Author { get; init; }
    public string? Description { get; init; }
    public string? LastUpdated { get; init; }
}
