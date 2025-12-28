using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Automation.Cli.Common;
using Xeyth.Common.IO.Paths;
using Contracts.Core.Models;
using Contracts.Core.Services;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Spectre.Console;
using Spectre.Console.Rendering;
using ValidationResult = Contracts.Core.Models.ValidationResult;

namespace Contracts.Cli;

internal sealed class ValidateCommandRunner
{
    private readonly IAnsiConsole _console;
    private readonly IContractDiscoveryService _discovery;
    private readonly IContractValidationService _validation;

    internal ValidateCommandRunner(
        IAnsiConsole? console = null,
        IContractDiscoveryService? discovery = null,
        IContractValidationService? validation = null)
    {
        _console = console ?? AnsiConsole.Console;
        _discovery = discovery ?? new ContractDiscoveryService();
        _validation = validation ?? new ContractValidationService();
    }

    internal async Task<int> RunAsync(ValidateOptions options, CancellationToken cancellationToken = default)
    {
        var targetPath = AbsolutePath.From(options.TargetPath);
        var isFile = File.Exists(targetPath.Value);
        var isDirectory = Directory.Exists(targetPath.Value);

        if (!isFile && !isDirectory)
        {
            _console.MarkupLine($"[red]Path not found[/]: {Markup.Escape(targetPath.Value)}");
            return 2;
        }

        var rootPath = isFile ? AbsolutePath.From(Path.GetDirectoryName(targetPath.Value)!) : targetPath;
        var contracts = await StatusSpinner.RunAsync(
            _console,
            "Discovering contracts...",
            () => _discovery.DiscoverContractsAsync(rootPath.Value, cancellationToken));
        if (contracts.Count == 0)
        {
            _console.MarkupLine($"[red]No contracts found[/] under {Markup.Escape(rootPath.Value)}");
            return 2;
        }

        var scopedContracts = FilterContracts(contracts, options.ContractName);
        if (scopedContracts.Count == 0)
        {
            _console.MarkupLine($"[red]No contract matched[/] {Markup.Escape(options.ContractName!)}");
            return 2;
        }

        var files = isFile
            ? new List<string> { targetPath.Value }
            : ResolveFiles(rootPath, scopedContracts);

        if (files.Count == 0)
        {
            _console.MarkupLine($"[yellow]No files matched the selected contracts[/] in {Markup.Escape(rootPath.Value)}.");
            return 0;
        }

        var results = await ValidateFilesAsync(files, scopedContracts, rootPath.Value, cancellationToken);

        foreach (var result in results)
        {
            RenderResult(result, rootPath.Value);
        }

        RenderSummary(files.Count, results, options.Strict);

        var errorCount = CountViolations(results, ViolationSeverity.Error);
        var warningCount = CountViolations(results, ViolationSeverity.Warning);
        var exitCode = errorCount > 0 || (options.Strict && warningCount > 0) ? 1 : 0;

        return exitCode;
    }

    private IReadOnlyList<ContractMetadata> FilterContracts(
        IReadOnlyList<ContractMetadata> contracts,
        string? requested)
    {
        if (string.IsNullOrWhiteSpace(requested))
        {
            return contracts;
        }

        var filtered = contracts
            .Where(contract => IsContractMatch(contract, requested))
            .ToList();

        return filtered;
    }

    private static bool IsContractMatch(ContractMetadata contract, string requested)
    {
        var sourceName = Path.GetFileNameWithoutExtension(contract.SourcePath ?? string.Empty);
        return string.Equals(sourceName, requested, StringComparison.OrdinalIgnoreCase)
            || string.Equals(sourceName + ".metadata", requested, StringComparison.OrdinalIgnoreCase);
    }

    private IReadOnlyList<string> ResolveFiles(AbsolutePath rootPath, IReadOnlyList<ContractMetadata> contracts)
    {
        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);

        foreach (var contract in contracts)
        {
            if (contract.Target is null)
            {
                continue;
            }

            matcher.AddIncludePatterns(contract.Target.Patterns);

            if (contract.Target.Exclude is { Count: > 0 })
            {
                matcher.AddExcludePatterns(contract.Target.Exclude);
            }
        }

