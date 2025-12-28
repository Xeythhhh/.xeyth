using Automation.Cli.Common;
using Automation.Cli.Common.Logging;

namespace Automation.Cli.Common.Tests.Logging;

/// <summary>
/// Example demonstrating logger usage in a CLI application.
/// This is a code example, not an executable test.
/// </summary>
public class LoggerUsageExample
{
    /// <summary>
    /// Example entry point showing logger setup and usage.
    /// </summary>
    public static void ExampleMain(string[] args)
    {
        // Setup
        var console = ConsoleEnvironment.CreateConsole();
        var logger = LoggerFactory.CreateFromArgs(console, args);
        var context = DiagnosticContext.Create("example-app", "1.0.0", args);

        // Log startup information
        logger.Info("Application starting");
        logger.Debug("Diagnostic context", context.ToString());

        // Simulate processing
        ProcessFiles(logger);

        // Log completion
        logger.Success("Application completed successfully");
    }

    private static void ProcessFiles(ILogger logger)
    {
        logger.Info("Processing files", "Found 5 files");

        for (int i = 1; i <= 5; i++)
        {
            logger.Debug($"Processing file {i}");

            if (i == 3)
            {
                logger.Warning($"File {i} is deprecated");
            }

            if (i == 4)
            {
                logger.Error($"Failed to process file {i}", "Permission denied");
                continue;
            }

            logger.Debug($"File {i} processed successfully");
        }

        logger.Info("File processing complete", "4 of 5 files processed");
    }
}
