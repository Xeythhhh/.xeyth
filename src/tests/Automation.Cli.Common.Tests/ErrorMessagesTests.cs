using Automation.Cli.Common;

namespace Automation.Cli.Common.Tests;

public sealed class ErrorMessagesTests
{
    [Fact]
    public void MissingRequiredArgument_WithoutExample_ReturnsMessage()
    {
        var result = ErrorMessages.MissingRequiredArgument("proposal name");

        Assert.Contains("Missing required argument: proposal name", result);
    }

    [Fact]
    public void MissingRequiredArgument_WithExample_IncludesExample()
    {
        var result = ErrorMessages.MissingRequiredArgument("proposal name", "xeyth-planning show-proposal <name>");

        Assert.Contains("Missing required argument: proposal name", result);
        Assert.Contains("Example: xeyth-planning show-proposal <name>", result);
    }

    [Fact]
    public void UnknownOption_WithoutValidOptions_ShowsBasicMessage()
    {
        var result = ErrorMessages.UnknownOption("--invalid");

        Assert.Contains("Unknown option: --invalid", result);
        Assert.Contains("Use --help to see all available options.", result);
    }

    [Fact]
    public void UnknownOption_WithValidOptions_ListsOptions()
    {
        var result = ErrorMessages.UnknownOption("--invalid", new[] { "--root", "--path" });

        Assert.Contains("Unknown option: --invalid", result);
        Assert.Contains("--root", result);
        Assert.Contains("--path", result);
        Assert.Contains("Use --help to see all available options.", result);
    }

    [Fact]
    public void UnknownCommand_WithoutValidCommands_ShowsBasicMessage()
    {
        var result = ErrorMessages.UnknownCommand("invalid");

        Assert.Contains("Unknown command: invalid", result);
        Assert.Contains("Use --help to see usage information.", result);
    }

    [Fact]
    public void UnknownCommand_WithValidCommands_ListsCommands()
    {
        var result = ErrorMessages.UnknownCommand("invalid", new[] { "setup", "validate" });

        Assert.Contains("Unknown command: invalid", result);
        Assert.Contains("setup", result);
        Assert.Contains("validate", result);
    }

    [Fact]
    public void MissingValue_WithoutExpectedType_ShowsBasicMessage()
    {
        var result = ErrorMessages.MissingValue("--tool");

        Assert.Contains("Missing value for --tool", result);
    }

    [Fact]
    public void MissingValue_WithExpectedType_ShowsExpectedFormat()
    {
        var result = ErrorMessages.MissingValue("--tool", "tool-name");

        Assert.Contains("Missing value for --tool", result);
        Assert.Contains("Expected: --tool <tool-name>", result);
    }

    [Fact]
    public void FileNotFound_WithoutSuggestion_ShowsBasicMessage()
    {
        var result = ErrorMessages.FileNotFound("/path/to/file.txt");

        Assert.Contains("File not found: /path/to/file.txt", result);
        Assert.Contains("Please check that the file path is correct and the file exists.", result);
    }

    [Fact]
    public void FileNotFound_WithSuggestion_ShowsSuggestion()
    {
        var result = ErrorMessages.FileNotFound("/path/to/file.txt", "Use --workspace to specify a valid file.");

        Assert.Contains("File not found: /path/to/file.txt", result);
        Assert.Contains("Suggestion: Use --workspace to specify a valid file.", result);
    }

    [Fact]
    public void DirectoryNotFound_WithoutSuggestion_ShowsBasicMessage()
    {
        var result = ErrorMessages.DirectoryNotFound("/path/to/dir");

        Assert.Contains("Directory not found: /path/to/dir", result);
        Assert.Contains("Please check that the directory path is correct and the directory exists.", result);
    }

    [Fact]
    public void DirectoryNotFound_WithSuggestion_ShowsSuggestion()
    {
        var result = ErrorMessages.DirectoryNotFound("/path/to/dir", "Use --root to specify a valid directory.");

        Assert.Contains("Directory not found: /path/to/dir", result);
        Assert.Contains("Suggestion: Use --root to specify a valid directory.", result);
    }

