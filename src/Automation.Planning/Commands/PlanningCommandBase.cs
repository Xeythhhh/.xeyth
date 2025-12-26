using Spectre.Console;

namespace Automation.Planning.Commands;

internal abstract class PlanningCommandBase : IPlanningCommand
{
    protected PlanningCommandBase(IAnsiConsole console, PlanningReporter reporter)
    {
        Console = console ?? throw new ArgumentNullException(nameof(console));
        Reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
    }

    protected IAnsiConsole Console { get; }
    protected PlanningReporter Reporter { get; }

    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken);
    public abstract void WriteUsage(IAnsiConsole console);

    protected static bool ShouldShowHelp(string[] args)
    {
        return args.Any(a => a is "--help" or "-h" or "help");
    }

    internal static void EnsureHasValue(Queue<string> queue, string token)
    {
        if (queue.Count == 0)
        {
            throw new ArgumentException($"Missing value for {token}");
        }
    }

    protected internal static string NormalizePath(string path)
    {
        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        return path.Replace('\\', '/');
    }
}
