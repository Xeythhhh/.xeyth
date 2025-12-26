using GitTool;

namespace GitTool.Tests;

public sealed class CommitMessageValidatorTests
{
    [Fact]
    public void Validate_Succeeds_ForValidMessage()
    {
        var result = CommitMessageValidator.Validate("feat(git): add hook", Array.Empty<string>());

        Assert.False(result.HasErrors);
        Assert.False(result.HasWarnings);
    }

    [Fact]
    public void Validate_Succeeds_ForScopeLessMessage()
    {
        var result = CommitMessageValidator.Validate("feat: add feature", Array.Empty<string>());

        Assert.False(result.HasErrors);
    }

    [Fact]
    public void Validate_Fails_ForInvalidType()
    {
        var result = CommitMessageValidator.Validate("invalid(git): message", Array.Empty<string>());

        Assert.Contains(result.Errors, error => error.Contains("Invalid type"));
    }

    [Fact]
    public void Validate_Fails_ForInvalidScope()
    {
        var result = CommitMessageValidator.Validate("feat(unknown): message", Array.Empty<string>());

        Assert.Contains(result.Errors, error => error.Contains("Invalid scope"));
    }

    [Fact]
    public void Validate_Fails_WhenSubjectTooLong()
    {
        var subject = new string('a', 51);
        var result = CommitMessageValidator.Validate($"feat(git): {subject}", Array.Empty<string>());

        Assert.Contains(result.Errors, error => error.Contains("50 characters"));
    }

    [Fact]
    public void Validate_Fails_WhenSubjectStartsWithUppercase()
    {
        var result = CommitMessageValidator.Validate("feat(git): Invalid subject", Array.Empty<string>());

        Assert.Contains(result.Errors, error => error.Contains("lowercase"));
    }

    [Fact]
    public void Validate_Fails_WhenSubjectEndsWithPeriod()
    {
        var result = CommitMessageValidator.Validate("feat(git): ends with period.", Array.Empty<string>());

        Assert.Contains(result.Errors, error => error.Contains("period"));
    }

    [Fact]
    public void Validate_Warns_WhenBodyLineTooLong()
    {
        var message = """
                      feat(git): add body

                      This is a body line that is intentionally made longer than the recommended seventy-two characters to trigger a warning.
                      """;

        var result = CommitMessageValidator.Validate(message, Array.Empty<string>());

        Assert.Contains(result.Warnings, warning => warning.Contains("72 characters"));
    }

    [Fact]
    public void Validate_Warns_WhenContextFilesMissingFooter()
    {
        var result = CommitMessageValidator.Validate("feat(git): add context", new[] { "Automation/Automation.task" });

        Assert.Contains(result.Warnings, warning => warning.Contains("Context files"));
    }

    [Fact]
    public void Validate_Allows_ContextFooterWhenPresent()
    {
        var message = """
                      feat(git): add context footer

                      Context: Automation/Automation.task
                      """;

        var result = CommitMessageValidator.Validate(message, new[] { "Automation/Automation.task" });

        Assert.False(result.HasWarnings);
    }

    [Fact]
    public void Validate_Skips_MergeMessages()
    {
        var result = CommitMessageValidator.Validate("Merge branch 'feature/example'", Array.Empty<string>());

        Assert.False(result.HasErrors);
    }
}
