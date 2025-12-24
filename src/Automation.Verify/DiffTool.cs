using System.Diagnostics.CodeAnalysis;

namespace Automation.Verify;

internal sealed class DiffTool
{
    private DiffTool(string name, string displayName, IReadOnlyList<string> toolOrder, IReadOnlyList<string> aliases)
    {
        Name = name;
        DisplayName = displayName;
        ToolOrder = toolOrder;
        Aliases = aliases;
    }

    internal string Name { get; }

    internal string DisplayName { get; }

    internal IReadOnlyList<string> ToolOrder { get; }

    internal IReadOnlyList<string> Aliases { get; }

    internal static DiffTool VisualStudioCodeInsiders { get; } = new(
        name: "VSCodeInsiders",
        displayName: "VS Code Insiders",
        toolOrder: new[] { "VisualStudioCodeInsiders", "VisualStudioCode" },
        aliases: new[] { "vscodeinsiders", "code-insiders", "insiders" });

    internal static DiffTool VisualStudioCode { get; } = new(
        name: "VSCode",
        displayName: "VS Code",
        toolOrder: new[] { "VisualStudioCode" },
        aliases: new[] { "vscode", "code" });

    internal static DiffTool VisualStudio { get; } = new(
        name: "VisualStudio",
        displayName: "Visual Studio",
        toolOrder: new[] { "VisualStudio" },
        aliases: new[] { "vs", "visualstudio" });

    internal static DiffTool Rider { get; } = new(
        name: "Rider",
        displayName: "Rider",
        toolOrder: new[] { "Rider" },
        aliases: new[] { "jetbrains rider", "jetbrains-rider" });

    internal static IReadOnlyList<DiffTool> All { get; } = new[]
    {
        VisualStudioCodeInsiders,
        VisualStudioCode,
        VisualStudio,
        Rider
    };

    internal static bool TryParse(string? value, [NotNullWhen(true)] out DiffTool? tool)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            tool = null;
            return false;
        }

        tool = All.FirstOrDefault(candidate => Matches(candidate, value));
        return tool is not null;
    }

    internal static bool IsKnownToolOrderValue(string value)
    {
        return All.SelectMany(tool => tool.ToolOrder)
            .Any(order => string.Equals(order, value, StringComparison.OrdinalIgnoreCase));
    }

    private static bool Matches(DiffTool tool, string candidate)
    {
        return string.Equals(tool.Name, candidate, StringComparison.OrdinalIgnoreCase)
            || string.Equals(tool.DisplayName, candidate, StringComparison.OrdinalIgnoreCase)
            || tool.Aliases.Any(alias => string.Equals(alias, candidate, StringComparison.OrdinalIgnoreCase));
    }

    public override string ToString() => DisplayName;
}
