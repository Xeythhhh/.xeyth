using Contracts.Core.Models;

namespace Contracts.Core.Rendering;

/// <summary>
/// Context information for rendering contracts.
/// </summary>
public sealed record RenderContext
{
    /// <summary>
    /// The contract metadata to render.
    /// </summary>
    public required ContractMetadata Contract { get; init; }

    /// <summary>
    /// Additional contracts for multi-contract rendering scenarios.
    /// </summary>
    public IReadOnlyList<ContractMetadata>? AdditionalContracts { get; init; }

    /// <summary>
    /// Output writer for the rendered content.
    /// </summary>
    public required TextWriter Output { get; init; }

    /// <summary>
    /// Configuration for the renderer.
    /// </summary>
    public RendererConfiguration? Configuration { get; init; }

    /// <summary>
    /// Optional root path for relativizing file paths.
    /// </summary>
    public string? RootPath { get; init; }
}
