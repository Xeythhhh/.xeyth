# CLI Visual Components - Usage Guide

This document describes the visual components available in `Automation.Cli.Common` for creating beautiful, consistent CLI experiences.

## Overview

The `Automation.Cli.Common` library provides a comprehensive set of visual components that:
- Ensure consistent styling across all CLI tools
- Adapt automatically to different environments (interactive vs CI, colored vs NO_COLOR)
- Follow modern CLI UX best practices
- Support accessibility requirements

## Core Components

### CliHeader

Branded header with FigletText (in interactive mode) or simple text header.

**Usage:**
```csharp
var header = new CliHeader("xeyth-contracts", "1.0.0", "Contract validation", "📋");
header.Render(console);
```

**Features:**
- Automatic FigletText rendering in interactive terminals
- Falls back to simple text in CI/non-interactive environments
- Supports emoji icons
- Respects `NO_COLOR` and `XEYTH_NO_FIGLET` environment variables

### ColorScheme

Semantic color constants for consistent color usage.

**Usage:**
```csharp
var successColor = ColorScheme.Success;    // Lime
var errorColor = ColorScheme.Error;        // IndianRed
var warningColor = ColorScheme.Warning;    // Yellow
var infoColor = ColorScheme.Info;          // DeepSkyBlue1
var primaryColor = ColorScheme.Primary;    // Aqua
var accentColor = ColorScheme.Accent;      // MediumPurple
var mutedColor = ColorScheme.Muted;        // Grey63
```

### CliPanels

Pre-styled panels for success, error, warning, and info messages.

**Usage:**
```csharp
var successPanel = CliPanels.Success("Operation completed", "Created 3 files");
var errorPanel = CliPanels.Error("Validation failed", "5 errors found");
var warningPanel = CliPanels.Warning("Deprecated feature", "Use --new-flag instead");
var infoPanel = CliPanels.Info("Processing...", "This may take a moment");

console.Write(successPanel);
```

### StatusSpinner

Environment-aware spinner for operations in progress.

**Usage:**
```csharp
// Synchronous
var result = StatusSpinner.Run(console, "Processing...", () => 
{
    // Do work
    return processedData;
});

// Asynchronous
var result = await StatusSpinner.RunAsync(console, "Discovering files...", async () =>
{
    var files = await DiscoverFilesAsync();
    return files;
});
```

**Features:**
- Shows animated spinner in interactive mode
- Falls back to simple text output in CI mode
- Automatically handles async operations

### ProgressHelper

Progress bars for multi-item operations.

**Usage:**
```csharp
await ProgressHelper.RunAsync(
    console,
    "Validating files",
    files,
    async file => await ValidateAsync(file),
    file => $"Validating {file.Name}...");
```

**Features:**
- Shows progress bar with percentage in interactive mode (for 2+ items)
- Falls back to spinner or simple text in non-interactive mode
- Supports both void and result-returning actions

## Advanced Components

### TreeBuilder

Hierarchical tree structures with consistent styling.

**Usage:**
```csharp
var tree = TreeBuilder.Create("Contracts");
var frameworkNode = tree.AddNodeSafe("Framework/");
frameworkNode.AddNodeSafe("Flow.prompt.md");
frameworkNode.AddNodeSafe("Strategic.prompt.md");
frameworkNode.AddNodeColored("NEW: Implementation.prompt.md", ColorScheme.Success);

console.Write(tree);
```

**Features:**
- Automatic markup escaping for labels
- Colored node support
- Primary color styling by default

### ChartBuilder

Metrics visualization with bar charts and breakdown charts.

**Usage:**
```csharp
// Metrics bar chart with semantic colors
var chart = ChartBuilder.CreateMetricsChart("Test Results", 
    successCount: 85, 
    warningCount: 10, 
    errorCount: 5);

// Breakdown chart (percentage distribution)
var breakdown = ChartBuilder.CreateBreakdownChart(
    successCount: 85,
    warningCount: 10,
    errorCount: 5);

console.Write(chart);
console.Write(breakdown);
```

### SectionDivider

Horizontal rules for section separation.

