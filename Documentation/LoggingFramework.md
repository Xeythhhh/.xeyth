# Logging & Diagnostics Framework

## Overview

The logging framework provides structured logging with verbosity control for CLI tools. It includes:

- **Verbosity Levels**: Quiet, Normal, Verbose, Debug
- **Log Levels**: Debug, Info, Warning, Error
- **Diagnostic Context**: Captures environment, version, arguments for troubleshooting
- **Spectre.Console Integration**: Uses existing CLI styling via CliPanels

## Quick Start

### Basic Usage

```csharp
using Automation.Cli.Common.Logging;
using Spectre.Console;

var console = ConsoleEnvironment.CreateConsole();
var logger = LoggerFactory.Create(console, VerbosityLevel.Normal);

logger.Info("Processing started");
logger.Success("Operation completed", "Processed 10 files");
logger.Warning("Deprecated feature used");
logger.Error("Operation failed", "File not found: config.json");
logger.Debug("Internal state: X=42, Y=13"); // Only shown with --debug
```

### Parse Verbosity from CLI Arguments

```csharp
var console = ConsoleEnvironment.CreateConsole();
var logger = LoggerFactory.CreateFromArgs(console, args);

// Supports: --quiet, -q, --verbose, -v, --debug
// Example: xeyth-planning list --verbose
```

### Create Diagnostic Context

```csharp
var context = DiagnosticContext.Create("xeyth-planning", "1.0.0", args);

// Log as JSON for CI/CD pipelines
logger.Debug("Diagnostic context", context.ToJson());

// Log formatted string
logger.Debug(context.ToString());
// Output: xeyth-planning v1.0.0 @ 2024-12-28T14:20:00.000Z [CWD: /workspace]
```

## Verbosity Level Behavior

| Level | Error | Success/Info | Warning | Debug |
|-------|-------|-------------|---------|-------|
| **Quiet** | ✓ | ✗ | ✗ | ✗ |
| **Normal** | ✓ | ✓ | ✗ | ✗ |
| **Verbose** | ✓ | ✓ | ✓ | ✗ |
| **Debug** | ✓ | ✓ | ✓ | ✓ |

## Integration with Existing Code

### Option 1: Direct Logger Usage

Replace direct `Console.WriteLine` calls:

```csharp
// Before
Console.WriteLine($"Updated {count} files");

// After
logger.Info($"Updated {count} files");
```

### Option 2: Extend Existing Reporters

Add logger to existing reporter classes:

```csharp
public class MyReporter
{
    private readonly ILogger _logger;

    public MyReporter(ILogger logger)
    {
        _logger = logger;
    }

    public void ReportProgress(string message)
    {
        _logger.Info(message);
    }

    public void ReportDebugInfo(string details)
    {
        _logger.Debug("Debug information", details);
    }
}
```

## Testing

The framework includes comprehensive tests demonstrating usage:

- `LoggerTests.cs` - 30+ tests covering all logging scenarios
- `DiagnosticContextTests.cs` - 11 tests for diagnostic context
- `LoggerFactoryTests.cs` - 19 tests for factory and argument parsing

## API Reference

### ILogger

```csharp
public interface ILogger
{
    VerbosityLevel Verbosity { get; set; }
    void Debug(string message, string? detail = null);
    void Info(string message, string? detail = null);
    void Warning(string message, string? detail = null);
    void Error(string message, string? detail = null);
    void Success(string message, string? detail = null);
    bool IsEnabled(LogLevel level);
}
```

### LoggerFactory

```csharp
public static class LoggerFactory
{
    ILogger Create(IAnsiConsole console, VerbosityLevel verbosity = VerbosityLevel.Normal);
    ILogger CreateFromArgs(IAnsiConsole console, string[] args);
    VerbosityLevel ParseVerbosityFromArgs(string[] args);
}
```

### DiagnosticContext

```csharp
public sealed class DiagnosticContext
{
    static DiagnosticContext Create(string toolName, string version, string[] arguments);
    string ToJson(bool indented = true);
    string ToString();
}
```

## Future Enhancements

- [ ] Structured log sinks (file, JSON, etc.)
- [ ] Integration with Microsoft.Extensions.Logging
- [ ] Performance metrics logging
- [ ] Log correlation IDs for distributed tracing
