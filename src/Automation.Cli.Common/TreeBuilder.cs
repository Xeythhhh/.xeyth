using Spectre.Console;

namespace Automation.Cli.Common;

/// <summary>
/// Helper for building hierarchical tree structures with consistent styling.
/// </summary>
public static class TreeBuilder
{
    /// <summary>
    /// Creates a tree with the specified root label and primary color styling.
    /// </summary>
    /// <param name="rootLabel">The label for the root node.</param>
    /// <returns>A configured tree instance.</returns>
    public static Tree Create(string rootLabel)
    {
        if (string.IsNullOrWhiteSpace(rootLabel))
        {
            throw new ArgumentException("Root label cannot be empty", nameof(rootLabel));
        }

        return new Tree(Markup.Escape(rootLabel))
            .Style(new Style(ColorScheme.Primary));
    }

    /// <summary>
    /// Creates a tree with the specified root label and custom color.
    /// </summary>
    /// <param name="rootLabel">The label for the root node.</param>
    /// <param name="color">The color to use for the tree lines.</param>
    /// <returns>A configured tree instance.</returns>
    public static Tree Create(string rootLabel, Color color)
    {
        if (string.IsNullOrWhiteSpace(rootLabel))
        {
            throw new ArgumentException("Root label cannot be empty", nameof(rootLabel));
        }

        return new Tree(Markup.Escape(rootLabel))
            .Style(new Style(color));
    }

    /// <summary>
    /// Adds a node to the parent with markup escaping.
    /// </summary>
    /// <param name="parent">The parent tree node.</param>
    /// <param name="label">The label for the new node.</param>
    /// <returns>The newly created tree node.</returns>
    public static IHasTreeNodes AddNode(this IHasTreeNodes parent, string label)
    {
        if (parent is null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        if (string.IsNullOrWhiteSpace(label))
        {
            throw new ArgumentException("Node label cannot be empty", nameof(label));
        }

        return parent.AddNode(Markup.Escape(label));
    }

    /// <summary>
    /// Adds a colored node to the parent with markup escaping.
    /// </summary>
    /// <param name="parent">The parent tree node.</param>
    /// <param name="label">The label for the new node.</param>
    /// <param name="color">The color to use for the node label.</param>
    /// <returns>The newly created tree node.</returns>
    public static IHasTreeNodes AddColoredNode(this IHasTreeNodes parent, string label, Color color)
    {
        if (parent is null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        if (string.IsNullOrWhiteSpace(label))
        {
            throw new ArgumentException("Node label cannot be empty", nameof(label));
        }

        var markup = new Markup(Markup.Escape(label), new Style(color));
        return parent.AddNode(markup);
    }
}