**Usage:**
```csharp
// Rule with title
var rule = SectionDivider.Create("Summary");
console.Write(rule);

// Simple line without title
var line = SectionDivider.CreateLine();
console.Write(line);

// Custom color
var coloredRule = SectionDivider.Create("Important", ColorScheme.Warning);
console.Write(coloredRule);
```

### TableBuilder

Consistently styled tables with automatic escaping.

**Usage:**
```csharp
// Standard table with rounded borders
var table = TableBuilder.Create("Name", "Value", "Status");
table.AddRowSafe("File1.txt", "42", "OK");
table.AddRowSafe("File2.txt", "100", "OK");

// Minimal table without borders
var minimal = TableBuilder.CreateMinimal("Column1", "Column2");
minimal.AddRowSafe("Data1", "Data2");

// Summary table (for metrics)
var summary = TableBuilder.CreateSummary("Validated", "Errors", "Warnings");
summary.AddRowSafe("35", "5", "10");
```

**Note:** For tables with markup content (like `[bold]text[/]`), use the raw Spectre.Console Table API instead of TableBuilder.

### LiveDisplayHelper

Environment-aware live updating displays.

**Usage:**
```csharp
await LiveDisplayHelper.RunAsync(
    console,
    new Panel("Starting..."),
    async ctx =>
    {
        for (int i = 0; i < 10; i++)
        {
            ctx.Update(new Panel($"Processing {i}/10"));
            await Task.Delay(100);
        }
        ctx.Update(new Panel("[green]Complete![/]"));
    });
```

**Features:**
- Shows live updates in interactive mode
- Falls back to line-by-line output in CI mode
- Abstract context works in both modes

## Environment Detection

The `ConsoleEnvironment` class provides environment detection utilities:

```csharp
// Check if running in CI
bool isCi = ConsoleEnvironment.IsCi;

// Check if NO_COLOR is requested
bool noColor = ConsoleEnvironment.NoColorRequested;

// Check if FigletText is allowed
bool figletAllowed = ConsoleEnvironment.FigletAllowed;

// Check if environment is interactive
bool isInteractive = ConsoleEnvironment.IsInteractiveEnvironment;

// Create console with environment-aware settings
var console = ConsoleEnvironment.CreateConsole();
```

## Best Practices

### 1. Always Use Semantic Colors

```csharp
// Good
panel.BorderStyle(new Style(ColorScheme.Success));

// Avoid
panel.BorderStyle(new Style(Color.Green));
```

### 2. Escape User Content

```csharp
// Good - automatically escaped
var table = TableBuilder.Create("Name");
table.AddRowSafe(userInput);

// Manual escaping when needed
console.MarkupLine($"[green]{Markup.Escape(userInput)}[/]");
```

### 3. Respect Environment

```csharp
// Good - adapts to environment
var result = await StatusSpinner.RunAsync(console, "Working...", DoWorkAsync);

// Avoid - always shows spinner
await console.Status().StartAsync("Working...", DoWorkAsync);
```

### 4. Use Appropriate Components

- **StatusSpinner**: Quick operations (< 10 items)
- **ProgressHelper**: Multi-item operations (10+ items)
- **LiveDisplayHelper**: Real-time streaming updates
- **CliPanels**: Final status messages

## Testing

All components support testing with `Spectre.Console.Testing.TestConsole`:

```csharp
[Fact]
public void CliHeader_RendersCorrectly()
{
    var console = new TestConsole();
    var header = new CliHeader("test-tool", "1.0.0");
    
    header.Render(console);
    
    var output = console.Output;
    Assert.Contains("test-tool", output);
    Assert.Contains("1.0.0", output);
}
```

## Examples

See the following CLI tools for complete examples:
- `xeyth-contracts`: Uses headers, spinners, progress, panels, charts, rules, tables
- `xeyth-verify`: Uses headers, panels, interactive prompts

## Environment Variables

- `CI`: Detected as CI environment (disables animations)
- `NO_COLOR`: Disables all color output (https://no-color.org/)
- `XEYTH_NO_FIGLET`: Disables FigletText even in interactive mode
