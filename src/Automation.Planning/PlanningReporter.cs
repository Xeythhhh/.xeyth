using Automation.Cli.Common;
using Spectre.Console;

namespace Automation.Planning;

internal sealed class PlanningReporter
{
    private readonly IAnsiConsole _console;

    internal PlanningReporter(IAnsiConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    internal void Success(string message, string? detail = null) => WritePanel(CliPanels.Success(message, detail));

    internal void Warning(string message, string? detail = null) => WritePanel(CliPanels.Warning(message, detail));

    internal void Info(string message, string? detail = null) => WritePanel(CliPanels.Info(message, detail));

    internal void Error(string message, string? detail = null) => WritePanel(CliPanels.Error(message, detail));

    internal void Help(IEnumerable<(string Name, string Description)> commands)
    {
        var usagePanel = new Panel(Markup.Escape("xeyth-planning <command> [options]"))
            .Header("[bold]Usage[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(ColorScheme.Primary));

        _console.Write(usagePanel);
        _console.WriteLine();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(ColorScheme.Primary);

        table.AddColumn("Command");
        table.AddColumn("Description");

        foreach (var command in commands)
        {
            table.AddRow($"[bold]{Markup.Escape(command.Name)}[/]", Markup.Escape(command.Description));
        }

        _console.Write(table);
        _console.WriteLine();
    }

    private void WritePanel(Panel panel)
    {
        _console.Write(panel);
        _console.WriteLine();
    }
}
