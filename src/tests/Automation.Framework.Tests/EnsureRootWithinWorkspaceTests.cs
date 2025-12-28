using Xeyth.Common.IO.Paths;
using Automation.Framework;

namespace Automation.Framework.Tests;

public sealed class EnsureRootWithinWorkspaceTests
{
    [Fact]
    public void Allows_WorkspaceDirectory_AsRoot()
    {
        var root = AbsolutePath.From(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
        Directory.CreateDirectory(root.Value);

        Program.EnsureRootWithinWorkspace(root, root);
    }

    [Fact]
    public void Allows_RootUnderWorkspace()
    {
        var workspace = AbsolutePath.From(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
        var child = workspace.Combine("child");
        Directory.CreateDirectory(child.Value);

        Program.EnsureRootWithinWorkspace(child, workspace);
    }

    [Fact]
    public void Rejects_RootOutsideWorkspace()
    {
        var workspace = AbsolutePath.From(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "workspace"));
        var outside = AbsolutePath.From(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "outside"));

        Directory.CreateDirectory(workspace.Value);
        Directory.CreateDirectory(outside.Value);

        Assert.Throws<InvalidOperationException>(() => Program.EnsureRootWithinWorkspace(outside, workspace));
    }
}
