using System;
using System.IO;
using Contracts.Cli;

namespace Contracts.Core.Tests.Cli;

public sealed class ValidateCommandTests : IDisposable
{
    private readonly string _root;
    private readonly string _originalCwd;

    public ValidateCommandTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "contracts-cli-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
        _originalCwd = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(_root);
    }

    [Fact]
    public async Task Validate_ReturnsZero_ForValidFile()
    {
        WriteMetadata(
            "contracts.metadata",
            """
            target:
              patterns:
                - "**/*.task"
            naming:
              pattern: "^Valid.*\\.task$"
            schema:
              requiredSections:
                - name: "Delegation Prompt"
                  level: 2
            """);

        WriteFile("Valid.task", "## Delegation Prompt\ncontent");

        var exitCode = await CliApp.RunAsync(new[] { "validate", "--path", _root });

        Assert.Equal(0, exitCode);
    }

    [Fact]
    public async Task Validate_ReturnsOne_ForWarningsInStrictMode()
    {
        WriteMetadata(
            "contracts.metadata",
            """
            target:
              patterns:
                - "**/*.task"
            archiving:
              directory: "archive"
              pattern: "^Archived.*\\.\\d{4}-\\d{2}-\\d{2}\\.task$"
            """);

        WriteFile("Archived.2024-12-24.task", "# heading");

        var exitCode = await CliApp.RunAsync(new[] { "validate", "--path", _root, "--strict" });

        Assert.Equal(1, exitCode);
    }

    [Fact]
    public async Task Validate_ReturnsUsageError_WhenPathEscapesWorkspace()
    {
        var outside = Path.Combine(Path.GetTempPath(), "contracts-cli-outside-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outside);

        try
        {
            var exitCode = await CliApp.RunAsync(new[] { "validate", "--path", outside });

            Assert.Equal(2, exitCode);
        }
        finally
        {
            if (Directory.Exists(outside))
            {
                Directory.Delete(outside, recursive: true);
            }
        }
    }

    private void WriteMetadata(string fileName, string content)
    {
        File.WriteAllText(Path.Combine(_root, fileName), content);
    }

    private void WriteFile(string fileName, string content)
    {
        File.WriteAllText(Path.Combine(_root, fileName), content);
    }

    public void Dispose()
    {
        Directory.SetCurrentDirectory(_originalCwd);
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }
}
