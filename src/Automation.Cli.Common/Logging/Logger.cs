using Spectre.Console;

namespace Automation.Cli.Common.Logging;

/// <summary>
/// Console-based logger with verbosity control using Spectre.Console.
/// </summary>
public sealed class Logger : ILogger
{
    private readonly IAnsiConsole _console;

    public Logger(IAnsiConsole console, VerbosityLevel verbosity = VerbosityLevel.Normal)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        Verbosity = verbosity;
    }

    public VerbosityLevel Verbosity { get; set; }

    public void Debug(string message, string? detail = null)
    {
        if (IsEnabled(LogLevel.Debug))
        {
            WritePanel(CliPanels.Info($"[DEBUG] {message}", detail));
        }
    }

    public void Info(string message, string? detail = null)
    {
        if (IsEnabled(LogLevel.Info))
        {
            WritePanel(CliPanels.Info(message, detail));
        }
    }

    public void Warning(string message, string? detail = null)
    {
        if (IsEnabled(LogLevel.Warning))
        {
            WritePanel(CliPanels.Warning(message, detail));
        }
    }

    public void Error(string message, string? detail = null)
    {
        if (IsEnabled(LogLevel.Error))
        {
            WritePanel(CliPanels.Error(message, detail));
        }
    }

    public void Success(string message, string? detail = null)
    {
        if (IsEnabled(LogLevel.Info))
        {
            WritePanel(CliPanels.Success(message, detail));
        }
    }

    public bool IsEnabled(LogLevel level)
    {
        return level switch
        {
            LogLevel.Debug => Verbosity >= VerbosityLevel.Debug,
            LogLevel.Info => Verbosity >= VerbosityLevel.Normal,
            LogLevel.Warning => Verbosity >= VerbosityLevel.Verbose,
            LogLevel.Error => true, // Always show errors
            _ => false
        };
    }

    private void WritePanel(Panel panel)
    {
        _console.Write(panel);
        _console.WriteLine();
    }
}
