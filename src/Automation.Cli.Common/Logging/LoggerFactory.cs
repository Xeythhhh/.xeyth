using Spectre.Console;

namespace Automation.Cli.Common.Logging;

/// <summary>
/// Factory for creating configured loggers.
/// </summary>
public static class LoggerFactory
{
    /// <summary>
    /// Creates a logger with the specified console and verbosity level.
    /// </summary>
    public static ILogger Create(IAnsiConsole console, VerbosityLevel verbosity = VerbosityLevel.Normal)
    {
        return new Logger(console, verbosity);
    }

    /// <summary>
    /// Creates a logger with verbosity level parsed from command line arguments.
    /// Supports: --quiet (-q), --verbose (-v), and --debug flags.
    /// </summary>
    public static ILogger CreateFromArgs(IAnsiConsole console, string[] args)
    {
        var verbosity = ParseVerbosityFromArgs(args);
        return new Logger(console, verbosity);
    }

    /// <summary>
    /// Parses verbosity level from command line arguments.
    /// Precedence when multiple flags are provided: Quiet > Debug > Verbose > Normal (default).
    /// </summary>
    public static VerbosityLevel ParseVerbosityFromArgs(string[] args)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        if (args.Contains("--quiet") || args.Contains("-q"))
        {
            return VerbosityLevel.Quiet;
        }

        if (args.Contains("--debug"))
        {
            return VerbosityLevel.Debug;
        }

        if (args.Contains("--verbose") || args.Contains("-v"))
        {
            return VerbosityLevel.Verbose;
        }

        return VerbosityLevel.Normal;
    }
}
