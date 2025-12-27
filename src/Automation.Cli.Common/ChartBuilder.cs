using Spectre.Console;

namespace Automation.Cli.Common;

/// <summary>
/// Helper for creating consistently styled chart visualizations.
/// </summary>
public static class ChartBuilder
{
    /// <summary>
    /// Creates a bar chart with semantic color coding for success/warning/error metrics.
    /// </summary>
    /// <param name="label">The label for the metric.</param>
    /// <param name="successCount">Count of successful items (green).</param>
    /// <param name="warningCount">Count of warning items (yellow).</param>
    /// <param name="errorCount">Count of error items (red).</param>
    /// <returns>A configured BarChart instance.</returns>
    public static BarChart CreateMetricsChart(
        string label,
        int successCount,
        int warningCount,
        int errorCount)
    {
        var chart = new BarChart();

        if (successCount > 0)
        {
            chart.AddItem("Success", successCount, ColorScheme.Success);
        }

        if (warningCount > 0)
        {
            chart.AddItem("Warnings", warningCount, ColorScheme.Warning);
        }

        if (errorCount > 0)
        {
            chart.AddItem("Errors", errorCount, ColorScheme.Error);
        }

        if (!string.IsNullOrWhiteSpace(label))
        {
            chart.Label = Markup.Escape(label);
        }

        return chart;
    }

    /// <summary>
    /// Creates a breakdown chart showing percentage distribution with semantic colors.
    /// </summary>
    /// <param name="successCount">Count of successful items (green).</param>
    /// <param name="warningCount">Count of warning items (yellow).</param>
    /// <param name="errorCount">Count of error items (red).</param>
    /// <returns>A configured BreakdownChart instance.</returns>
    public static BreakdownChart CreateBreakdownChart(
        int successCount,
        int warningCount,
        int errorCount)
    {
        var chart = new BreakdownChart();

        if (successCount > 0)
        {
            chart.AddItem("Success", successCount, ColorScheme.Success);
        }

        if (warningCount > 0)
        {
            chart.AddItem("Warnings", warningCount, ColorScheme.Warning);
        }

        if (errorCount > 0)
        {
            chart.AddItem("Errors", errorCount, ColorScheme.Error);
        }

        return chart;
    }

    /// <summary>
    /// Creates a custom bar chart with specified items.
    /// </summary>
    /// <param name="label">Optional label for the chart.</param>
    /// <returns>A new BarChart instance with optional label.</returns>
    public static BarChart CreateBarChart(string? label = null)
    {
        var chart = new BarChart();

        if (!string.IsNullOrWhiteSpace(label))
        {
            chart.Label = Markup.Escape(label);
        }

        return chart;
    }

    /// <summary>
    /// Adds an item to a bar chart with markup escaping.
    /// </summary>
    /// <param name="chart">The chart to add to.</param>
    /// <param name="label">The label for the item.</param>
    /// <param name="value">The value for the item.</param>
    /// <param name="color">The color for the item.</param>
    /// <returns>The chart for method chaining.</returns>
    public static BarChart AddItem(this BarChart chart, string label, double value, Color color)
    {
        if (chart is null)
        {
            throw new ArgumentNullException(nameof(chart));
        }

        return chart.AddItem(Markup.Escape(label), value, color);
    }
}
