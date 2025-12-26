using System.Diagnostics;

namespace Automation.Git;

internal sealed class GitClient
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

    internal string? GetConfig(string key)
    {
        return TryRun("config", "--get", key);
    }

    internal string? GetRepositoryRoot()
    {
        return TryRun("rev-parse", "--show-toplevel");
    }

    internal IReadOnlyList<string> GetContextFiles()
    {
        var staged = GetStagedFiles();
        return staged
            .Where(IsContextFile)
            .ToArray();
    }

    private IReadOnlyList<string> GetStagedFiles()
    {
        var output = TryRun("diff", "--cached", "--name-only");
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

    private static string? TryRun(params string[] args)
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

            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return null;
            }

            var output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit(2000);

            return process.ExitCode == 0 ? output : null;
        }
        catch
        {
            return null;
        }
    }
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
