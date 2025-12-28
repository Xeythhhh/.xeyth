namespace Automation.Cli.Common.Logging;

/// <summary>
/// Defines verbosity levels for controlling output detail.
/// </summary>
public enum VerbosityLevel
{
    /// <summary>Minimal output - errors only.</summary>
    Quiet = 0,

    /// <summary>Normal output - errors and important information.</summary>
    Normal = 1,

    /// <summary>Verbose output - includes warnings and progress.</summary>
    Verbose = 2,

    /// <summary>Debug output - all messages including debug information.</summary>
    Debug = 3
}
