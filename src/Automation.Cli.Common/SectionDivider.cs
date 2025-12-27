using Spectre.Console;

namespace Automation.Cli.Common;

/// <summary>
/// Helper for creating consistently styled section dividers and rules.
/// </summary>
public static class SectionDivider
{
    /// <summary>
    /// Creates a horizontal rule with the specified title.
    /// </summary>
    /// <param name="title">The title to display in the rule.</param>
    /// <returns>A configured Rule instance.</returns>
    public static Rule Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty", nameof(title));
        }

        return new Rule($"[bold]{Markup.Escape(title)}[/]")
            .RuleStyle(new Style(ColorScheme.Muted));
    }

    /// <summary>
    /// Creates a horizontal rule with the specified title and color.
    /// </summary>
    /// <param name="title">The title to display in the rule.</param>
    /// <param name="color">The color for the rule.</param>
    /// <returns>A configured Rule instance.</returns>
    public static Rule Create(string title, Color color)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty", nameof(title));
        }

        return new Rule($"[bold]{Markup.Escape(title)}[/]")
            .RuleStyle(new Style(color));
    }

    /// <summary>
    /// Creates a simple horizontal line without title.
    /// </summary>
    /// <returns>A configured Rule instance.</returns>
    public static Rule CreateLine()
    {
        return new Rule()
            .RuleStyle(new Style(ColorScheme.Muted));
    }

    /// <summary>
    /// Creates a simple horizontal line without title with custom color.
    /// </summary>
    /// <param name="color">The color for the rule.</param>
    /// <returns>A configured Rule instance.</returns>
    public static Rule CreateLine(Color color)
    {
        return new Rule()
            .RuleStyle(new Style(color));
    }
}
