using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Core.Services;
using Spectre.Console;

namespace Contracts.Cli;

public static class CliApp
{
    public static Task<int> RunAsync(string[] args, IAnsiConsole? console = null, CancellationToken cancellationToken = default)
    {
        console ??= AnsiConsole.Console;

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

        var resolvedPath = Path.GetFullPath(targetPath ?? Directory.GetCurrentDirectory());
        var options = new ValidateOptions(resolvedPath, contractName, strict);
        return new ParseResult(true, false, options, null);
    }

    private static void WriteUsage(IAnsiConsole console, string? error = null)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            console.MarkupLine($"[red]{Markup.Escape(error)}[/]");
            console.WriteLine();
        }

        var table = new Table().NoBorder().HideHeaders();
        table.AddColumn(new TableColumn(string.Empty));
        table.AddColumn(new TableColumn(string.Empty));

        table.AddRow("[bold]Usage[/]", "xeyth-contracts validate [options]");
        table.AddEmptyRow();
        table.AddRow("[bold]Options[/]", "--path, -p       File or directory to validate (defaults to current)\n--contract, -c  Filter by contract file name (without .metadata)\n--strict        Treat warnings as errors\n--help, -h      Show help");
        table.AddEmptyRow();
        table.AddRow("[bold]Exit Codes[/]", "0 = success/warnings, 1 = errors or strict warnings, 2 = usage errors");

        console.Write(table);
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
}

internal sealed record ValidateOptions(string TargetPath, string? ContractName, bool Strict);

internal sealed record ParseResult(bool Success, bool ShowHelp, ValidateOptions? Options, string? Error);
