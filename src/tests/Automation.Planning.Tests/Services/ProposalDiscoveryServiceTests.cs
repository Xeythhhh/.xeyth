using Automation.Planning.Models;
using Automation.Planning.Services;

namespace Automation.Planning.Tests.Services;

public sealed class ProposalDiscoveryServiceTests
{
    [Fact]
    public async Task DiscoverAsync_FindsAllProposals()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteFile(
            "Planning/Proposed/Framework.First.proposal",
            """
            # Proposal: First
            **Status**: Pending
            """);

        workspace.WriteFile(
            "Planning/Proposed/archive/Framework.Second.proposal",
            """
            # Proposal: Second
            **Status**: Accepted
            """);

        var service = new ProposalDiscoveryService(new ProposalParser());

        var proposals = await service.DiscoverAsync(workspace.Root);

        Assert.Equal(2, proposals.Count);
        Assert.Contains(proposals, p => p.Name == "Framework.First" && p.Status == ProposalStatus.Pending);
        Assert.Contains(proposals, p => p.Name == "Framework.Second" && p.Status == ProposalStatus.Accepted);
    }
}
