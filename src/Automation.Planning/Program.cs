using Automation.Cli.Common;
using Automation.Planning.Commands;
using Automation.Planning.Services;
using Spectre.Console;

namespace Automation.Planning;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var console = ConsoleEnvironment.CreateConsole();
        var reporter = new PlanningReporter(console);
        var header = new CliHeader("xeyth-planning", VersionHelper.GetVersion(), "Proposal & backlog management");
        header.Render(console);

        var parser = new ProposalParser();
        var discoveryService = new ProposalDiscoveryService(parser);
        var decisionService = new ProposalDecisionService();
        var taskCreationService = new TaskCreationService();

        var dispatcher = new CommandDispatcher(
            console,
            reporter,
            new IPlanningCommand[]
            {
                new ListProposalsCommand(console, reporter, discoveryService),
                new ShowProposalCommand(console, reporter, discoveryService),
                new AcceptProposalCommand(console, reporter, discoveryService, decisionService, taskCreationService),
                new DeferProposalCommand(console, reporter, discoveryService, decisionService),
                new RejectProposalCommand(console, reporter, discoveryService, decisionService)
            });

        try
        {
            return await dispatcher.ExecuteAsync(args, CancellationToken.None);
        }
        catch (Exception ex)
        {
            reporter.Error(ex.Message);
            return 1;
        }
    }
}
