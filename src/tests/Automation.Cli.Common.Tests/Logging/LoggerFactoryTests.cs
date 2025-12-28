using Automation.Cli.Common.Logging;
using Spectre.Console.Testing;

namespace Automation.Cli.Common.Tests.Logging;

public sealed class LoggerFactoryTests
{
    [Fact]
    public void Create_ReturnsLoggerWithDefaultVerbosity()
    {
        var console = new TestConsole();

        var logger = LoggerFactory.Create(console);

        Assert.NotNull(logger);
        Assert.Equal(VerbosityLevel.Normal, logger.Verbosity);
    }

    [Theory]
    [InlineData(VerbosityLevel.Quiet)]
    [InlineData(VerbosityLevel.Normal)]
    [InlineData(VerbosityLevel.Verbose)]
    [InlineData(VerbosityLevel.Debug)]
    public void Create_ReturnsLoggerWithSpecifiedVerbosity(VerbosityLevel verbosity)
    {
        var console = new TestConsole();

        var logger = LoggerFactory.Create(console, verbosity);

        Assert.Equal(verbosity, logger.Verbosity);
    }

    [Fact]
    public void CreateFromArgs_ReturnsNormalVerbosityWhenNoFlags()
    {
        var console = new TestConsole();
        var args = new[] { "command", "--option", "value" };

        var logger = LoggerFactory.CreateFromArgs(console, args);

        Assert.Equal(VerbosityLevel.Normal, logger.Verbosity);
    }

    [Fact]
    public void CreateFromArgs_ReturnsQuietVerbosityForQuietFlag()
    {
        var console = new TestConsole();
        var args = new[] { "--quiet", "command" };

        var logger = LoggerFactory.CreateFromArgs(console, args);

        Assert.Equal(VerbosityLevel.Quiet, logger.Verbosity);
    }

    [Fact]
    public void CreateFromArgs_ReturnsQuietVerbosityForShortQuietFlag()
    {
        var console = new TestConsole();
        var args = new[] { "-q", "command" };

        var logger = LoggerFactory.CreateFromArgs(console, args);

        Assert.Equal(VerbosityLevel.Quiet, logger.Verbosity);
    }

    [Fact]
    public void CreateFromArgs_ReturnsVerboseVerbosityForVerboseFlag()
    {
        var console = new TestConsole();
        var args = new[] { "--verbose", "command" };

        var logger = LoggerFactory.CreateFromArgs(console, args);

        Assert.Equal(VerbosityLevel.Verbose, logger.Verbosity);
    }

    [Fact]
    public void CreateFromArgs_ReturnsVerboseVerbosityForShortVerboseFlag()
    {
        var console = new TestConsole();
        var args = new[] { "-v", "command" };

        var logger = LoggerFactory.CreateFromArgs(console, args);

        Assert.Equal(VerbosityLevel.Verbose, logger.Verbosity);
    }

    [Fact]
    public void CreateFromArgs_ReturnsDebugVerbosityForDebugFlag()
    {
        var console = new TestConsole();
        var args = new[] { "--debug", "command" };

        var logger = LoggerFactory.CreateFromArgs(console, args);

        Assert.Equal(VerbosityLevel.Debug, logger.Verbosity);
    }

    [Fact]
    public void ParseVerbosityFromArgs_ReturnsNormalWhenNoFlags()
    {
        var args = new[] { "command" };

        var verbosity = LoggerFactory.ParseVerbosityFromArgs(args);

        Assert.Equal(VerbosityLevel.Normal, verbosity);
    }

    [Theory]
    [InlineData(new[] { "--quiet" }, VerbosityLevel.Quiet)]
    [InlineData(new[] { "-q" }, VerbosityLevel.Quiet)]
    [InlineData(new[] { "--verbose" }, VerbosityLevel.Verbose)]
    [InlineData(new[] { "-v" }, VerbosityLevel.Verbose)]
    [InlineData(new[] { "--debug" }, VerbosityLevel.Debug)]
    public void ParseVerbosityFromArgs_ReturnsCorrectVerbosity(string[] args, VerbosityLevel expected)
    {
        var verbosity = LoggerFactory.ParseVerbosityFromArgs(args);

        Assert.Equal(expected, verbosity);
    }

    [Fact]
    public void ParseVerbosityFromArgs_QuietTakesPrecedenceOverOthers()
    {
        var args = new[] { "--quiet", "--verbose", "--debug" };

        var verbosity = LoggerFactory.ParseVerbosityFromArgs(args);

        Assert.Equal(VerbosityLevel.Quiet, verbosity);
    }

    [Fact]
    public void ParseVerbosityFromArgs_DebugTakesPrecedenceOverVerbose()
    {
        var args = new[] { "--verbose", "--debug" };

        var verbosity = LoggerFactory.ParseVerbosityFromArgs(args);

        Assert.Equal(VerbosityLevel.Debug, verbosity);
    }
}
