using System.Reflection;
using Automation.Cli.Common;
using Xeyth.Common.IO.Paths;
using Spectre.Console;

namespace Automation.Verify;

internal static class Program
{
    private static int Main(string[] args)
    {
        var console = ConsoleEnvironment.CreateConsole();
        var reporter = new Reporter(console);
        var header = new CliHeader("xeyth-verify", GetVersion(), "DiffEngine configuration helper");
        header.Render(console);

        var dispatcher = new CommandDispatcher(reporter, console);

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
    private readonly IAnsiConsole _console;

    internal CommandDispatcher(Reporter reporter, IAnsiConsole console)
    {
        _reporter = reporter;
        _console = console;
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
            "setup" => RunSetup(remaining),
            "validate" => RunValidate(remaining),
            _ => Unknown(command)
        };
    }

    private int RunSetup(string[] args)
    {
        var options = SetupOptions.Parse(args);
        var tool = ResolveTool(options.Tool);
        var targetDirectory = ResolveTargetDirectory(options.TargetDirectory);

        var result = VerifyConfigurator.Configure(targetDirectory.Value, tool);
        _reporter.Success(
            message: $"Created {result.ConfigPath}",
            detail: $"Diff tool order: {string.Join(", ", tool.ToolOrder)}");

        return 0;
    }

    private DiffTool ResolveTool(DiffTool? provided)
    {
        if (provided is not null)
        {
            return provided;
        }

        if (!ConsoleEnvironment.IsInteractive(_console))
        {
            _reporter.Info("Defaulting to VS Code Insiders (non-interactive).");
            return DiffTool.VisualStudioCodeInsiders;
        }

        var prompt = new SelectionPrompt<DiffTool>()
            .Title("Select diff tool")
            .UseConverter(tool => tool.DisplayName)
            .AddChoices(DiffTool.All);

        return _console.Prompt(prompt);
    }

    private int RunValidate(string[] args)
    {
        var options = ValidateOptions.Parse(args);
        var targetDirectory = ResolveTargetDirectory(options.TargetDirectory);
        var result = StatusSpinner.Run(_console, "Validating DiffEngine.json...", () => VerifyValidator.Validate(targetDirectory));

        if (result.IsValid)
        {
            _reporter.Success(result.Message);
            return 0;
        }

        _reporter.Warning(result.Message);
        return 1;
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

    internal static AbsolutePath ResolveTargetDirectory(string? targetDirectory)
    {
        var workspaceRoot = AbsolutePath.From(Directory.GetCurrentDirectory());
        var resolved = AbsolutePath.From(targetDirectory ?? workspaceRoot.Value);

        if (!resolved.IsUnder(workspaceRoot))
        {
            throw new InvalidOperationException(ErrorMessages.PathMustBeWithinWorkspace(
                resolved.Value,
                workspaceRoot.Value,
                "Use --path to specify a directory within the workspace."));
        }

        return resolved;
    }
    }

internal sealed record SetupOptions(DiffTool? Tool, string? TargetDirectory)
{
    internal static SetupOptions Parse(string[] args)
    {
        DiffTool? tool = null;
        string? targetDirectory = null;

        var queue = new Queue<string>(args);
        while (queue.Count > 0)
        {
            var token = queue.Dequeue();
            switch (token)
            {
                case "--tool":
                case "-t":
                    EnsureHasValue(queue, token);
                    var requested = queue.Dequeue();
                    if (!DiffTool.TryParse(requested, out tool))
                    {
                        var validTools = DiffTool.All.Select(t => t.DisplayName);
                        throw new ArgumentException(ErrorMessages.InvalidValue(token, requested, validTools));
                    }

                    break;

                case "--path":
                case "-p":
                case "--target":
                    EnsureHasValue(queue, token);
                    targetDirectory = queue.Dequeue();
                    break;

                default:
                    throw new ArgumentException(ErrorMessages.UnknownOption(
                        token,
                        new[] { "--tool", "-t", "--path", "-p", "--target" }));
            }
        }

        return new SetupOptions(tool, targetDirectory);
    }

    internal static void EnsureHasValue(Queue<string> queue, string token)
    {
        if (queue.Count == 0)
        {
            var valueType = token switch
            {
                "--tool" or "-t" => "tool",
                "--path" or "-p" or "--target" => "path",
                _ => "value"
            };
            throw new ArgumentException(ErrorMessages.MissingValue(token, valueType));
        }
    }
}

internal sealed record ValidateOptions(string? TargetDirectory)
{
    internal static ValidateOptions Parse(string[] args)
    {
        string? targetDirectory = null;
        var queue = new Queue<string>(args);

        while (queue.Count > 0)
        {
            var token = queue.Dequeue();
            switch (token)
            {
                case "--path":
                case "-p":
                case "--target":
                    SetupOptions.EnsureHasValue(queue, token);
                    targetDirectory = queue.Dequeue();
                    break;
                default:
                    throw new ArgumentException(ErrorMessages.UnknownOption(
                        token,
                        new[] { "--path", "-p", "--target" }));
            }
        }

        return new ValidateOptions(targetDirectory);
    }
}
