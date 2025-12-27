using Automation.Planning.Models;
using Automation.Planning.Services;

namespace Automation.Planning.Tests.Services;

public sealed class ProposalFormatterTests
{
    [Fact]
    public void UpdateStatus_ReplacesExistingStatus()
    {
        var content = """
        **Status**: Pending
        Body
        """;

        var updated = ProposalFormatter.UpdateStatus(content, ProposalStatus.Accepted);

        Assert.Contains("**Status**: Accepted", updated);
        Assert.DoesNotContain("Pending", updated);
    }

    [Fact]
    public void AppendDecision_AppendsDecisionBlock()
    {
        var content = "Body";
        var result = ProposalFormatter.AppendDecision(content, ProposalStatus.Deferred, new DateOnly(2024, 12, 24), "Later", "Task.path");

        Assert.Contains("**Status**: Deferred", result);
        Assert.Contains("2024-12-24", result);
        Assert.Contains("Later", result);
        Assert.Contains("Task.path", result);
    }
}
