using System.Diagnostics;

namespace GitTool;

internal interface IGitClient
{
    string? GetConfig(string key);
    string? GetRepositoryRoot();
    IReadOnlyList<string> GetContextFiles();
}

internal sealed class GitClient : IGitClient
{
    private static readonly string[] ContextExtensions =
    [
        ".task",
        ".plan",
        ".report",
        ".template",
        ".convention",
        ".inventory",
        ".proposal"
    ];

    private readonly IGitProcessRunner _runner;

    internal GitClient()
        : this(new GitProcessRunner())
    {
    }

    internal GitClient(IGitProcessRunner runner)
    {
        _runner = runner;
    }

    public string? GetConfig(string key)
    {
        return _runner.Run("config", "--get", key);
    }

    public string? GetRepositoryRoot()
    {
        return _runner.Run("rev-parse", "--show-toplevel");
    }

    public IReadOnlyList<string> GetContextFiles()
    {
        var staged = GetStagedFiles();
        return staged
            .Where(IsContextFile)
            .ToArray();
    }

    private IReadOnlyList<string> GetStagedFiles()
    {
        var output = _runner.Run("diff", "--cached", "--name-only");
        if (string.IsNullOrWhiteSpace(output))
        {
            return Array.Empty<string>();
        }

        return output
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => line.Length > 0)
            .ToArray();
    }

    private static bool IsContextFile(string path)
    {
        var extension = Path.GetExtension(path);
        return ContextExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

}

internal interface IGitProcessRunner
{
    string? Run(params string[] args);
}

internal sealed class GitProcessRunner : IGitProcessRunner
{
    private readonly IProcessFactory _processFactory;
    private readonly int _timeoutMilliseconds;

    private const int DefaultTimeoutMilliseconds = 5000;

    internal GitProcessRunner()
        : this(new ProcessFactory(), DefaultTimeoutMilliseconds)
    {
    }

    internal GitProcessRunner(IProcessFactory processFactory, int timeoutMilliseconds = DefaultTimeoutMilliseconds)
    {
        _processFactory = processFactory;
        _timeoutMilliseconds = timeoutMilliseconds;
    }

    public string? Run(params string[] args)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "git",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            foreach (var arg in args)
            {
                startInfo.ArgumentList.Add(arg);
            }

            using var process = _processFactory.Start(startInfo);
            if (process is null)
            {
                return null;
            }

            var output = process.StandardOutput.ReadToEnd().Trim();
            var exited = process.WaitForExit(_timeoutMilliseconds);
            if (!exited)
            {
                try
                {
                    process.Kill(entireProcessTree: true);
                }
                catch
                {
                    // Ignore failures when attempting to kill the process.
                }

                return null;
            }

            return process.ExitCode == 0 ? output : null;
        }
        catch
        {
            return null;
        }
    }
}

internal interface IProcessFactory
{
    IProcess? Start(ProcessStartInfo startInfo);
}

internal sealed class ProcessFactory : IProcessFactory
{
    public IProcess? Start(ProcessStartInfo startInfo)
    {
        var process = Process.Start(startInfo);
        return process is null ? null : new ProcessAdapter(process);
    }
}

internal interface IProcess : IDisposable
{
    StreamReader StandardOutput { get; }
    StreamReader StandardError { get; }
    bool WaitForExit(int milliseconds);
    void Kill(bool entireProcessTree);
    int ExitCode { get; }
}

internal sealed class ProcessAdapter : IProcess
{
    private readonly Process _process;

    internal ProcessAdapter(Process process)
    {
        _process = process;
    }

    public StreamReader StandardOutput => _process.StandardOutput;
    public StreamReader StandardError => _process.StandardError;
    public bool WaitForExit(int milliseconds) => _process.WaitForExit(milliseconds);
    public void Kill(bool entireProcessTree) => _process.Kill(entireProcessTree);
    public int ExitCode => _process.ExitCode;
    public void Dispose() => _process.Dispose();
}

internal static class CommitTemplateInjector
{
    internal static string? ResolveTemplatePath(string? configuredTemplate, string? repositoryRoot)
    {
        if (!string.IsNullOrWhiteSpace(configuredTemplate))
        {
            return MakeAbsolute(configuredTemplate, repositoryRoot);
        }

        if (!string.IsNullOrWhiteSpace(repositoryRoot))
        {
            var fallback = Path.Combine(repositoryRoot, ".gitmessage");
            if (File.Exists(fallback))
            {
                return fallback;
            }
        }

        return null;
    }

    internal static bool InjectIfEmpty(string commitMessagePath, string templatePath)
    {
        if (!File.Exists(commitMessagePath) || !File.Exists(templatePath))
        {
            return false;
        }

        var current = File.ReadAllText(commitMessagePath);
        if (!string.IsNullOrWhiteSpace(current))
        {
            return false;
        }

        var template = File.ReadAllText(templatePath);
        if (!template.EndsWith(Environment.NewLine, StringComparison.Ordinal))
        {
            template += Environment.NewLine;
        }

        File.WriteAllText(commitMessagePath, template);
        return true;
    }

    private static string MakeAbsolute(string path, string? repositoryRoot)
    {
        if (Path.IsPathRooted(path))
        {
            return path;
        }

        return string.IsNullOrWhiteSpace(repositoryRoot)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(Path.Combine(repositoryRoot, path));
    }
}
