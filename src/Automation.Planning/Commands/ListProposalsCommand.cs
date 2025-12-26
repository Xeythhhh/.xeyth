using Automation.Cli.Common;
using Automation.Planning.Models;
using Automation.Planning.Services;
using Spectre.Console;

namespace Automation.Planning.Commands;

internal sealed class ListProposalsCommand : PlanningCommandBase
{
    private readonly ProposalDiscoveryService _discoveryService;

    internal ListProposalsCommand(IAnsiConsole console, PlanningReporter reporter, ProposalDiscoveryService discoveryService)
        : base(console, reporter)
    {
        _discoveryService = discoveryService ?? throw new ArgumentNullException(nameof(discoveryService));
    }

    public override string Name => "list-proposals";
    public override string Description => "List proposals with optional status filters.";

    public override async Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken)
    {
        if (ShouldShowHelp(args))
        {
            WriteUsage(Console);
            return 0;
        }

        var options = ListProposalsOptions.Parse(args);
        var proposals = await StatusSpinner.RunAsync(Console, "Discovering proposals...", () => _discoveryService.DiscoverAsync(options.RootPath, cancellationToken));

        var filtered = FilterProposals(proposals, options);

        if (filtered.Count == 0)
        {
            Reporter.Info("No proposals found for the specified filters.");
            return 0;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(ColorScheme.Primary);

        table.AddColumn("Name");
        table.AddColumn("Status");
        table.AddColumn("Submitted");
        table.AddColumn("Author");
        table.AddColumn("Path");

        foreach (var proposal in filtered)
        {
            table.AddRow(
                $"[bold]{Markup.Escape(proposal.Name)}[/]",
                proposal.Status.ToString(),
                string.IsNullOrWhiteSpace(proposal.Submitted) ? "-" : proposal.Submitted,
                string.IsNullOrWhiteSpace(proposal.Author) ? "-" : proposal.Author,
                Markup.Escape(Path.GetRelativePath(options.RootPath, proposal.Path)));
        }

        Console.Write(table);
        Console.WriteLine();
        return 0;
    }

    public override void WriteUsage(IAnsiConsole console)
    {
        var table = new Table().NoBorder().HideHeaders();
        table.AddColumn(new TableColumn(string.Empty));
        table.AddColumn(new TableColumn(string.Empty));

        table.AddRow("[bold]Usage[/]", "xeyth-planning list-proposals [--pending|--all|--status <Status>] [--root <path>]");
        table.AddEmptyRow();
        table.AddRow("[bold]Options[/]", "--pending       Show pending proposals (default)\n--all           Show all proposals\n--status <s>    Filter by status (Pending, Accepted, Deferred, Rejected)\n--root <path>   Root directory (defaults to current)");

        console.Write(table);
        console.WriteLine();
    }

    private static List<Proposal> FilterProposals(IReadOnlyList<Proposal> proposals, ListProposalsOptions options)
    {
        IEnumerable<Proposal> query = proposals;

        if (options.StatusFilter is { } status)
        {
            query = query.Where(p => p.Status == status);
        }
        else if (options.PendingOnly)
        {
            query = query.Where(p => p.Status == ProposalStatus.Pending);
        }

        return query.ToList();
    }

    private sealed record ListProposalsOptions(string RootPath, ProposalStatus? StatusFilter, bool PendingOnly)
    {
        internal static ListProposalsOptions Parse(string[] args)
        {
            var queue = new Queue<string>(args);
            var rootPath = Directory.GetCurrentDirectory();
            ProposalStatus? status = ProposalStatus.Pending;
            var pendingOnly = true;

            while (queue.Count > 0)
            {
                var token = queue.Dequeue();
                switch (token)
                {
                    case "--root":
                        EnsureHasValue(queue, token);
                        rootPath = queue.Dequeue();
                        break;

                    case "--pending":
                        status = ProposalStatus.Pending;
                        pendingOnly = true;
                        break;

                    case "--all":
                        status = null;
                        pendingOnly = false;
                        break;

                    case "--status":
                        EnsureHasValue(queue, token);
                        var value = queue.Dequeue();
                        if (!Enum.TryParse<ProposalStatus>(value, ignoreCase: true, out var parsed) || parsed == ProposalStatus.Unknown)
                        {
                            throw new ArgumentException($"Unknown status: {value}");
                        }

                        status = parsed;
                        pendingOnly = false;
                        break;

                    default:
                        throw new ArgumentException($"Unknown option: {token}");
                }
            }

            return new ListProposalsOptions(Path.GetFullPath(rootPath), status, pendingOnly);
        }
    }
}
