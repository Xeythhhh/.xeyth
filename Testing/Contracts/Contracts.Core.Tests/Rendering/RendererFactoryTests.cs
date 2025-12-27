using Contracts.Core.Rendering;

namespace Contracts.Core.Tests.Rendering;

public class RendererFactoryTests
{
    [Theory]
    [InlineData(RenderStyle.Card, typeof(CardRenderer))]
    [InlineData(RenderStyle.Table, typeof(TableRenderer))]
    [InlineData(RenderStyle.Tree, typeof(TreeRenderer))]
    [InlineData(RenderStyle.Compact, typeof(CompactRenderer))]
    public void Create_WithRenderStyle_ReturnsCorrectRenderer(RenderStyle style, Type expectedType)
    {
        // Act
        var renderer = RendererFactory.Create(style);

        // Assert
        Assert.IsType(expectedType, renderer);
    }

    [Fact]
    public void Create_WithNullConfiguration_ReturnsCardRenderer()
    {
        // Act
        var renderer = RendererFactory.Create((RendererConfiguration?)null);

        // Assert
        Assert.IsType<CardRenderer>(renderer);
    }

    [Fact]
    public void Create_WithConfiguration_ReturnsCorrectRenderer()
    {
        // Arrange
        var config = new RendererConfiguration { Style = RenderStyle.Table };

        // Act
        var renderer = RendererFactory.Create(config);

        // Assert
        Assert.IsType<TableRenderer>(renderer);
    }
}
