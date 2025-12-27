using Spectre.Console;

namespace Automation.Cli.Common;

/// <summary>
/// Helper for creating consistently styled tables.
/// </summary>
public static class TableBuilder
{
    /// <summary>
    /// Creates a standard table with rounded borders and primary color.
    /// </summary>
    /// <param name="columns">The column names for the table.</param>
    /// <returns>A configured Table instance.</returns>
    public static Table Create(params string[] columns)
    {
        if (columns is null || columns.Length == 0)
        {
            throw new ArgumentException("At least one column is required", nameof(columns));
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(ColorScheme.Primary);

        foreach (var column in columns)
        {
            table.AddColumn(Markup.Escape(column));
        }

        return table;
    }

    /// <summary>
    /// Creates a minimal table without borders.
    /// </summary>
    /// <param name="columns">The column names for the table.</param>
    /// <returns>A configured Table instance.</returns>
    public static Table CreateMinimal(params string[] columns)
    {
        if (columns is null || columns.Length == 0)
        {
            throw new ArgumentException("At least one column is required", nameof(columns));
        }

        var table = new Table()
            .Border(TableBorder.None);

        foreach (var column in columns)
        {
            table.AddColumn(Markup.Escape(column));
        }

        return table;
    }

    /// <summary>
    /// Creates a summary table with minimal heavy header border.
    /// </summary>
    /// <param name="columns">The column names for the table.</param>
    /// <returns>A configured Table instance.</returns>
    public static Table CreateSummary(params string[] columns)
    {
        if (columns is null || columns.Length == 0)
        {
            throw new ArgumentException("At least one column is required", nameof(columns));
        }

        var table = new Table()
            .Border(TableBorder.MinimalHeavyHead);

        foreach (var column in columns)
        {
            table.AddColumn(Markup.Escape(column));
        }

        return table;
    }

    /// <summary>
    /// Adds a row to a table with automatic markup escaping.
    /// </summary>
    /// <param name="table">The table to add to.</param>
    /// <param name="cells">The cell values for the row.</param>
    /// <returns>The table for method chaining.</returns>
    public static Table AddRowSafe(this Table table, params string[] cells)
    {
        if (table is null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        if (cells is null || cells.Length == 0)
        {
            return table;
        }

        var escapedCells = cells.Select(cell => Markup.Escape(cell ?? string.Empty)).ToArray();
        return table.AddRow(escapedCells);
    }
}
