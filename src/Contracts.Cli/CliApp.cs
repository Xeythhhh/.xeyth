using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Automation.Cli.Common;
using Xeyth.Common.IO.Paths;
using Contracts.Core.Rendering;
using Contracts.Core.Services;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Contracts.Cli;

public static class CliApp
{
    public static Task<int> RunAsync(string[] args, IAnsiConsole? console = null, CancellationToken cancellationToken = default)
    {
        console ??= ConsoleEnvironment.CreateConsole();

        RenderHeader(console);

        var parseResult = Parse(args);

        if (parseResult.ShowHelp)
        {
            WriteUsage(console);
            return Task.FromResult(0);
        }

        if (!parseResult.Success)
        {
            WriteUsage(console, parseResult.Error);
            return Task.FromResult(2);
        }

        if (parseResult.ValidateOptions is not null)
        {
            var runner = new ValidateCommandRunner(console);
            return runner.RunAsync(parseResult.ValidateOptions, cancellationToken);
        }

        if (parseResult.ListOptions is not null)
        {
            var runner = new ListContractsCommandRunner(console);
            return runner.RunAsync(parseResult.ListOptions, cancellationToken);
        }

        WriteUsage(console, "Invalid command");
        return Task.FromResult(2);
    }

    private static ParseResult Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return ParseResult.WithError("Missing command. Use 'validate' or 'list'.");
        }

        if (IsHelp(args[0]))
        {
            return ParseResult.Help();
        }

        var command = args[0];
        
        return command.ToLowerInvariant() switch
        {
            "validate" => ParseValidateCommand(args.Skip(1).ToArray()),
            "list" or "list-contracts" => ParseListCommand(args.Skip(1).ToArray()),
            _ => ParseResult.WithError($"Unknown command: {command}. Use 'validate' or 'list'.")
        };
    }

    private static ParseResult ParseValidateCommand(string[] args)
    {
        string? targetPath = null;
        string? contractName = null;
        var strict = false;

        var queue = new Queue<string>(args);

        try
        {
            while (queue.Count > 0)
            {
                var token = queue.Dequeue();
                switch (token)
                {
                    case "--path":
                    case "-p":
                        EnsureHasValue(queue, token);
                        targetPath = queue.Dequeue();
                        break;

                    case "--contract":
                    case "-c":
                        EnsureHasValue(queue, token);
                        contractName = queue.Dequeue();
                        break;

                    case "--strict":
                        strict = true;
                        break;

                    case "--help":
                    case "-h":
                    case "help":
                        return ParseResult.Help();

                    default:
                        return ParseResult.WithError($"Unknown option: {token}");
                }
            }
        }
        catch (ArgumentException ex)
        {
            return ParseResult.WithError(ex.Message);
        }

        var workspaceRoot = AbsolutePath.From(Directory.GetCurrentDirectory());
        var requestedPath = targetPath ?? ".";
        var absoluteTarget = AbsolutePath.From(requestedPath);

        if (!absoluteTarget.IsUnder(workspaceRoot))
        {
            return ParseResult.WithError($"Path must be within workspace root: {workspaceRoot}");
        }

        var options = new ValidateOptions(absoluteTarget.Value, contractName, strict);
        return ParseResult.ValidateCommand(options);
    }

    private static ParseResult ParseListCommand(string[] args)
    {
        string? targetPath = null;
        var format = RenderStyle.Card;
        var verbose = false;
        var noColor = false;
        int? maxWidth = null;

        var queue = new Queue<string>(args);

        try
        {
            while (queue.Count > 0)
            {
                var token = queue.Dequeue();
                switch (token)
                {
                    case "--path":
                    case "-p":
                        EnsureHasValue(queue, token);
                        targetPath = queue.Dequeue();
                        break;

                    case "--format":
                    case "-f":
                        EnsureHasValue(queue, token);
                        var formatStr = queue.Dequeue();
                        if (!Enum.TryParse<RenderStyle>(formatStr, true, out format))
                        {
                            return ParseResult.WithError($"Invalid format: {formatStr}. Use card, table, tree, or compact.");
                        }
                        break;

                    case "--verbose":
                    case "-v":
                        verbose = true;
                        break;

                    case "--no-color":
                        noColor = true;
                        break;

                    case "--width":
                    case "-w":
                        EnsureHasValue(queue, token);
                        var widthStr = queue.Dequeue();
                        if (!int.TryParse(widthStr, out var width))
                        {
                            return ParseResult.WithError($"Invalid width: {widthStr}");
                        }
                        maxWidth = width;
                        break;

                    case "--help":
                    case "-h":
                    case "help":
                        return ParseResult.Help();

                    default:
                        return ParseResult.WithError($"Unknown option: {token}");
                }
            }
        }
        catch (ArgumentException ex)
        {
            return ParseResult.WithError(ex.Message);
        }

        var resolvedPath = Path.GetFullPath(targetPath ?? Directory.GetCurrentDirectory());
        var options = new ListContractsOptions(resolvedPath, format, verbose, noColor, maxWidth);
        return ParseResult.ListCommand(options);
    }

    private static void WriteUsage(IAnsiConsole console, string? error = null)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            console.MarkupLine($"[red]{Markup.Escape(error)}[/]");
            console.WriteLine();
        }

        var bold = new Style(decoration: Decoration.Bold);
        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();

        grid.AddRow(new Text("Usage", bold), new Text("xeyth-contracts <command> [options]"));
        grid.AddEmptyRow();
        grid.AddRow(new Text("Commands", bold), new Text("validate         Validate files against contracts\nlist             List discovered contracts"));
        grid.AddEmptyRow();
        grid.AddRow(new Text("validate options", bold), new Text("--path, -p       File or directory to validate (defaults to current)\n--contract, -c   Filter by contract file name (without .metadata)\n--strict         Treat warnings as errors"));
        grid.AddEmptyRow();
        grid.AddRow(new Text("list options", bold), new Text("--path, -p       Directory to search (defaults to current)\n--format, -f     Output format: card, table, tree, compact (default: card)\n--verbose, -v    Include verbose details\n--no-color       Disable color output\n--width, -w      Maximum output width"));
        grid.AddEmptyRow();
        grid.AddRow(new Text("Global options", bold), new Text("--help, -h       Show help"));
        grid.AddEmptyRow();
        grid.AddRow(new Text("Exit Codes", bold), new Text("0 = success/warnings, 1 = errors or strict warnings, 2 = usage errors"));

        console.Write(grid);
        console.WriteLine();
    }

    private static bool IsHelp(string token) => token is "--help" or "-h" or "help";

    private static void EnsureHasValue(Queue<string> queue, string token)
    {
        if (queue.Count == 0)
        {
            throw new ArgumentException(ErrorMessages.MissingValue(token));
        }
    }

    private static void RenderHeader(IAnsiConsole console)
    {
        var header = new CliHeader("xeyth-contracts", GetVersion(), "Contract validation", "📋");
        header.Render(console);
    }

    private static string GetVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var informational = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        if (!string.IsNullOrWhiteSpace(informational))
        {
            var buildMetadataIndex = informational.IndexOf('+');
            return buildMetadataIndex >= 0 ? informational[..buildMetadataIndex] : informational;
        }

        return assembly.GetName().Version?.ToString(3) ?? "1.0.0";
    }
}

internal sealed record ValidateOptions(string TargetPath, string? ContractName, bool Strict);

internal sealed record ParseResult(bool Success, bool ShowHelp, ValidateOptions? ValidateOptions, ListContractsOptions? ListOptions, string? Error)
{
    public static ParseResult ValidateCommand(ValidateOptions options) => new(true, false, options, null, null);
    public static ParseResult ListCommand(ListContractsOptions options) => new(true, false, null, options, null);
    public static ParseResult Help() => new(false, true, null, null, null);
    public static ParseResult WithError(string error) => new(false, false, null, null, error);
}
