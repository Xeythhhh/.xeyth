using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Automation.Cli.Common;
using Xeyth.Common.IO.Paths;
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

        if (!parseResult.Success || parseResult.Options is null)
        {
            WriteUsage(console, parseResult.Error);
            return Task.FromResult(2);
        }

        var runner = new ValidateCommandRunner(console);
        return runner.RunAsync(parseResult.Options, cancellationToken);
    }

    private static ParseResult Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return new ParseResult(false, false, null, "Missing command. Use 'validate'.");
        }

        if (IsHelp(args[0]))
        {
            return new ParseResult(false, true, null, null);
        }

        var command = args[0];
        if (!string.Equals(command, "validate", StringComparison.OrdinalIgnoreCase))
        {
            return new ParseResult(false, false, null, $"Unknown command: {command}");
        }

        string? targetPath = null;
        string? contractName = null;
        var strict = false;

        var queue = new Queue<string>(args.Skip(1));

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
                        return new ParseResult(false, true, null, null);

                    default:
                        return new ParseResult(false, false, null, $"Unknown option: {token}");
                }
            }
        }
        catch (ArgumentException ex)
        {
            return new ParseResult(false, false, null, ex.Message);
        }

        var workspaceRoot = AbsolutePath.From(Directory.GetCurrentDirectory());
        var requestedPath = targetPath ?? ".";
        var absoluteTarget = AbsolutePath.From(requestedPath);

        if (!absoluteTarget.IsUnder(workspaceRoot))
        {
            return new ParseResult(false, false, null, $"Path must be within workspace root: {workspaceRoot}");
        }

        var options = new ValidateOptions(absoluteTarget.Value, contractName, strict);
        return new ParseResult(true, false, options, null);
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

        grid.AddRow(new Text("Usage", bold), new Text("xeyth-contracts validate [options]"));
        grid.AddEmptyRow();
        grid.AddRow(new Text("Options", bold), new Text("--path, -p       File or directory to validate (defaults to current)\n--contract, -c  Filter by contract file name (without .metadata)\n--strict        Treat warnings as errors\n--help, -h      Show help"));
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
            throw new ArgumentException($"Missing value for {token}");
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

internal sealed record ParseResult(bool Success, bool ShowHelp, ValidateOptions? Options, string? Error);
