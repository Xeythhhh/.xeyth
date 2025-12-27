using Automation.Cli.Common;
using Spectre.Console;
using Spectre.Console.Testing;

namespace Automation.Cli.Common.Tests;

public sealed class TreeBuilderTests
{
    [Fact]
    public void Create_WithValidLabel_ReturnsTree()
    {
        var tree = TreeBuilder.Create("Root");

        Assert.NotNull(tree);
    }

    [Fact]
    public void Create_WithEmptyLabel_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TreeBuilder.Create(string.Empty));
    }

    [Fact]
    public void AddNode_WithValidLabel_AddsNode()
    {
        var tree = TreeBuilder.Create("Root");
        var node = tree.AddNode("Child");

        Assert.NotNull(node);
    }

    [Fact]
    public void AddColoredNode_WithValidLabel_AddsColoredNode()
    {
        var tree = TreeBuilder.Create("Root");
        var node = tree.AddColoredNode("Child", ColorScheme.Success);

        Assert.NotNull(node);
    }

    [Fact]
    public void Create_RendersInConsole()
    {
        var console = new TestConsole();
        var tree = TreeBuilder.Create("Root");
        tree.AddNode("Child 1");
        tree.AddNode("Child 2");

        console.Write(tree);
        var output = console.Output;

        Assert.Contains("Root", output);
        Assert.Contains("Child 1", output);
        Assert.Contains("Child 2", output);
    }
}
