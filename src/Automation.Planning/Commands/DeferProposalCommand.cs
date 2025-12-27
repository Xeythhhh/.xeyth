using Automation.Cli.Common;
using Automation.Planning.Models;
using Automation.Planning.Services;
using Spectre.Console;

namespace Automation.Planning.Commands;

internal sealed class DeferProposalCommand : PlanningCommandBase
{
    private readonly ProposalDiscoveryService _discoveryService;
    private readonly ProposalDecisionService _decisionService;

    internal DeferProposalCommand(IAnsiConsole console, PlanningReporter reporter, ProposalDiscoveryService discoveryService, ProposalDecisionService decisionService)
        : base(console, reporter)
    {
        _discoveryService = discoveryService ?? throw new ArgumentNullException(nameof(discoveryService));
        _decisionService = decisionService ?? throw new ArgumentNullException(nameof(decisionService));
    }

    public override string Name => "defer-proposal";
    public override string Description => "Defer a proposal and archive it with rationale.";

    public override async Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken)
    {
        if (ShouldShowHelp(args))
        {
            WriteUsage(Console);
            return 0;
        }

        var options = DecisionOptions.Parse(args);
        var proposal = await _discoveryService.FindByNameAsync(options.RootPath, options.Name, cancellationToken);

        if (proposal is null)
        {
            Reporter.Error($"Proposal not found: {options.Name}");
            return 1;
        }

        var archivePath = _decisionService.ArchiveDecision(proposal, ProposalStatus.Deferred, options.RootPath, options.Reason);
        var relativeArchivePath = NormalizePath(Path.GetRelativePath(options.RootPath, archivePath));
        Reporter.Success(
            $"Deferred proposal {proposal.Name}",
            $"Archived to {relativeArchivePath}");

        return 0;
    }

    public override void WriteUsage(IAnsiConsole console)
    {
        var table = new Table().NoBorder().HideHeaders();
        table.AddColumn(new TableColumn(string.Empty));
        table.AddColumn(new TableColumn(string.Empty));

        table.AddRow("[bold]Usage[/]", Markup.Escape("xeyth-planning defer-proposal <name> --reason <text> [--root <path>]"));
        table.AddEmptyRow();
        table.AddRow("[bold]Options[/]", Markup.Escape("--reason <text>  Rationale for deferring (required)\n--root <path>   Root directory (defaults to current)"));

        console.Write(table);
        console.WriteLine();
    }

}
