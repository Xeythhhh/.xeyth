using Automation.Planning.Commands;
using Automation.Planning.Services;
using Spectre.Console.Testing;

namespace Automation.Planning.Tests.Commands;

public sealed class ShowProposalCommandTests
{
    [Fact]
    public async Task Execute_ShowsSummaryAndTruncationMessage()
    {
        using var workspace = new TempWorkspace();
        var longContent = string.Join(Environment.NewLine, Enumerable.Range(0, 90).Select(i => $"Line {i}"));
        workspace.WriteFile(
            "Planning/Proposed/Automation.CliTool.proposal",
            $$"""
            # Proposal: Planning CLI

            **Status**: Pending
            **Submitted**: 2024-12-24
            **Author**: Implementation Agent
            **Related Task**: Automation/PlanningCliTool.task

            {{longContent}}
            """);

        var console = new TestConsole();
        var reporter = new PlanningReporter(console);
        var discovery = new ProposalDiscoveryService(new ProposalParser());
        var command = new ShowProposalCommand(console, reporter, discovery);

        var exitCode = await command.ExecuteAsync(new[] { "Automation.CliTool", "--root", workspace.Root }, CancellationToken.None);

        Assert.Equal(0, exitCode);
        var output = console.Output;
        Assert.Contains("Automation.CliTool", output);
        Assert.Contains("showing first 80", output);
    }
}
