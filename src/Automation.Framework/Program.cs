using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;
using Xeyth.Common.IO.Paths;
using Automation.Cli.Common;

internal static class Program
{
    private static readonly string[] DefaultExcludes =
    {
        "node_modules",
        "bin",
        "obj",
        ".git",
        "archive"
    };

    internal static int Main(string[] args)
    {
        try
        {
            return Run(args);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ERROR: {ex.Message}");
            return 1;
        }
    }

    private static int Run(string[] args)
    {
        if (args.Length == 0 || IsHelp(args[0]))
        {
            PrintHelp();
            return args.Length == 0 ? 1 : 0;
        }

        var command = args[0].ToLowerInvariant();
        var options = CliOptions.Parse(args.Skip(1));
        var workspacePath = ResolveWorkspacePath(options.WorkspacePath);
        var workspaceDirectory = AbsolutePath.From(Path.GetDirectoryName(workspacePath.Value) ?? Directory.GetCurrentDirectory());
        var rootPath = options.RootPath is null
            ? workspaceDirectory
            : AbsolutePath.From(options.RootPath);

        EnsureRootWithinWorkspace(rootPath, workspaceDirectory);

        var excludes = BuildExclusions(options.Excludes);

        var locations = DiscoverInstructionLocations(rootPath, excludes);

        return command switch
        {
            "discover" => PrintLocations(locations),
            "update-chat-locations" => UpdateWorkspace(workspacePath, locations),
            "validate-chat-locations" => ValidateWorkspace(workspacePath, locations),
            _ => UnknownCommand()
        };
    }

    internal static void EnsureRootWithinWorkspace(AbsolutePath rootPath, AbsolutePath workspaceDirectory)
    {
        if (!rootPath.IsUnder(workspaceDirectory))
        {
            throw new InvalidOperationException(ErrorMessages.PathMustBeWithinWorkspace(
                rootPath.Value,
                workspaceDirectory.Value,
                "Use --root to specify a path within the workspace directory."));
        }
    }

    private static bool IsHelp(string value) => value is "-h" or "--help" or "help";

    private static IReadOnlyList<string> DiscoverInstructionLocations(AbsolutePath rootPath, HashSet<string> excludes)
    {
        if (!Directory.Exists(rootPath.Value))
        {
            throw new DirectoryNotFoundException(ErrorMessages.DirectoryNotFound(
                rootPath.Value,
                "Use --root to specify a valid directory path."));
        }

        var locations = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in EnumerateInstructionFiles(rootPath))
        {
            if (ShouldExclude(file, rootPath, excludes))
            {
                continue;
            }

            var parent = Path.GetDirectoryName(file) ?? rootPath.Value;
            var relative = Path.GetRelativePath(rootPath.Value, parent);
            locations.Add(NormalizePath(relative));
        }

