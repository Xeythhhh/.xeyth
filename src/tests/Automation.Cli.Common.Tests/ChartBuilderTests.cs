using Automation.Cli.Common;
using Spectre.Console;
using Spectre.Console.Testing;

namespace Automation.Cli.Common.Tests;

public sealed class ChartBuilderTests
{
    [Fact]
    public void CreateMetricsChart_WithCounts_ReturnsChart()
    {
        var chart = ChartBuilder.CreateMetricsChart("Test Metrics", 10, 5, 2);

        Assert.NotNull(chart);
    }

    [Fact]
    public void CreateBreakdownChart_WithCounts_ReturnsChart()
    {
        var chart = ChartBuilder.CreateBreakdownChart(10, 5, 2);

        Assert.NotNull(chart);
    }

    [Fact]
    public void CreateBarChart_WithLabel_ReturnsChart()
    {
        var chart = ChartBuilder.CreateBarChart("Custom Chart");

        Assert.NotNull(chart);
    }

    [Fact]
    public void CreateMetricsChart_RendersInConsole()
    {
        var console = new TestConsole();
        var chart = ChartBuilder.CreateMetricsChart("Results", 10, 5, 2);

        console.Write(chart);
        var output = console.Output;

        Assert.Contains("Success", output);
        Assert.Contains("Warnings", output);
        Assert.Contains("Errors", output);
    }
}
