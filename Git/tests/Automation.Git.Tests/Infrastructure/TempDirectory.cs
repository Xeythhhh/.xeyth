namespace Automation.Git.Tests.Infrastructure;

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
