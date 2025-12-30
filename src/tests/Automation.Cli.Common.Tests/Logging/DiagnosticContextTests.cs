using System.Text.Json;
using Automation.Cli.Common.Logging;

namespace Automation.Cli.Common.Tests.Logging;

// Disable parallel execution for this test class to avoid environment variable conflicts
[Collection("EnvironmentVariableTests")]
public sealed class DiagnosticContextTests
{
    [Fact]
    public void Constructor_ThrowsWhenToolNameIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DiagnosticContext(null!, "1.0.0", Array.Empty<string>()));
    }

    [Fact]
    public void Constructor_ThrowsWhenVersionIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DiagnosticContext("tool", null!, Array.Empty<string>()));
    }

    [Fact]
    public void Constructor_ThrowsWhenArgumentsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DiagnosticContext("tool", "1.0.0", null!));
    }

    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var args = new[] { "--verbose", "--output", "result.txt" };
        var env = new Dictionary<string, string> { { "CI", "true" } };

        var context = new DiagnosticContext("xeyth-planning", "1.2.3", args, env);

        Assert.Equal("xeyth-planning", context.ToolName);
        Assert.Equal("1.2.3", context.Version);
        Assert.Equal(args, context.Arguments);
        Assert.Equal(env, context.Environment);
        Assert.NotEqual(default, context.Timestamp);
        Assert.NotEmpty(context.WorkingDirectory);
    }

    [Fact]
    public void Create_CapturesEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("CI", "true");
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", "true");

        try
        {
            var context = DiagnosticContext.Create(
                "xeyth-planning",
                "1.0.0",
                new[] { "--list" });

            Assert.Contains("CI", context.Environment.Keys);
            Assert.Equal("true", context.Environment["CI"]);
            Assert.Contains("GITHUB_ACTIONS", context.Environment.Keys);
        }
        finally
        {
            Environment.SetEnvironmentVariable("CI", null);
            Environment.SetEnvironmentVariable("GITHUB_ACTIONS", null);
        }
    }

    [Fact]
    public void Create_IgnoresMissingEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("CI", null);
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", null);

        var context = DiagnosticContext.Create(
            "xeyth-planning",
            "1.0.0",
            Array.Empty<string>());

        Assert.DoesNotContain("CI", context.Environment.Keys);
        Assert.DoesNotContain("GITHUB_ACTIONS", context.Environment.Keys);
    }

    [Fact]
    public void ToJson_ReturnsValidJson()
    {
        var args = new[] { "--verbose" };
        var context = new DiagnosticContext("tool", "1.0.0", args);

        var json = context.ToJson();

        Assert.NotEmpty(json);
        var doc = JsonDocument.Parse(json);
        Assert.Equal("tool", doc.RootElement.GetProperty("toolName").GetString());
        Assert.Equal("1.0.0", doc.RootElement.GetProperty("version").GetString());
    }

    [Fact]
    public void ToJson_IncludesAllProperties()
    {
        var args = new[] { "--test" };
        var env = new Dictionary<string, string> { { "KEY", "value" } };
        var context = new DiagnosticContext("tool", "1.0.0", args, env);

        var json = context.ToJson();
        var doc = JsonDocument.Parse(json);

        Assert.True(doc.RootElement.TryGetProperty("timestamp", out _));
        Assert.True(doc.RootElement.TryGetProperty("toolName", out _));
        Assert.True(doc.RootElement.TryGetProperty("version", out _));
        Assert.True(doc.RootElement.TryGetProperty("arguments", out _));
        Assert.True(doc.RootElement.TryGetProperty("workingDirectory", out _));
        Assert.True(doc.RootElement.TryGetProperty("environment", out _));
    }

    [Fact]
    public void ToString_ContainsKeyInformation()
    {
        var context = new DiagnosticContext("xeyth-planning", "1.2.3", Array.Empty<string>());

        var str = context.ToString();

        Assert.Contains("xeyth-planning", str);
        Assert.Contains("1.2.3", str);
        Assert.Contains(context.WorkingDirectory, str);
    }
}
