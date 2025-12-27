using Automation.Planning.Models;
using Automation.Planning.Services;

namespace Automation.Planning.Tests.Services;

public sealed class TaskCreationServiceTests
{
    [Fact]
    public void CreateTask_PopulatesTemplateAndUsesRelativePath()
    {
        using var workspace = new TempWorkspace();
        var templatePath = workspace.WriteFile(
            "Planning/Task.task.template",
            """
            # Task Template
            **Task File**: {Slice}/{TaskName}.task
            **Objective**: {One-line objective}
            {TaskPath}
            """);

        var service = new TaskCreationService();
        var proposal = new Proposal("Automation.CliTool", "CLI", ProposalStatus.Pending, "path", null, null, null, "content");

        var created = service.CreateTask(workspace.Root, "Automation/PlanningCliTool", proposal);

        Assert.True(File.Exists(created));
        var content = File.ReadAllText(created);
        Assert.Contains("PlanningCliTool", content);
        Assert.Contains("Automation/PlanningCliTool.task", content);
        Assert.DoesNotContain("{", content);
    }

    [Fact]
    public void CreateTask_ThrowsWhenTemplateMissing()
    {
        using var workspace = new TempWorkspace();
        var service = new TaskCreationService();
        var proposal = new Proposal("Automation.CliTool", "CLI", ProposalStatus.Pending, "path", null, null, null, "content");

        Assert.Throws<FileNotFoundException>(() => service.CreateTask(workspace.Root, "Automation/PlanningCliTool", proposal));
    }

    [Fact]
    public void CreateTask_ThrowsWhenOutsideRoot()
    {
        using var workspace = new TempWorkspace();
        workspace.WriteFile(
            "Planning/Task.task.template",
            """
            # Task Template
            """);

        var service = new TaskCreationService();
        var proposal = new Proposal("Automation.CliTool", "CLI", ProposalStatus.Pending, "path", null, null, null, "content");

        Assert.Throws<InvalidOperationException>(() => service.CreateTask(workspace.Root, "../Outside", proposal));
    }
}
