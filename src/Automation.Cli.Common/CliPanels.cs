using Spectre.Console;
using Spectre.Console.Rendering;

namespace Automation.Cli.Common;

public static class CliPanels
{
    public static Panel Success(string message, string? detail = null) =>
        Build("[OK]", "Success", message, detail, ColorScheme.Success);

    public static Panel Error(string message, string? detail = null) =>
        Build("[ERR]", "Error", message, detail, ColorScheme.Error);

    public static Panel Warning(string message, string? detail = null) =>
        Build("[WARN]", "Warning", message, detail, ColorScheme.Warning);

    public static Panel Info(string message, string? detail = null) =>
        Build("[INFO]", "Info", message, detail, ColorScheme.Info);

    private static Panel Build(string icon, string title, string message, string? detail, Color borderColor)
    {
        var rows = new List<IRenderable>
        {
            new Markup($"{Markup.Escape(icon)} {Markup.Escape(message)}")
        };

        if (!string.IsNullOrWhiteSpace(detail))
        {
            rows.Add(new Markup($"[dim]{Markup.Escape(detail)}[/]"));
        }

        return new Panel(new Rows(rows))
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(borderColor))
            .Header($"[bold]{Markup.Escape(title)}[/]");
    }
}
