namespace Contracts.Core.Rendering;

/// <summary>
/// Abstraction for rendering contract metadata in various formats.
/// </summary>
public interface IContractRenderer
{
    /// <summary>
    /// Renders a single contract to the provided context.
    /// </summary>
    /// <param name="context">The rendering context containing contract data and output configuration.</param>
    void Render(RenderContext context);
}
