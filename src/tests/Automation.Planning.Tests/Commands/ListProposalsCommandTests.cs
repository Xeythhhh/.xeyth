using Automation.Planning.Commands;
using Automation.Planning.Services;
using Spectre.Console.Testing;

namespace Automation.Planning.Tests.Commands;

public sealed class ListProposalsCommandTests
{
    [Fact]
    public async Task Execute_FiltersPendingByDefault()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteFile(
            "Planning/Proposed/Framework.Pending.proposal",
            """
            # Proposal: Pending
            **Status**: Pending
            """);

        workspace.WriteFile(
            "Planning/Proposed/Framework.Accepted.proposal",
            """
            # Proposal: Accepted
            **Status**: Accepted
            """);

        var console = new TestConsole();
        var reporter = new PlanningReporter(console);
        var discovery = new ProposalDiscoveryService(new ProposalParser());
        var command = new ListProposalsCommand(console, reporter, discovery);

        var exitCode = await command.ExecuteAsync(new[] { "--root", workspace.Root }, CancellationToken.None);

        Assert.Equal(0, exitCode);
        var output = StripAnsi(console.Output);
        Assert.Contains("Framework.Pending", output);
        Assert.DoesNotContain("Framework.Accepted", output);
    }

    private static string StripAnsi(string value)
    {
        return System.Text.RegularExpressions.Regex.Replace(value, "\\x1B\\[[0-9;]*[mK]", string.Empty);
    }
}
