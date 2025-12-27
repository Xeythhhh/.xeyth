using Automation.Verify;

namespace Automation.Verify.Tests;

public sealed class ResolveTargetDirectoryTests : IDisposable
{
    private readonly string _workspaceRoot;
    private readonly string _originalCwd;

    public ResolveTargetDirectoryTests()
    {
        _originalCwd = Directory.GetCurrentDirectory();
        var workspaceRoot = Path.Combine(Path.GetTempPath(), "xeyth-verify-workspace-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workspaceRoot);
        Directory.SetCurrentDirectory(workspaceRoot);
        _workspaceRoot = Directory.GetCurrentDirectory();
    }

    [Fact]
    public void Uses_CurrentDirectory_WhenNotSpecified()
    {
        var resolved = CommandDispatcher.ResolveTargetDirectory(null);

        Assert.Equal(_workspaceRoot, resolved.Value);
    }

    [Fact]
    public void Accepts_Path_Under_Workspace()
    {
        var child = Path.Combine(_workspaceRoot, "child");
        Directory.CreateDirectory(child);

        var resolved = CommandDispatcher.ResolveTargetDirectory(child);

        Assert.Equal(child, resolved.Value);
    }

    [Fact]
    public void Rejects_Path_Outside_Workspace()
    {
        var outside = Path.Combine(Path.GetTempPath(), "xeyth-verify-outside-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outside);

        Assert.Throws<InvalidOperationException>(() => CommandDispatcher.ResolveTargetDirectory(outside));
    }

    [Fact]
    public void Rejects_Relative_Path_Escaping_Workspace()
    {
        var relativeOutside = Path.Combine("..", "outside-" + Guid.NewGuid().ToString("N"));

        Assert.Throws<InvalidOperationException>(() => CommandDispatcher.ResolveTargetDirectory(relativeOutside));
    }

    public void Dispose()
    {
        Directory.SetCurrentDirectory(_originalCwd);
        try
        {
            if (Directory.Exists(_workspaceRoot))
            {
                Directory.Delete(_workspaceRoot, recursive: true);
            }
        }
        catch
        {
            // Best-effort cleanup.
        }
    }
}
