using Automation.Planning.Commands;

namespace Automation.Planning.Tests.Commands;

public sealed class PlanningCommandBaseTests
{
    [Fact]
    public void NormalizePath_ReplacesBackslashes()
    {
        var normalized = PlanningCommandBase.NormalizePath("root\\nested\\file.proposal");

        Assert.Equal("root/nested/file.proposal", normalized);
    }
}