    [Fact]
    public void PathMustBeWithinWorkspace_ShowsBothPaths()
    {
        var result = ErrorMessages.PathMustBeWithinWorkspace("/outside/path", "/workspace/root");

        Assert.Contains("Path must be within workspace root.", result);
        Assert.Contains("Provided path: /outside/path", result);
        Assert.Contains("Workspace root: /workspace/root", result);
    }

    [Fact]
    public void PathMustBeWithinWorkspace_WithSuggestion_ShowsSuggestion()
    {
        var result = ErrorMessages.PathMustBeWithinWorkspace("/outside/path", "/workspace/root", "Use --path to specify a valid path.");

        Assert.Contains("Suggestion: Use --path to specify a valid path.", result);
    }

    [Fact]
    public void InvalidValue_WithoutValidValues_ShowsBasicMessage()
    {
        var result = ErrorMessages.InvalidValue("--format", "invalid");

        Assert.Contains("Invalid value for --format: invalid", result);
    }

    [Fact]
    public void InvalidValue_WithValidValues_ListsValues()
    {
        var result = ErrorMessages.InvalidValue("--format", "invalid", new[] { "json", "xml", "yaml" });

        Assert.Contains("Invalid value for --format: invalid", result);
        Assert.Contains("json", result);
        Assert.Contains("xml", result);
        Assert.Contains("yaml", result);
    }

    [Fact]
    public void InvalidJson_WithoutDetail_ShowsBasicMessage()
    {
        var result = ErrorMessages.InvalidJson("/path/to/file.json");

        Assert.Contains("Invalid JSON file: /path/to/file.json", result);
        Assert.Contains("Please ensure the file contains valid JSON.", result);
    }

    [Fact]
    public void InvalidJson_WithDetail_ShowsDetail()
    {
        var result = ErrorMessages.InvalidJson("/path/to/file.json", "Unexpected token at position 5");

        Assert.Contains("Invalid JSON file: /path/to/file.json", result);
        Assert.Contains("Details: Unexpected token at position 5", result);
    }

    [Fact]
    public void AlreadyExists_WithoutSuggestion_ShowsBasicMessage()
    {
        var result = ErrorMessages.AlreadyExists("Task", "/path/to/task.task");

        Assert.Contains("Task already exists: /path/to/task.task", result);
    }

    [Fact]
    public void AlreadyExists_WithSuggestion_ShowsSuggestion()
    {
        var result = ErrorMessages.AlreadyExists("Task", "/path/to/task.task", "Choose a different name.");

        Assert.Contains("Task already exists: /path/to/task.task", result);
        Assert.Contains("Suggestion: Choose a different name.", result);
    }

    [Fact]
    public void NotFound_WithoutSuggestion_ShowsBasicMessage()
    {
        var result = ErrorMessages.NotFound("Proposal", "MyProposal");

        Assert.Contains("Proposal not found: MyProposal", result);
    }

    [Fact]
    public void NotFound_WithSuggestion_ShowsSuggestion()
    {
        var result = ErrorMessages.NotFound("Proposal", "MyProposal", "Use list-proposals to see available proposals.");

        Assert.Contains("Proposal not found: MyProposal", result);
        Assert.Contains("Suggestion: Use list-proposals to see available proposals.", result);
    }

    [Fact]
    public void PathMustBeAbsolute_ShowsPath()
    {
        var result = ErrorMessages.PathMustBeAbsolute("../relative/path");

        Assert.Contains("Path must be absolute: ../relative/path", result);
        Assert.Contains("Relative paths are not allowed in this context.", result);
    }

    [Fact]
    public void RequiredValue_WithoutContext_ShowsBasicMessage()
    {
        var result = ErrorMessages.RequiredValue("Task path");

        Assert.Contains("Task path is required", result);
    }

    [Fact]
    public void RequiredValue_WithContext_ShowsContext()
    {
        var result = ErrorMessages.RequiredValue("Task path", "Use --task to specify where to create the task file.");

        Assert.Contains("Task path is required", result);
        Assert.Contains("Context: Use --task to specify where to create the task file.", result);
    }
}
