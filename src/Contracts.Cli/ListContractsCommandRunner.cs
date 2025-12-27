using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Automation.Cli.Common;
using Contracts.Core.Rendering;
using Contracts.Core.Services;
using Spectre.Console;

namespace Contracts.Cli;

internal sealed class ListContractsCommandRunner
{
    private readonly IAnsiConsole _console;
    private readonly IContractDiscoveryService _discovery;

    internal ListContractsCommandRunner(
        IAnsiConsole? console = null,
        IContractDiscoveryService? discovery = null)
    {
        _console = console ?? AnsiConsole.Console;
        _discovery = discovery ?? new ContractDiscoveryService();
    }

    internal async Task<int> RunAsync(ListContractsOptions options, CancellationToken cancellationToken = default)
    {
        var targetPath = options.TargetPath;
        var isDirectory = Directory.Exists(targetPath);

        if (!isDirectory)
        {
            _console.MarkupLine($"[red]Directory not found[/]: {Markup.Escape(targetPath)}");
            return 2;
        }

        var contracts = await StatusSpinner.RunAsync(
            _console,
            "Discovering contracts...",
            () => _discovery.DiscoverContractsAsync(targetPath, cancellationToken));

        if (contracts.Count == 0)
        {
            _console.MarkupLine($"[red]No contracts found[/] under {Markup.Escape(targetPath)}");
            return 2;
        }

        _console.MarkupLine($"[green]Found {contracts.Count} contract(s)[/]");
        _console.WriteLine();

        // Use the renderer to display contracts
        var config = new RendererConfiguration
        {
            Style = options.Format,
            Verbose = options.Verbose,
            NoColor = options.NoColor,
            MaxWidth = options.MaxWidth
        };

        var renderer = RendererFactory.Create(config);

        // For table renderer, pass all contracts at once
        if (options.Format == RenderStyle.Table && contracts.Count > 1)
        {
            var context = new RenderContext
            {
                Contract = contracts[0],
                AdditionalContracts = contracts.Skip(1).ToList(),
                Output = Console.Out,
                Configuration = config,
                RootPath = targetPath
            };
            renderer.Render(context);
        }
        else
        {
            // For other renderers, render each contract separately
            foreach (var contract in contracts)
            {
                var context = new RenderContext
                {
                    Contract = contract,
                    Output = Console.Out,
                    Configuration = config,
                    RootPath = targetPath
                };
                renderer.Render(context);
                
                if (options.Format != RenderStyle.Compact && options.Format != RenderStyle.Table)
                {
                    _console.WriteLine();
                }
            }
        }

        return 0;
    }
}

internal sealed record ListContractsOptions(
    string TargetPath,
    RenderStyle Format,
    bool Verbose,
    bool NoColor,
    int? MaxWidth);
