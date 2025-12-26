using Automation.Cli.Common;
using Automation.Planning.Services;
using Spectre.Console;

namespace Automation.Planning.Commands;

internal sealed class ShowProposalCommand : PlanningCommandBase
{
    private readonly ProposalDiscoveryService _discoveryService;

    internal ShowProposalCommand(IAnsiConsole console, PlanningReporter reporter, ProposalDiscoveryService discoveryService)
        : base(console, reporter)
    {
        _discoveryService = discoveryService ?? throw new ArgumentNullException(nameof(discoveryService));
    }

    public override string Name => "show-proposal";
    public override string Description => "Show proposal details.";

    public override async Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken)
    {
        if (ShouldShowHelp(args))
        {
            WriteUsage(Console);
            return 0;
        }

        var options = ShowProposalOptions.Parse(args);
        var proposal = await _discoveryService.FindByNameAsync(options.RootPath, options.Name, cancellationToken);

        if (proposal is null)
        {
            Reporter.Error($"Proposal not found: {options.Name}");
            return 1;
        }

        var relativePath = NormalizePath(Path.GetRelativePath(options.RootPath, proposal.Path));

        var summary = new Table().NoBorder().HideHeaders();
        summary.AddColumn(new TableColumn(string.Empty));
        summary.AddColumn(new TableColumn(string.Empty));
        summary.AddRow("[bold]Title[/]", Markup.Escape(proposal.Title));
        summary.AddRow("[bold]Status[/]", proposal.Status.ToString());
        summary.AddRow("[bold]Submitted[/]", string.IsNullOrWhiteSpace(proposal.Submitted) ? "-" : proposal.Submitted);
        summary.AddRow("[bold]Author[/]", string.IsNullOrWhiteSpace(proposal.Author) ? "-" : proposal.Author);
        if (!string.IsNullOrWhiteSpace(proposal.RelatedTask))
        {
            summary.AddRow("[bold]Related Task[/]", Markup.Escape(proposal.RelatedTask));
        }
        summary.AddRow("[bold]Path[/]", Markup.Escape(relativePath));

        var panel = new Panel(summary)
            .Header($"[bold]{Markup.Escape(proposal.Name)}[/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(ColorScheme.Primary));

        Console.Write(panel);
        Console.WriteLine();

        var contentLines = proposal.Content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        const int maxContentLines = 80;
        var wasTruncated = contentLines.Length > maxContentLines;
        var trimmed = string.Join(Environment.NewLine, contentLines.Take(maxContentLines));
        Console.MarkupLine($"[dim]{Markup.Escape(trimmed)}[/]");
        if (wasTruncated)
        {
            Console.MarkupLine($"[dim]... (showing first {maxContentLines} of {contentLines.Length} lines)[/]");
        }
        Console.WriteLine();

        return 0;
    }

    public override void WriteUsage(IAnsiConsole console)
    {
        var table = new Table().NoBorder().HideHeaders();
        table.AddColumn(new TableColumn(string.Empty));
        table.AddColumn(new TableColumn(string.Empty));

        table.AddRow("[bold]Usage[/]", "xeyth-planning show-proposal <name> [--root <path>]");
        table.AddEmptyRow();
        table.AddRow("[bold]Options[/]", "--root <path>   Root directory (defaults to current)");

        console.Write(table);
        console.WriteLine();
    }

    private sealed record ShowProposalOptions(string Name, string RootPath)
    {
        internal static ShowProposalOptions Parse(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("Proposal name is required");
            }

            var name = args[0];
            var root = Directory.GetCurrentDirectory();
            var queue = new Queue<string>(args.Skip(1));

            while (queue.Count > 0)
            {
                var token = queue.Dequeue();
                switch (token)
                {
                    case "--root":
                        EnsureHasValue(queue, token);
                        root = queue.Dequeue();
                        break;
                    default:
                        throw new ArgumentException($"Unknown option: {token}");
                }
            }

            return new ShowProposalOptions(name, Path.GetFullPath(root));
        }
    }
}
