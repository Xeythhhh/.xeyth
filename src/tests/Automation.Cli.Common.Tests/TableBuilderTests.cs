using Automation.Cli.Common;
using Spectre.Console;
using Spectre.Console.Testing;

namespace Automation.Cli.Common.Tests;

public sealed class TableBuilderTests
{
    [Fact]
    public void Create_WithColumns_ReturnsTable()
    {
        var table = TableBuilder.Create("Column1", "Column2");

        Assert.NotNull(table);
    }

    [Fact]
    public void Create_WithNoColumns_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TableBuilder.Create());
    }

    [Fact]
    public void CreateMinimal_WithColumns_ReturnsTable()
    {
        var table = TableBuilder.CreateMinimal("Column1", "Column2");

        Assert.NotNull(table);
    }

    [Fact]
    public void CreateSummary_WithColumns_ReturnsTable()
    {
        var table = TableBuilder.CreateSummary("Column1", "Column2");

        Assert.NotNull(table);
    }

    [Fact]
    public void AddRowSafe_WithValues_AddsRow()
    {
        var console = new TestConsole();
        var table = TableBuilder.Create("Name", "Value");
        table.AddRowSafe("Test", "123");

        console.Write(table);
        var output = console.Output;

        Assert.Contains("Name", output);
        Assert.Contains("Value", output);
        Assert.Contains("Test", output);
        Assert.Contains("123", output);
    }
}
