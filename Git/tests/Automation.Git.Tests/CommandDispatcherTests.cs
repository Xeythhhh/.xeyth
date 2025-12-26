using System.Text;
using Automation.Git.Tests.Infrastructure;
using Spectre.Console;

namespace Automation.Git.Tests;

public sealed class CommandDispatcherTests
{
    [Fact]
    public void Execute_ShowsHelp_WhenNoArgs()
    {
        var writer = new StringWriter();
        var console = TestConsole(writer);
        var reporter = new Reporter(console);
        var dispatcher = new CommandDispatcher(reporter, new FakeGitClient());

        var exitCode = dispatcher.Execute(Array.Empty<string>());

        Assert.Equal(1, exitCode);
        Assert.Contains("Usage", writer.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Execute_ReturnsError_ForUnknownCommand()
    {
        var writer = new StringWriter();
        var console = TestConsole(writer);
        var reporter = new Reporter(console);
        var dispatcher = new CommandDispatcher(reporter, new FakeGitClient());

        var exitCode = dispatcher.Execute(new[] { "unknown" });

        Assert.Equal(1, exitCode);
        Assert.Contains("Unknown", writer.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CommitMsg_DisabledValidation_Succeeds()
    {
        using var sandbox = new TempDirectory();
        var messagePath = Path.Combine(sandbox.Path, "COMMIT_EDITMSG");
        File.WriteAllText(messagePath, "invalid");

        var git = new FakeGitClient
        {
            ConfigValue = "disable",
            ContextFiles = Array.Empty<string>()
        };

        var dispatcher = BuildDispatcher(git);

        var exitCode = dispatcher.Execute(new[] { "commit-msg", messagePath });

        Assert.Equal(0, exitCode);
    }

    [Fact]
    public void CommitMsg_StrictInvalid_Fails()
    {
        using var sandbox = new TempDirectory();
        var messagePath = Path.Combine(sandbox.Path, "COMMIT_EDITMSG");
        File.WriteAllText(messagePath, "invalid");

        var git = new FakeGitClient
        {
            ConfigValue = "strict",
            ContextFiles = Array.Empty<string>()
        };

        var dispatcher = BuildDispatcher(git);

        var exitCode = dispatcher.Execute(new[] { "commit-msg", messagePath });

        Assert.Equal(1, exitCode);
    }

    [Fact]
    public void PrepareCommitMsg_InsertsTemplate_WhenEmpty()
    {
        using var sandbox = new TempDirectory();
        var messagePath = Path.Combine(sandbox.Path, "COMMIT_EDITMSG");
        var templatePath = Path.Combine(sandbox.Path, ".gitmessage");
        File.WriteAllText(messagePath, string.Empty);
        File.WriteAllText(templatePath, "template");

        var git = new FakeGitClient
        {
            ConfigValue = templatePath
        };

        var dispatcher = BuildDispatcher(git);

        var exitCode = dispatcher.Execute(new[] { "prepare-commit-msg", messagePath });

        Assert.Equal(0, exitCode);
        Assert.Equal("template" + Environment.NewLine, File.ReadAllText(messagePath));
    }

    private static CommandDispatcher BuildDispatcher(IGitClient git)
    {
        var writer = new StringWriter();
        var console = TestConsole(writer);
        var reporter = new Reporter(console);
        return new CommandDispatcher(reporter, git);
    }

    private static IAnsiConsole TestConsole(StringWriter writer)
    {
        var settings = new AnsiConsoleSettings
        {
            Ansi = AnsiSupport.No,
            ColorSystem = ColorSystemSupport.NoColors,
            Out = new AnsiConsoleOutput(writer),
            Interactive = InteractionSupport.No
        };

        return AnsiConsole.Create(settings);
    }
}

internal sealed class FakeGitClient : IGitClient
{
    internal string? ConfigValue { get; init; }
    internal string? RepositoryRoot { get; init; }
    internal IReadOnlyList<string> ContextFiles { get; init; } = Array.Empty<string>();

    public string? GetConfig(string key) => ConfigValue;

    public string? GetRepositoryRoot() => RepositoryRoot;

    public IReadOnlyList<string> GetContextFiles() => ContextFiles;
}
