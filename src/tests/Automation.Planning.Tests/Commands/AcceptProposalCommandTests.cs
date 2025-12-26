using Automation.Planning.Commands;
using Automation.Planning.Services;
using Spectre.Console.Testing;

namespace Automation.Planning.Tests.Commands;

public sealed class AcceptProposalCommandTests
{
    [Fact]
    public async Task Execute_CreatesTaskAndArchivesProposal()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteFile(
            "Planning/Proposed/Automation.CliTool.proposal",
            """
            # Proposal: Planning CLI

            **Status**: Pending  
            **Submitted**: 2024-12-24  
            **Author**: Implementation Agent  

            ## Context

            Body
            """);

        workspace.WriteFile(
            "Planning/Task.task.template",
            """
            # Task Template

            **Task File**: {Slice}/{TaskName}.task  
            **Objective**: {One-line objective}

            ## Task Details

            Status: {Not Started|In Progress|Complete|Blocked}
            """);

        var console = new TestConsole();
        var reporter = new PlanningReporter(console);
        var discovery = new ProposalDiscoveryService(new ProposalParser());
        var decision = new ProposalDecisionService();
        var taskCreation = new TaskCreationService();
        var command = new AcceptProposalCommand(console, reporter, discovery, decision, taskCreation);

        var exitCode = await command.ExecuteAsync(new[] { "Automation.CliTool", "--task", "Automation/PlanningCliTool.task", "--reason", "High impact", "--root", workspace.Root }, CancellationToken.None);

        Assert.Equal(0, exitCode);
        var taskPath = Path.Combine(workspace.Root, "Automation/PlanningCliTool.task");
        Assert.True(File.Exists(taskPath));
        var taskContent = File.ReadAllText(taskPath);
        Assert.Contains("Planning CLI", taskContent);

        var archiveDir = Path.Combine(workspace.Root, "Planning/Proposed/archive");
        Assert.True(Directory.Exists(archiveDir));
        var archived = Directory.EnumerateFiles(archiveDir, "*.proposal").Single();
        var archivedContent = File.ReadAllText(archived);

        Assert.Contains("**Status**: Accepted", archivedContent);
        Assert.Contains("**Task Created**: Automation/PlanningCliTool.task", archivedContent);
        Assert.Contains("High impact", archivedContent);
        Assert.False(File.Exists(Path.Combine(workspace.Root, "Planning/Proposed/Automation.CliTool.proposal")));
    }
}
