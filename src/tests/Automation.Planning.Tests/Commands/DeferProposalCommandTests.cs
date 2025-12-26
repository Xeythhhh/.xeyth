using Automation.Planning.Commands;
using Automation.Planning.Services;
using Spectre.Console.Testing;

namespace Automation.Planning.Tests.Commands;

public sealed class DeferProposalCommandTests
{
    [Fact]
    public async Task Execute_ArchivesWithDeferredStatus()
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
        var command = new DeferProposalCommand(console, reporter, discovery, decision);

        var exitCode = await command.ExecuteAsync(new[] { "Automation.CliTool", "--reason", "Later", "--root", workspace.Root }, CancellationToken.None);

        Assert.Equal(0, exitCode);
        var archiveDir = Path.Combine(workspace.Root, "Planning/Proposed/archive");
        var archived = Directory.EnumerateFiles(archiveDir, "*.proposal").Single();
        var content = File.ReadAllText(archived);

        Assert.Contains("**Status**: Deferred", content);
        Assert.Contains("Later", content);
        Assert.False(File.Exists(Path.Combine(workspace.Root, "Planning/Proposed/Automation.CliTool.proposal")));
    }
}
