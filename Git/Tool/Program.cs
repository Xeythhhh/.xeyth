using System.Reflection;
using Automation.Cli.Common;
using Spectre.Console;

namespace GitTool;

internal static class Program
{
    private static int Main(string[] args)
    {
        var console = ConsoleEnvironment.CreateConsole();
        var reporter = new Reporter(console);
        var header = new CliHeader("xeyth-git", GetVersion(), "Git hook helpers");
        if (ConsoleEnvironment.IsInteractive(console))
        {
            header.Render(console);
        }

        var dispatcher = new CommandDispatcher(reporter, new GitClient());

        try
        {
            return dispatcher.Execute(args);
        }
        catch (Exception ex)
        {
            reporter.Error(ex.Message);
            return 1;
        }
    }

    private static string GetVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var informational = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        if (!string.IsNullOrWhiteSpace(informational))
        {
            var plusIndex = informational.IndexOf('+');
            return plusIndex > 0 ? informational[..plusIndex] : informational;
        }

        return assembly.GetName().Version?.ToString(3) ?? "1.0.0";
    }
}

internal sealed class CommandDispatcher
{
    private readonly Reporter _reporter;
    private readonly IGitClient _git;

    internal CommandDispatcher(Reporter reporter, IGitClient git)
    {
        _reporter = reporter;
        _git = git;
    }

    internal int Execute(string[] args)
    {
        if (args.Length == 0 || IsHelp(args[0]))
        {
            _reporter.Help();
            return args.Length == 0 ? 1 : 0;
        }

        var command = args[0].ToLowerInvariant();
        var remaining = args.Skip(1).ToArray();

        return command switch
        {
            "prepare-commit-msg" => RunPrepareCommitMsg(remaining),
            "commit-msg" => RunCommitMsg(remaining),
            _ => Unknown(command)
        };
    }

    private int RunPrepareCommitMsg(string[] args)
    {
        if (args.Length == 0)
        {
            _reporter.Error("Missing commit message file path.");
            return 1;
        }

        var commitMessagePath = args[0];
        if (!File.Exists(commitMessagePath))
        {
            _reporter.Error($"Commit message file not found: {commitMessagePath}");
            return 1;
        }

        var templatePath = CommitTemplateInjector.ResolveTemplatePath(
            _git.GetConfig("commit.template"),
            _git.GetRepositoryRoot());

        if (string.IsNullOrWhiteSpace(templatePath))
        {
            return 0;
        }

        var injected = CommitTemplateInjector.InjectIfEmpty(commitMessagePath, templatePath);
        if (injected)
        {
            _reporter.Info($"Inserted commit template from {templatePath}");
        }

        return 0;
    }

    private int RunCommitMsg(string[] args)
    {
        if (args.Length == 0)
        {
            _reporter.Error("Missing commit message file path.");
            return 1;
        }

        var commitMessagePath = args[0];
        if (!File.Exists(commitMessagePath))
        {
            _reporter.Error($"Commit message file not found: {commitMessagePath}");
            return 1;
        }

        var level = ValidationLevels.Parse(_git.GetConfig("xeyth.commitValidation"));
        if (level == ValidationLevel.Disable)
        {
            return 0;
        }

        var message = File.ReadAllText(commitMessagePath);
        var contextFiles = _git.GetContextFiles();
        var result = CommitMessageValidator.Validate(message, contextFiles);

        _reporter.ReportValidation(result, level);
        return result.ShouldBlock(level) ? 1 : 0;
    }

    private int Unknown(string command)
    {
        _reporter.Warning($"Unknown command: {command}");
        _reporter.Help();
        return 1;
    }

    private static bool IsHelp(string value)
    {
        return value is "help" or "--help" or "-h";
    }
}

public enum ValidationLevel
{
    Strict,
    Warn,
    Disable
}

internal static class ValidationLevels
{
    internal static ValidationLevel Parse(string? value)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            "warn" => ValidationLevel.Warn,
            "disable" => ValidationLevel.Disable,
            _ => ValidationLevel.Strict
        };
    }
}
