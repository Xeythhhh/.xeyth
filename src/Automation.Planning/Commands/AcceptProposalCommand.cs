using Automation.Cli.Common;
using Automation.Planning.Models;
using Automation.Planning.Services;
using Spectre.Console;

namespace Automation.Planning.Commands;

internal sealed class AcceptProposalCommand : PlanningCommandBase
{
    private readonly ProposalDiscoveryService _discoveryService;
    private readonly ProposalDecisionService _decisionService;
    private readonly TaskCreationService _taskCreationService;

    internal AcceptProposalCommand(
        IAnsiConsole console,
        PlanningReporter reporter,
        ProposalDiscoveryService discoveryService,
        ProposalDecisionService decisionService,
        TaskCreationService taskCreationService)
        : base(console, reporter)
    {
        _discoveryService = discoveryService ?? throw new ArgumentNullException(nameof(discoveryService));
        _decisionService = decisionService ?? throw new ArgumentNullException(nameof(decisionService));
        _taskCreationService = taskCreationService ?? throw new ArgumentNullException(nameof(taskCreationService));
    }

    public override string Name => "accept-proposal";
    public override string Description => "Accept a proposal, create a task, and archive the proposal.";

    public override async Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken)
    {
        if (ShouldShowHelp(args))
        {
            WriteUsage(Console);
            return 0;
        }

        var options = AcceptProposalOptions.Parse(args);
        var proposal = await _discoveryService.FindByNameAsync(options.RootPath, options.Name, cancellationToken);

        if (proposal is null)
        {
            Reporter.Error($"Proposal not found: {options.Name}");
            return 1;
        }

        var taskPath = StatusSpinner.Run(Console, "Creating task from template...", () => _taskCreationService.CreateTask(options.RootPath, options.TaskPath, proposal));
        var relativeTaskPath = NormalizePath(Path.GetRelativePath(options.RootPath, taskPath));
        var archivePath = _decisionService.ArchiveDecision(
            proposal,
            ProposalStatus.Accepted,
            options.RootPath,
            options.Reason,
            relativeTaskPath);
        var relativeArchivePath = NormalizePath(Path.GetRelativePath(options.RootPath, archivePath));

        Reporter.Success(
            $"Accepted proposal {proposal.Name}",
            $"Created {relativeTaskPath} and archived to {relativeArchivePath}");

        return 0;
    }

    public override void WriteUsage(IAnsiConsole console)
    {
        var table = new Table().NoBorder().HideHeaders();
        table.AddColumn(new TableColumn(string.Empty));
        table.AddColumn(new TableColumn(string.Empty));

        table.AddRow("[bold]Usage[/]", Markup.Escape("xeyth-planning accept-proposal <name> --task <path> [--reason <text>] [--root <path>]"));
        table.AddEmptyRow();
        table.AddRow("[bold]Options[/]", Markup.Escape("--task <path>   Target task path (relative or absolute)\n--reason <text> Optional rationale to record\n--root <path>   Root directory (defaults to current)"));

        console.Write(table);
        console.WriteLine();
    }

    private sealed record AcceptProposalOptions(string Name, string TaskPath, string RootPath, string? Reason)
    {
        internal static AcceptProposalOptions Parse(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException(ErrorMessages.MissingRequiredArgument(
                    "proposal name",
                    "xeyth-planning accept-proposal <name> --task <path>"));
            }

            var name = args[0];
            var queue = new Queue<string>(args.Skip(1));
            string? taskPath = null;
            string? reason = null;
            var root = Directory.GetCurrentDirectory();

            while (queue.Count > 0)
            {
                var token = queue.Dequeue();
                switch (token)
                {
                    case "--task":
                    case "-t":
                        EnsureHasValue(queue, token);
                        taskPath = queue.Dequeue();
                        break;

                    case "--reason":
                    case "-r":
                        EnsureHasValue(queue, token);
                        reason = queue.Dequeue();
                        break;

                    case "--root":
                        EnsureHasValue(queue, token);
                        root = queue.Dequeue();
                        break;

                    default:
                        throw new ArgumentException(ErrorMessages.UnknownOption(
                            token,
                            new[] { "--task", "-t", "--reason", "-r", "--root" }));
                }
            }

            if (string.IsNullOrWhiteSpace(taskPath))
            {
                throw new ArgumentException(ErrorMessages.MissingRequiredArgument(
                    "--task",
                    "xeyth-planning accept-proposal <name> --task <path>"));
            }

            return new AcceptProposalOptions(name, taskPath, Path.GetFullPath(root), reason);
        }
    }
}
