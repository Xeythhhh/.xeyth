using GitTool;

namespace GitTool.Tests;

public sealed class ValidationLevelsTests
{
    [Theory]
    [InlineData("warn", ValidationLevel.Warn)]
    [InlineData("WARN", ValidationLevel.Warn)]
    [InlineData("disable", ValidationLevel.Disable)]
    [InlineData("DISABLE", ValidationLevel.Disable)]
    [InlineData(null, ValidationLevel.Strict)]
    [InlineData("", ValidationLevel.Strict)]
    [InlineData("unknown", ValidationLevel.Strict)]
    public void Parse_ReturnsExpected(string? value, ValidationLevel expected)
    {
        var level = ValidationLevels.Parse(value);

        Assert.Equal(expected, level);
    }
}
