using Automation.Cli.Common.Logging;
using Spectre.Console.Testing;

namespace Automation.Cli.Common.Tests.Logging;

public sealed class LoggerTests
{
    [Fact]
    public void Constructor_ThrowsWhenConsoleIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Logger(null!));
    }

    [Fact]
    public void Constructor_SetsDefaultVerbosityToNormal()
    {
        var console = new TestConsole();
        var logger = new Logger(console);

        Assert.Equal(VerbosityLevel.Normal, logger.Verbosity);
    }

    [Theory]
    [InlineData(VerbosityLevel.Quiet)]
    [InlineData(VerbosityLevel.Normal)]
    [InlineData(VerbosityLevel.Verbose)]
    [InlineData(VerbosityLevel.Debug)]
    public void Constructor_SetsVerbosityFromParameter(VerbosityLevel verbosity)
    {
        var console = new TestConsole();
        var logger = new Logger(console, verbosity);

        Assert.Equal(verbosity, logger.Verbosity);
    }

    [Fact]
    public void Error_AlwaysLogsRegardlessOfVerbosity()
    {
        var console = new TestConsole();
        var logger = new Logger(console, VerbosityLevel.Quiet);

        logger.Error("Test error");

        Assert.Contains("Test error", console.Output);
    }

    [Fact]
    public void Info_LogsWhenVerbosityIsNormalOrHigher()
    {
        var console = new TestConsole();
        var logger = new Logger(console, VerbosityLevel.Normal);

        logger.Info("Test info");

        Assert.Contains("Test info", console.Output);
    }

    [Fact]
    public void Info_DoesNotLogWhenVerbosityIsQuiet()
    {
        var console = new TestConsole();
        var logger = new Logger(console, VerbosityLevel.Quiet);

        logger.Info("Test info");

        Assert.DoesNotContain("Test info", console.Output);
    }

    [Fact]
    public void Warning_LogsWhenVerbosityIsVerboseOrHigher()
    {
        var console = new TestConsole();
        var logger = new Logger(console, VerbosityLevel.Verbose);

        logger.Warning("Test warning");

        Assert.Contains("Test warning", console.Output);
    }

    [Fact]
    public void Warning_DoesNotLogWhenVerbosityIsNormal()
    {
        var console = new TestConsole();
        var logger = new Logger(console, VerbosityLevel.Normal);

        logger.Warning("Test warning");

        Assert.DoesNotContain("Test warning", console.Output);
    }

    [Fact]
    public void Debug_LogsWhenVerbosityIsDebug()
    {
        var console = new TestConsole();
        var logger = new Logger(console, VerbosityLevel.Debug);

        logger.Debug("Test debug");

        Assert.Contains("DEBUG", console.Output);
        Assert.Contains("Test debug", console.Output);
    }

    [Fact]
    public void Debug_DoesNotLogWhenVerbosityIsVerbose()
    {
        var console = new TestConsole();
        var logger = new Logger(console, VerbosityLevel.Verbose);

        logger.Debug("Test debug");

        Assert.DoesNotContain("Test debug", console.Output);
    }

    [Fact]
    public void Success_LogsWhenVerbosityIsNormalOrHigher()
    {
        var console = new TestConsole();
        var logger = new Logger(console, VerbosityLevel.Normal);

        logger.Success("Test success");

        Assert.Contains("Test success", console.Output);
    }

    [Theory]
    [InlineData(LogLevel.Error, VerbosityLevel.Quiet, true)]
    [InlineData(LogLevel.Info, VerbosityLevel.Normal, true)]
    [InlineData(LogLevel.Info, VerbosityLevel.Quiet, false)]
    [InlineData(LogLevel.Warning, VerbosityLevel.Verbose, true)]
    [InlineData(LogLevel.Warning, VerbosityLevel.Normal, false)]
    [InlineData(LogLevel.Debug, VerbosityLevel.Debug, true)]
    [InlineData(LogLevel.Debug, VerbosityLevel.Verbose, false)]
    public void IsEnabled_ReturnsCorrectValueForVerbosityLevel(
        LogLevel logLevel,
        VerbosityLevel verbosity,
        bool expectedEnabled)
    {
        var console = new TestConsole();
        var logger = new Logger(console, verbosity);

        var isEnabled = logger.IsEnabled(logLevel);

        Assert.Equal(expectedEnabled, isEnabled);
    }

    [Fact]
    public void Verbosity_CanBeChanged()
    {
        var console = new TestConsole();
        var logger = new Logger(console, VerbosityLevel.Quiet);

        logger.Verbosity = VerbosityLevel.Debug;

        Assert.Equal(VerbosityLevel.Debug, logger.Verbosity);
    }

    [Fact]
    public void LogWithDetail_IncludesDetailInOutput()
    {
        var console = new TestConsole();
        var logger = new Logger(console, VerbosityLevel.Normal);

        logger.Info("Main message", "Additional detail");

        var output = console.Output;
        Assert.Contains("Main message", output);
        Assert.Contains("Additional detail", output);
    }
}
