using GitTool;
using Spectre.Console;

namespace GitTool.Tests;

public sealed class ReporterTests
{
    [Fact]
    public void ReportValidation_WritesSuccess_WhenNoIssues()
    {
        using var writer = new StringWriter();
        var reporter = new Reporter(TestConsole(writer));

        reporter.ReportValidation(new ValidationResult(), ValidationLevel.Strict);

        Assert.Contains("looks good", writer.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ReportValidation_WritesWarnings_AndErrors()
    {
        var result = new ValidationResult();
        result.Warnings.Add("warn");
        result.Errors.Add("error");

        using var writer = new StringWriter();
        var reporter = new Reporter(TestConsole(writer));

        reporter.ReportValidation(result, ValidationLevel.Warn);

        var output = writer.ToString();
        Assert.Contains("warn", output, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("error", output, StringComparison.OrdinalIgnoreCase);
    }

    private static IAnsiConsole TestConsole(StringWriter writer)
    {
        var settings = new AnsiConsoleSettings
        {
            Ansi = AnsiSupport.No,
            ColorSystem = ColorSystemSupport.NoColors,
            Out = new AnsiConsoleOutput(writer),
            Interactive = InteractionSupport.No
        };

        return AnsiConsole.Create(settings);
    }
}
