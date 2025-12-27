using Automation.Cli.Common;
using Spectre.Console;
using Spectre.Console.Testing;

namespace Automation.Cli.Common.Tests;

public sealed class SectionDividerTests
{
    [Fact]
    public void Create_WithTitle_ReturnsRule()
    {
        var rule = SectionDivider.Create("Section Title");

        Assert.NotNull(rule);
    }

    [Fact]
    public void Create_WithEmptyTitle_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => SectionDivider.Create(string.Empty));
    }

    [Fact]
    public void CreateLine_ReturnsRule()
    {
        var rule = SectionDivider.CreateLine();

        Assert.NotNull(rule);
    }

    [Fact]
    public void Create_RendersInConsole()
    {
        var console = new TestConsole();
        var rule = SectionDivider.Create("Test Section");

        console.Write(rule);
        var output = console.Output;

        Assert.Contains("Test Section", output);
    }
}
