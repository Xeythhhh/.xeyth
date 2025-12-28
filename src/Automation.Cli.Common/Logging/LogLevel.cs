namespace Automation.Cli.Common.Logging;

/// <summary>
/// Defines log severity levels for messages.
/// </summary>
public enum LogLevel
{
    /// <summary>Debug information for troubleshooting.</summary>
    Debug = 0,

    /// <summary>Informational messages.</summary>
    Info = 1,

    /// <summary>Warning messages for potential issues.</summary>
    Warning = 2,

    /// <summary>Error messages for failures.</summary>
    Error = 3
}
