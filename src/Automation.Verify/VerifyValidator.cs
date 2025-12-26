using System.Text.Json;
using Automation.Cli.Common.Paths;

namespace Automation.Verify;

internal static class VerifyValidator
{
    internal static ValidationResult Validate(string targetDirectory)
    {
        if (string.IsNullOrWhiteSpace(targetDirectory))
        {
            return ValidationResult.Failure("Target directory is required.");
        }

        return Validate(AbsolutePath.From(targetDirectory));
    }

    internal static ValidationResult Validate(AbsolutePath targetDirectory)
    {
        var configPath = targetDirectory.Combine(".verify").Combine("DiffEngine.json");
        if (!File.Exists(configPath.Value))
        {
            return ValidationResult.Failure("DiffEngine.json not found. Run 'xeyth-verify setup' first.");
        }

        DiffEngineConfiguration? config;
        try
        {
            var json = File.ReadAllText(configPath.Value);
            config = JsonSerializer.Deserialize<DiffEngineConfiguration>(json);
        }
        catch (JsonException ex)
        {
            return ValidationResult.Failure($"DiffEngine.json is invalid JSON ({ex.Message}).");
        }

        if (config?.ToolOrder is null || config.ToolOrder.Count == 0)
        {
            return ValidationResult.Failure("DiffEngine.json is missing ToolOrder entries.");
        }

        var unknown = config.ToolOrder
            .Where(value => !DiffTool.IsKnownToolOrderValue(value))
            .ToList();

        if (unknown.Count > 0)
        {
            return ValidationResult.Failure($"Unrecognized tool order value(s): {string.Join(", ", unknown)}.");
        }

        return ValidationResult.Success(
            message: $"DiffEngine.json is valid (ToolOrder: {string.Join(", ", config.ToolOrder)}).");
    }
}

internal sealed record ValidationResult(bool IsValid, string Message)
{
    internal static ValidationResult Success(string message) => new(true, message);

    internal static ValidationResult Failure(string message) => new(false, message);
}
