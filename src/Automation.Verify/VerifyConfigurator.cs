using System.Text.Json;
using System.Text.Json.Serialization;
using Automation.Cli.Common;

namespace Automation.Verify;

internal static class VerifyConfigurator
{
    internal static ConfigureResult Configure(string targetDirectory, DiffTool tool)
    {
        if (string.IsNullOrWhiteSpace(targetDirectory))
        {
            throw new ArgumentException(ErrorMessages.RequiredValue("Target directory"));
        }

        var verifyDirectory = Path.Combine(targetDirectory, ".verify");
        Directory.CreateDirectory(verifyDirectory);

        var config = new DiffEngineConfiguration(tool.ToolOrder);
        var json = JsonSerializer.Serialize(config, SerializerOptions.Value);
        var configPath = Path.Combine(verifyDirectory, "DiffEngine.json");
        File.WriteAllText(configPath, json);

        return new ConfigureResult(configPath, tool);
    }

    private static readonly Lazy<JsonSerializerOptions> SerializerOptions = new(() => new JsonSerializerOptions
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    });
}

internal sealed record ConfigureResult(string ConfigPath, DiffTool Tool);

internal sealed record DiffEngineConfiguration(IReadOnlyList<string> ToolOrder);
