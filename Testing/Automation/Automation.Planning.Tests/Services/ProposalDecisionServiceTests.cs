using Automation.Planning.Models;
using Automation.Planning.Services;

namespace Automation.Planning.Tests.Services;

public sealed class ProposalDecisionServiceTests
{
    [Fact]
    public void ArchiveDecision_WritesArchiveAndDeletesSource()
    {
        using var workspace = new TempWorkspace();
        var proposalPath = workspace.WriteFile(
            "Planning/Proposed/Sample.proposal",
            """
            # Proposal: Sample

            **Status**: Pending
            """);

        var proposal = new Proposal(
            "Sample",
            "Sample",
            ProposalStatus.Pending,
            proposalPath,
            null,
            null,
            null,
            File.ReadAllText(proposalPath));

        var service = new ProposalDecisionService();

        var archivePath = service.ArchiveDecision(proposal, ProposalStatus.Accepted, workspace.Root, "Done", "Tasks/Example.task");

        Assert.False(File.Exists(proposalPath));
        Assert.True(File.Exists(archivePath));

        var content = File.ReadAllText(archivePath);
        Assert.Contains("**Status**: Accepted", content);
        Assert.Contains("Tasks/Example.task", content);
        Assert.Contains("Done", content);
    }
}
