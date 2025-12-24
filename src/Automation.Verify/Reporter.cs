using Automation.Cli.Common;
using Spectre.Console;

namespace Automation.Verify;

internal sealed class Reporter
{
    private readonly IAnsiConsole _console;

    internal Reporter(IAnsiConsole console)
    {
        _console = console;
    }

    internal void Success(string message, string? detail = null)
    {
        WritePanel(CliPanels.Success(message, detail));
    }

    internal void Warning(string message)
    {
        WritePanel(CliPanels.Warning(message));
    }

    internal void Info(string message)
    {
        WritePanel(CliPanels.Info(message));
    }

    internal void Error(string message)
    {
        WritePanel(CliPanels.Error(message));
    }

    internal void Help()
    {
        var usagePanel = new Panel("xeyth-verify <command> [options]")
            .Header("[bold]Usage[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(ColorScheme.Primary));

        _console.Write(usagePanel);
        _console.WriteLine();

        var commands = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(ColorScheme.Primary);

        commands.AddColumn("Command");
        commands.AddColumn("Description");

        commands.AddRow("[bold]setup[/]", "Create or update DiffEngine.json");
        commands.AddRow("[bold]validate[/]", "Check existing DiffEngine.json");

        _console.Write(commands);
        _console.WriteLine();

        var options = new Table().NoBorder().HideHeaders();
        options.AddColumn(new TableColumn(string.Empty));
        options.AddColumn(new TableColumn(string.Empty));

        options.AddRow(
            "[bold]setup[/]",
            "--tool, -t    VSCodeInsiders | VSCode | VisualStudio | Rider\n--path, -p    Target directory (defaults to current)\n--target      Alias for --path");
        options.AddEmptyRow();
        options.AddRow(
            "[bold]validate[/]",
            "--path, -p    Target directory (defaults to current)\n--target      Alias for --path");

        _console.Write(options);
        _console.WriteLine();
        _console.MarkupLine("Defaults to [green]VS Code Insiders[/] when no tool is specified.");
    }

    private void WritePanel(Panel panel)
    {
        _console.Write(panel);
        _console.WriteLine();
    }
}