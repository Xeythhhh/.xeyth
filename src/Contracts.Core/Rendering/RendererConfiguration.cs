namespace Contracts.Core.Rendering;

/// <summary>
/// Configuration options for contract rendering.
/// </summary>
public sealed record RendererConfiguration
{
    /// <summary>
    /// The rendering style to use.
    /// </summary>
    public RenderStyle Style { get; init; } = RenderStyle.Card;

    /// <summary>
    /// Color theme for terminal rendering.
    /// </summary>
    public ColorTheme Theme { get; init; } = ColorTheme.Auto;

    /// <summary>
    /// Maximum width for rendering (defaults to terminal width).
    /// </summary>
    public int? MaxWidth { get; init; }

    /// <summary>
    /// Whether to include verbose details.
    /// </summary>
    public bool Verbose { get; init; }

    /// <summary>
    /// Whether to show color in output.
    /// </summary>
    public bool NoColor { get; init; }
}

/// <summary>
/// Rendering style options.
/// </summary>
public enum RenderStyle
{
    /// <summary>
    /// Card style with boxed formatting.
    /// </summary>
    Card,

    /// <summary>
    /// Tabular format for comparing multiple contracts.
    /// </summary>
    Table,

    /// <summary>
    /// Hierarchical tree view.
    /// </summary>
    Tree,

    /// <summary>
    /// Compact single-line format.
    /// </summary>
    Compact
}

/// <summary>
/// Color theme options.
/// </summary>
public enum ColorTheme
{
    /// <summary>
    /// Auto-detect based on terminal background.
    /// </summary>
    Auto,

    /// <summary>
    /// Light terminal theme.
    /// </summary>
    Light,

    /// <summary>
    /// Dark terminal theme.
    /// </summary>
    Dark
}
