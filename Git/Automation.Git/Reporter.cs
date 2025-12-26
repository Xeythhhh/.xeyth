using Automation.Cli.Common;
using Spectre.Console;

namespace Automation.Git;

internal sealed class Reporter
{
    private readonly IAnsiConsole _console;

    internal Reporter(IAnsiConsole console)
    {
        _console = console;
    }

    internal void Success(string message)
    {
        WritePanel(CliPanels.Success(message));
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
        var usagePanel = new Panel(Markup.Escape("xeyth-git <command> [args]"))
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

        commands.AddRow("[bold]prepare-commit-msg[/]", "Inject commit template into message file");
        commands.AddRow("[bold]commit-msg[/]", "Validate commit message format");

        _console.Write(commands);
        _console.WriteLine();
    }

    internal void ReportValidation(ValidationResult result, ValidationLevel level)
    {
        if (!result.HasErrors && !result.HasWarnings)
        {
            Success("Commit message looks good.");
            return;
        }

        foreach (var warning in result.Warnings)
        {
            Warning(warning);
        }

        foreach (var error in result.Errors)
        {
            if (level == ValidationLevel.Warn)
            {
                Warning(error);
            }
            else
            {
                Error(error);
            }
        }
    }

    private void WritePanel(Panel panel)
    {
        _console.Write(panel);
        _console.WriteLine();
    }
}