        return locations.ToList();
    }

    private static IEnumerable<string> EnumerateInstructionFiles(AbsolutePath root)
    {
        var patterns = new[] { "*.instructions.md", "copilot-instructions.md" };

        foreach (var pattern in patterns)
        {
            foreach (var file in Directory.EnumerateFiles(root.Value, pattern, SearchOption.AllDirectories))
            {
                yield return file;
            }
        }
    }

    private static int PrintLocations(IReadOnlyCollection<string> locations)
    {
        foreach (var location in locations)
        {
            Console.WriteLine(location);
        }

        return 0;
    }

    private static int UpdateWorkspace(AbsolutePath workspacePath, IReadOnlyCollection<string> discoveredLocations)
    {
        var workspace = LoadWorkspace(workspacePath);
        var settings = workspace["settings"] as JsonObject ?? new JsonObject();
        workspace["settings"] = settings;

        var locationsNode = new JsonArray();
        foreach (var location in discoveredLocations)
        {
            locationsNode.Add(location);
        }

        settings["chat.instructionsFiles.locations"] = locationsNode;

        CreateBackup(workspacePath);
        WriteWorkspace(workspacePath, workspace);

        Console.WriteLine($"Updated {workspacePath} with {discoveredLocations.Count} location(s).");
        return 0;
    }

    private static int ValidateWorkspace(AbsolutePath workspacePath, IReadOnlyCollection<string> discoveredLocations)
    {
        var workspace = LoadWorkspace(workspacePath);
        var settings = workspace["settings"] as JsonObject;
        var configured = ReadWorkspaceLocations(settings);

        var missing = discoveredLocations
            .Where(location => configured.All(existing => !PathEquals(existing, location)))
            .ToList();

        if (missing.Count == 0)
        {
            Console.WriteLine("All .instructions.md locations are covered by chat.instructionsFiles.locations.");
            return 0;
        }

        Console.Error.WriteLine("Missing chat.instructionsFiles.locations entries:");
        foreach (var entry in missing)
        {
            Console.Error.WriteLine($"- {entry}");
        }

        return 1;
    }

    private static JsonObject LoadWorkspace(AbsolutePath workspacePath)
    {
        if (!File.Exists(workspacePath.Value))
        {
            throw new FileNotFoundException(ErrorMessages.FileNotFound(
                workspacePath.Value,
                "Use --workspace to specify a valid .code-workspace file."));
        }

        var json = File.ReadAllText(workspacePath.Value);
        var node = JsonNode.Parse(json) as JsonObject;
        if (node is null)
        {
            throw new InvalidOperationException(ErrorMessages.InvalidJson(workspacePath.Value));
        }

        return node;
    }

    private static IReadOnlyList<string> ReadWorkspaceLocations(JsonObject? settings)
    {
        if (settings is null)
        {
            return Array.Empty<string>();
        }

        if (settings["chat.instructionsFiles.locations"] is not JsonArray array)
        {
            return Array.Empty<string>();
        }

        var results = new List<string>();
        foreach (var item in array)
        {
            if (item is JsonValue value && value.TryGetValue<string>(out var text))
            {
                results.Add(NormalizePath(text));
            }
        }

        return results;
    }

    private static void WriteWorkspace(AbsolutePath workspacePath, JsonObject workspace)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        File.WriteAllText(workspacePath.Value, workspace.ToJsonString(options));
    }

    private static void CreateBackup(AbsolutePath workspacePath)
    {
        var backupPath = workspacePath.Value + ".bak";
        File.Copy(workspacePath.Value, backupPath, overwrite: true);
    }

    private static AbsolutePath ResolveWorkspacePath(string? workspacePath)
    {
        var root = AbsolutePath.From(Directory.GetCurrentDirectory());
        if (!string.IsNullOrWhiteSpace(workspacePath))
        {
            var resolved = AbsolutePath.From(workspacePath);
            if (!File.Exists(resolved.Value))
            {
                throw new FileNotFoundException(ErrorMessages.FileNotFound(
                    resolved.Value,
                    "Check that the workspace file path is correct."));
            }

            if (!resolved.IsUnder(root))
            {
                throw new InvalidOperationException(ErrorMessages.PathMustBeWithinWorkspace(
                    resolved.Value,
                    root.Value,
                    "The workspace file must be within the current directory."));
            }

            return resolved;
        }

        var defaultPath = root.Combine(".xeyth.code-workspace");
        if (File.Exists(defaultPath.Value))
        {
            return defaultPath;
        }

        var firstWorkspacePath = Directory.EnumerateFiles(root.Value, "*.code-workspace", SearchOption.TopDirectoryOnly)
            .FirstOrDefault();

        if (firstWorkspacePath is null)
        {
            throw new FileNotFoundException(ErrorMessages.FileNotFound(
                "*.code-workspace",
                "Create a .code-workspace file or specify one with --workspace <path>."));
        }

        return AbsolutePath.From(firstWorkspacePath);
    }

    private static HashSet<string> BuildExclusions(IEnumerable<string> extras)
    {
        var excludes = new HashSet<string>(DefaultExcludes.Select(NormalizePath), StringComparer.OrdinalIgnoreCase);
        foreach (var extra in extras)
        {
            excludes.Add(NormalizePath(extra));
        }

        return excludes;
    }

    private static bool ShouldExclude(string path, AbsolutePath root, HashSet<string> excludes)
    {
        var relative = NormalizePath(Path.GetRelativePath(root.Value, path));
        return excludes.Any(exclude => relative.Equals(exclude, StringComparison.OrdinalIgnoreCase) || relative.StartsWith(exclude + "/", StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizePath(string path)
    {
        var normalized = path.Replace("\\", "/");
        return string.IsNullOrWhiteSpace(normalized) || normalized == "." ? "." : normalized;
    }

    private static bool PathEquals(string left, string right)
    {
        return string.Equals(NormalizePath(left), NormalizePath(right), StringComparison.OrdinalIgnoreCase);
    }

    private static void PrintHelp()
    {
        Console.WriteLine("xeyth-workspace commands:");
        Console.WriteLine("  discover [--root <path>] [--workspace <file>] [--exclude <dir>]");
        Console.WriteLine("      Lists instruction file locations relative to the root.");
        Console.WriteLine("  update-chat-locations [--root <path>] [--workspace <file>] [--exclude <dir>]");
        Console.WriteLine("      Updates chat.instructionsFiles.locations in the workspace file.");
        Console.WriteLine("  validate-chat-locations [--root <path>] [--workspace <file>] [--exclude <dir>]");
        Console.WriteLine("      Exits with code 1 if any .instructions.md location is missing from the workspace file.");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --root       Root directory to scan (defaults to workspace file directory)");
        Console.WriteLine("  --workspace  Path to the .code-workspace file (defaults to .xeyth.code-workspace or first *.code-workspace)");
        Console.WriteLine("  --exclude    Additional relative directories to exclude (can be repeated)");
        Console.WriteLine();
    }

    private static int UnknownCommand()
    {
        PrintHelp();
        return 1;
    }

    private sealed record CliOptions(string? RootPath, string? WorkspacePath, IReadOnlyList<string> Excludes)
    {
        public static CliOptions Parse(IEnumerable<string> args)
        {
            string? root = null;
            string? workspace = null;
            var excludes = new List<string>();

            var queue = new Queue<string>(args);
            while (queue.Count > 0)
            {
                var token = queue.Dequeue();
                switch (token)
                {
                    case "--root":
                        root = DequeueValue(queue, token);
                        break;
                    case "--workspace":
                        workspace = DequeueValue(queue, token);
                        break;
                    case "--exclude":
                        excludes.Add(DequeueValue(queue, token));
                        break;
                    default:
                        throw new ArgumentException(ErrorMessages.UnknownOption(
                            token,
                            new[] { "--root", "--workspace", "--exclude" }));
                }
            }

            return new CliOptions(root, workspace, excludes);
        }

        private static string DequeueValue(Queue<string> queue, string token)
        {
            if (queue.Count == 0)
            {
                var valueType = token switch
                {
                    "--root" => "path",
                    "--workspace" => "file",
                    "--exclude" => "directory",
                    _ => "value"
                };
                throw new ArgumentException(ErrorMessages.MissingValue(token, valueType));
            }

            return queue.Dequeue();
        }
    }
}
