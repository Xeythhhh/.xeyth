using System.Text;
using Xeyth.Common.IO.Paths;
using Automation.Planning.Models;
using Automation.Cli.Common;

namespace Automation.Planning.Services;

public sealed class TaskCreationService
{
    private const string DefaultTemplatePath = "Planning/Task.task.template";

    public string CreateTask(string rootPath, string requestedPath, Proposal proposal)
    {
        if (proposal is null)
        {
            throw new ArgumentNullException(nameof(proposal));
        }

        if (string.IsNullOrWhiteSpace(requestedPath))
        {
            throw new ArgumentException(ErrorMessages.RequiredValue("Task path", "Use --task to specify where to create the task file."));
        }

        var root = AbsolutePath.From(string.IsNullOrWhiteSpace(rootPath) ? Directory.GetCurrentDirectory() : rootPath);
        var targetPath = ResolveTaskPath(root, requestedPath);
        var templatePath = root.Combine(DefaultTemplatePath).Value;

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException(ErrorMessages.FileNotFound(
                templatePath,
                "Ensure the Planning/Task.task.template file exists in the repository."));
        }

        var targetDirectory = Path.GetDirectoryName(targetPath);
        if (string.IsNullOrWhiteSpace(targetDirectory))
        {
            throw new InvalidOperationException($"Could not resolve directory for task path: {targetPath}");
        }

        Directory.CreateDirectory(targetDirectory);

        if (File.Exists(targetPath))
        {
            throw new InvalidOperationException(ErrorMessages.AlreadyExists(
                "Task",
                targetPath,
                "Choose a different task name or path."));
        }

        var content = File.ReadAllText(templatePath);
        content = PopulateTemplate(content, proposal, root.Value, targetPath);

        File.WriteAllText(targetPath, content);
        return targetPath;
    }

    private static string ResolveTaskPath(AbsolutePath root, string requestedPath)
    {
        var normalized = requestedPath;
        if (!normalized.EndsWith(".task", StringComparison.OrdinalIgnoreCase))
        {
            normalized += ".task";
        }

        var absolute = Path.IsPathRooted(normalized)
            ? AbsolutePath.From(normalized)
            : root.Combine(normalized);

        if (!absolute.IsUnder(root))
        {
            throw new InvalidOperationException(ErrorMessages.PathMustBeWithinWorkspace(
                absolute.Value,
                root.Value,
                "Specify a task path within the repository root."));
        }

        return absolute.Value;
    }

    private static string PopulateTemplate(string template, Proposal proposal, string rootPath, string targetPath)
    {
        var builder = new StringBuilder(template);
        var taskName = Path.GetFileNameWithoutExtension(targetPath);
        var relativePath = Path.GetRelativePath(rootPath, targetPath).Replace('\\', '/');

        Replace(builder, "{TaskName}", taskName);
        Replace(builder, "{TaskPath}", relativePath);
        Replace(builder, "{One-line objective}", proposal.Title);
        Replace(builder, "{Detailed objective}", proposal.Title);

        var slice = ExtractSlice(relativePath);
        if (!string.IsNullOrWhiteSpace(slice))
        {
            Replace(builder, "{Slice}", slice);
            Replace(builder, "{Slice}/{TaskName}.task", relativePath);
        }

        return builder.ToString();
    }

    private static string? ExtractSlice(string relativePath)
    {
        var directory = Path.GetDirectoryName(relativePath)?.Replace('\\', '/');
        if (string.IsNullOrWhiteSpace(directory))
        {
            return null;
        }

        var segments = directory.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length > 0 ? segments[0] : null;
    }

    private static void Replace(StringBuilder builder, string token, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        builder.Replace(token, value);
    }
}
