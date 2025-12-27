using Automation.Planning.Commands;
using Automation.Planning.Services;
using Spectre.Console.Testing;

namespace Automation.Planning.Tests.Commands;

public sealed class RejectProposalCommandTests
{
    [Fact]
    public async Task Execute_ArchivesWithRejectedStatus()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteFile(
            "Planning/Proposed/Automation.CliTool.proposal",
            """
            # Proposal: Planning CLI

            **Status**: Pending
            """);

        var console = new TestConsole();
        var reporter = new PlanningReporter(console);
        var discovery = new ProposalDiscoveryService(new ProposalParser());
        var decision = new ProposalDecisionService();
        var command = new RejectProposalCommand(console, reporter, discovery, decision);

        var exitCode = await command.ExecuteAsync(new[] { "Automation.CliTool", "--reason", "Not needed", "--root", workspace.Root }, CancellationToken.None);

        Assert.Equal(0, exitCode);
        var archiveDir = Path.Combine(workspace.Root, "Planning/Proposed/archive");
        var archived = Directory.EnumerateFiles(archiveDir, "*.proposal").Single();
        var content = File.ReadAllText(archived);

        Assert.Contains("**Status**: Rejected", content);
        Assert.Contains("Not needed", content);
        Assert.False(File.Exists(Path.Combine(workspace.Root, "Planning/Proposed/Automation.CliTool.proposal")));
    }
}