        var matches = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(rootPath.Value)));

        return matches.Files
            .Select(match => AbsolutePath.From(Path.Combine(rootPath.Value, match.Path)))
            .Where(path => path.IsUnder(rootPath) && File.Exists(path.Value))
            .Distinct()
            .OrderBy(path => path.Value, StringComparer.OrdinalIgnoreCase)
            .Select(path => path.Value)
            .ToList();
    }

    private void RenderResult(ValidationResult result, string rootPath)
    {
        var relativePath = MakeRelative(rootPath, result.FilePath);
        var hasErrors = result.Violations.Any(v => v.Severity == ViolationSeverity.Error);
        var hasWarnings = result.Violations.Any(v => v.Severity == ViolationSeverity.Warning);
        var borderColor = hasErrors ? ColorScheme.Error : hasWarnings ? ColorScheme.Warning : ColorScheme.Success;

        IRenderable body;

        if (result.Violations.Count == 0)
        {
            body = new Markup($"[green]{Markup.Escape(relativePath)}[/]");
        }
        else
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("Severity");
            table.AddColumn("Code");
            table.AddColumn("Message");
            table.AddColumn("Location");

            foreach (var violation in result.Violations
                         .OrderByDescending(v => SeverityRank(v.Severity))
                         .ThenBy(v => v.Code, StringComparer.OrdinalIgnoreCase))
            {
                table.AddRow(
                    FormatSeverity(violation.Severity),
                    Markup.Escape(violation.Code),
                    Markup.Escape(violation.Message),
                    FormatLocation(violation));
            }

            body = table;
        }

        var panel = new Panel(body)
            .Header($"[bold]{Markup.Escape(relativePath)}[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(borderColor));

        _console.Write(panel);
        _console.WriteLine();
    }

    private void RenderSummary(int fileCount, IReadOnlyList<ValidationResult> results, bool strict)
    {
        var errors = CountViolations(results, ViolationSeverity.Error);
        var warnings = CountViolations(results, ViolationSeverity.Warning);
        var successes = results.Count - results.Count(r => r.Violations.Count > 0);

        _console.Write(SectionDivider.Create("Summary"));

        var summary = TableBuilder.CreateSummary("Validated", "Errors", "Warnings");
        summary.AddRowSafe(
            fileCount.ToString(CultureInfo.InvariantCulture),
            errors.ToString(CultureInfo.InvariantCulture),
            warnings.ToString(CultureInfo.InvariantCulture));

        _console.Write(summary);
        _console.WriteLine();

        var chart = ChartBuilder.CreateBreakdownChart(successes, warnings, errors);

        _console.Write(chart);
        _console.WriteLine();

        if (strict)
        {
            _console.MarkupLine("[yellow]Strict mode[/]: warnings will fail the build.");
        }
    }

    private async Task<IReadOnlyList<ValidationResult>> ValidateFilesAsync(
        IReadOnlyList<string> files,
        IReadOnlyList<ContractMetadata> contracts,
        string rootPath,
        CancellationToken cancellationToken)
    {
        return await ProgressHelper.RunAsync(
            _console,
            "Validating files",
            files,
            file => _validation.ValidateAsync(file, contracts, rootPath, cancellationToken),
            file => $"Validating {MakeRelative(rootPath, file)}...");
    }

    private static string FormatSeverity(ViolationSeverity severity) => severity switch
    {
        ViolationSeverity.Error => "[red]Error[/]",
        ViolationSeverity.Warning => "[yellow]Warning[/]",
        _ => "[cyan]Info[/]"
    };

    private static string FormatLocation(Violation violation)
    {
        var parts = new List<string>();

        if (violation.LineNumber is not null)
        {
            parts.Add($"line {violation.LineNumber}");
        }

        if (!string.IsNullOrWhiteSpace(violation.Section))
        {
            parts.Add(violation.Section!);
        }

        return parts.Count == 0 ? "" : string.Join(", ", parts);
    }

    private static int CountViolations(IReadOnlyList<ValidationResult> results, ViolationSeverity severity)
    {
        return results.Sum(result => result.Violations.Count(v => v.Severity == severity));
    }

    private static int SeverityRank(ViolationSeverity severity) => severity switch
    {
        ViolationSeverity.Error => 2,
        ViolationSeverity.Warning => 1,
        _ => 0
    };

    private static string MakeRelative(string rootPath, string filePath)
    {
        try
        {
            var relative = Path.GetRelativePath(rootPath, filePath);
            if (!relative.StartsWith("..", StringComparison.Ordinal))
            {
                return relative;
            }
        }
        catch
        {
            // Fall back to absolute path if relative resolution fails.
        }

        return filePath;
    }
}
