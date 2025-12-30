using System.Text.Json;
using System.Text.Json.Serialization;

namespace Automation.Cli.Common.Logging;

/// <summary>
/// Captures diagnostic context for troubleshooting CLI tool execution.
/// </summary>
public sealed class DiagnosticContext
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; }

    [JsonPropertyName("toolName")]
    public string ToolName { get; init; }

    [JsonPropertyName("version")]
    public string Version { get; init; }

    [JsonPropertyName("arguments")]
    public string[] Arguments { get; init; }

    [JsonPropertyName("workingDirectory")]
    public string WorkingDirectory { get; init; }

    [JsonPropertyName("environment")]
    public Dictionary<string, string> Environment { get; init; }

    public DiagnosticContext(
        string toolName,
        string version,
        string[] arguments,
        Dictionary<string, string>? environment = null)
    {
        Timestamp = DateTime.UtcNow;
        ToolName = toolName ?? throw new ArgumentNullException(nameof(toolName));
        Version = version ?? throw new ArgumentNullException(nameof(version));
        Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        WorkingDirectory = Directory.GetCurrentDirectory();
        Environment = environment ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// Creates a diagnostic context with key environment variables.
    /// </summary>
    public static DiagnosticContext Create(string toolName, string version, string[] arguments)
    {
        var envVars = new Dictionary<string, string>();
        
        // Capture key environment variables for CI/CD diagnostics
        var keyVars = new[] { "CI", "GITHUB_ACTIONS", "NO_COLOR", "TERM", "DOTNET_CLI_TELEMETRY_OPTOUT" };
        
        foreach (var key in keyVars)
        {
            var value = System.Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrEmpty(value))
            {
                envVars[key] = value;
            }
        }

        return new DiagnosticContext(toolName, version, arguments, envVars);
    }

    /// <summary>
    /// Serializes the diagnostic context to JSON.
    /// </summary>
    public string ToJson(bool indented = true)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = indented,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return JsonSerializer.Serialize(this, options);
    }

    /// <summary>
    /// Creates a formatted string representation for logging.
    /// </summary>
    public override string ToString()
    {
        return $"{ToolName} v{Version} @ {Timestamp:O} [CWD: {WorkingDirectory}]";
    }
}
