using System.Text.Json;
using Automation.Verify;

namespace Automation.Verify.Tests;

public sealed class VerifyConfiguratorTests
{
    [Theory]
    [InlineData("VSCodeInsiders", new[] { "VisualStudioCodeInsiders", "VisualStudioCode" })]
    [InlineData("VSCode", new[] { "VisualStudioCode" })]
    [InlineData("VisualStudio", new[] { "VisualStudio" })]
    [InlineData("Rider", new[] { "Rider" })]
    public void Setup_WritesExpectedToolOrder(string toolName, string[] expectedOrder)
    {
        using var sandbox = new TempDirectory();
        Assert.True(DiffTool.TryParse(toolName, out var tool));

        var result = VerifyConfigurator.Configure(sandbox.Path, tool!);

        Assert.True(File.Exists(result.ConfigPath));
        var json = File.ReadAllText(result.ConfigPath);
        var config = JsonSerializer.Deserialize<DiffEngineConfiguration>(json);

        Assert.NotNull(config);
        Assert.Equal(expectedOrder, config!.ToolOrder);
    }

    [Fact]
    public void Validate_Succeeds_WhenConfigPresent()
    {
        using var sandbox = new TempDirectory();
        VerifyConfigurator.Configure(sandbox.Path, DiffTool.VisualStudioCode);

        var result = VerifyValidator.Validate(sandbox.Path);

        Assert.True(result.IsValid);
        Assert.Contains("DiffEngine.json is valid", result.Message);
    }

    [Fact]
    public void Validate_Fails_WhenConfigMissing()
    {
        using var sandbox = new TempDirectory();

        var result = VerifyValidator.Validate(sandbox.Path);

        Assert.False(result.IsValid);
        Assert.Contains("DiffEngine.json not found", result.Message);
    }
}

internal sealed class TempDirectory : IDisposable
{
    internal string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "xeyth-verify", Guid.NewGuid().ToString("N"));

    internal TempDirectory()
    {
        Directory.CreateDirectory(Path);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
        catch
        {
            // Best-effort cleanup.
        }
    }
}
