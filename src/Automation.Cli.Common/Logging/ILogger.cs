namespace Automation.Cli.Common.Logging;

/// <summary>
/// Provides logging capabilities with verbosity control.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Gets or sets the current verbosity level.
    /// </summary>
    VerbosityLevel Verbosity { get; set; }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    void Debug(string message, string? detail = null);

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    void Info(string message, string? detail = null);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    void Warning(string message, string? detail = null);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    void Error(string message, string? detail = null);

    /// <summary>
    /// Logs a success message.
    /// </summary>
    void Success(string message, string? detail = null);

    /// <summary>
    /// Determines if a log level should be output based on current verbosity.
    /// </summary>
    bool IsEnabled(LogLevel level);
}
