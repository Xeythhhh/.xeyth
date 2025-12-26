using Automation.Planning.Models;
using Automation.Planning.Services;

namespace Automation.Planning.Tests.Services;

public sealed class ProposalParserTests
{
    [Fact]
    public void Parse_ReadsMetadataAndTitle()
    {
        using var workspace = new TempWorkspace();
        var file = workspace.WriteFile(
            "Planning/Proposed/Framework.Testing.proposal",
            """
            # Proposal: Improve Testing

            **Status**: Pending  
            **Submitted**: 2024-12-24  
            **Author**: Implementation Agent  
            **Related Task**: Automation/PlanningCliTool.task

            ## Context

            Text body.
            """);

        var parser = new ProposalParser();

        var proposal = parser.Parse(file);

        Assert.Equal("Framework.Testing", proposal.Name);
        Assert.Equal("Improve Testing", proposal.Title);
        Assert.Equal(ProposalStatus.Pending, proposal.Status);
        Assert.Equal("2024-12-24", proposal.Submitted);
        Assert.Equal("Implementation Agent", proposal.Author);
        Assert.Equal("Automation/PlanningCliTool.task", proposal.RelatedTask);
        Assert.Contains("Context", proposal.Content);
    }
}
