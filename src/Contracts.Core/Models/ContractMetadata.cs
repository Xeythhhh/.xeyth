namespace Contracts.Core.Models;

/// <summary>
/// Represents a complete contract metadata definition loaded from a .metadata YAML file.
/// </summary>
public record ContractMetadata
{
    /// <summary>
    /// File targeting configuration specifying which files this contract applies to.
    /// </summary>
    public required TargetConfiguration Target { get; init; }

    /// <summary>
    /// Schema definition with required sections and fields.
    /// </summary>
    public SchemaDefinition? Schema { get; init; }

    /// <summary>
    /// Naming conventions for files matching this contract.
    /// </summary>
    public NamingConvention? Naming { get; init; }

    /// <summary>
    /// Archiving rules for completed files.
    /// </summary>
    public ArchivingRules? Archiving { get; init; }

    /// <summary>
    /// Related files that should be associated with files matching this contract.
    /// </summary>
    public List<RelatedFilePattern>? RelatedFiles { get; init; }

    /// <summary>
    /// Validation rules for this contract.
    /// </summary>
    public ValidationConfiguration? Validation { get; init; }

    /// <summary>
    /// Metadata about this contract.
    /// </summary>
    public ContractMeta? Meta { get; init; }

    /// <summary>
    /// Source file path where this contract was loaded from.
    /// </summary>
    public string? SourcePath { get; init; }
}
