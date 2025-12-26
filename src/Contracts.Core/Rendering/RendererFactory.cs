namespace Contracts.Core.Rendering;

/// <summary>
/// Factory for creating contract renderers based on configuration.
/// </summary>
public static class RendererFactory
{
    /// <summary>
    /// Creates a renderer based on the specified style.
    /// </summary>
    /// <param name="style">The rendering style to use.</param>
    /// <returns>An instance of the appropriate renderer.</returns>
    public static IContractRenderer Create(RenderStyle style)
    {
        return style switch
        {
            RenderStyle.Card => new CardRenderer(),
            RenderStyle.Table => new TableRenderer(),
            RenderStyle.Tree => new TreeRenderer(),
            RenderStyle.Compact => new CompactRenderer(),
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, "Unknown render style")
        };
    }

    /// <summary>
    /// Creates a renderer from configuration.
    /// </summary>
    /// <param name="configuration">The renderer configuration.</param>
    /// <returns>An instance of the appropriate renderer.</returns>
    public static IContractRenderer Create(RendererConfiguration? configuration)
    {
        var style = configuration?.Style ?? RenderStyle.Card;
        return Create(style);
    }
}
