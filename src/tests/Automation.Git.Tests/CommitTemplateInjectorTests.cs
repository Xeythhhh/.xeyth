namespace Automation.Git.Tests;

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

internal sealed class TempDirectory : IDisposable
{
    internal string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "xeyth-git", Guid.NewGuid().ToString("N"));

    internal TempDirectory()
    {
        Directory.CreateDirectory(Path);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
        catch
        {
            // Best-effort cleanup.
        }
    }
}
