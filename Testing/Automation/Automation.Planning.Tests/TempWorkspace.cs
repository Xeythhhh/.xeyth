namespace Automation.Planning.Tests;

internal sealed class TempWorkspace : IDisposable
{
    internal string Root { get; } = Path.Combine(Path.GetTempPath(), "xeyth-planning", Guid.NewGuid().ToString("N"));

    internal TempWorkspace()
    {
        Directory.CreateDirectory(Root);
    }

    internal string WriteFile(string relativePath, string content)
    {
        var fullPath = Path.Combine(Root, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        File.WriteAllText(fullPath, content);
        return fullPath;
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, recursive: true);
            }
        }
        catch
        {
            // Best-effort cleanup.
        }
    }
}
