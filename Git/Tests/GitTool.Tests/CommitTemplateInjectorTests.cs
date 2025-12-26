using GitTool;
using GitTool.Tests.Infrastructure;

namespace GitTool.Tests;

public sealed class CommitTemplateInjectorTests
{
    [Fact]
    public void InjectIfEmpty_WritesTemplate_WhenFileEmpty()
    {
        using var sandbox = new TempDirectory();
        var messagePath = Path.Combine(sandbox.Path, "COMMIT_EDITMSG");
        var templatePath = Path.Combine(sandbox.Path, ".gitmessage");

        File.WriteAllText(messagePath, string.Empty);
        File.WriteAllText(templatePath, "template content");

        var injected = CommitTemplateInjector.InjectIfEmpty(messagePath, templatePath);

        Assert.True(injected);
        Assert.Equal("template content" + Environment.NewLine, File.ReadAllText(messagePath));
    }

    [Fact]
    public void InjectIfEmpty_DoesNotOverwrite_WhenContentExists()
    {
        using var sandbox = new TempDirectory();
        var messagePath = Path.Combine(sandbox.Path, "COMMIT_EDITMSG");
        var templatePath = Path.Combine(sandbox.Path, ".gitmessage");

        File.WriteAllText(messagePath, "existing");
        File.WriteAllText(templatePath, "template content");

        var injected = CommitTemplateInjector.InjectIfEmpty(messagePath, templatePath);

        Assert.False(injected);
        Assert.Equal("existing", File.ReadAllText(messagePath));
    }

    [Fact]
    public void ResolveTemplatePath_UsesFallback_WhenConfigMissing()
    {
        using var sandbox = new TempDirectory();
        var templatePath = Path.Combine(sandbox.Path, ".gitmessage");
        File.WriteAllText(templatePath, "template");

        var resolved = CommitTemplateInjector.ResolveTemplatePath(null, sandbox.Path);

        Assert.Equal(templatePath, resolved);
    }
}
