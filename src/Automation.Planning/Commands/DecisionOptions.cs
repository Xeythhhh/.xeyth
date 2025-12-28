using Automation.Cli.Common;

namespace Automation.Planning.Commands;

internal sealed record DecisionOptions(string Name, string RootPath, string Reason)
{
    internal static DecisionOptions Parse(string[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException(ErrorMessages.MissingRequiredArgument(
                "proposal name",
                "xeyth-planning <command> <name> --reason <text>"));
        }

        var name = args[0];
        var queue = new Queue<string>(args.Skip(1));
        var root = Directory.GetCurrentDirectory();
        string? reason = null;

        while (queue.Count > 0)
        {
            var token = queue.Dequeue();
            switch (token)
            {
                case "--reason":
                case "-r":
                    PlanningCommandBase.EnsureHasValue(queue, token);
                    reason = queue.Dequeue();
                    break;

                case "--root":
                    PlanningCommandBase.EnsureHasValue(queue, token);
                    root = queue.Dequeue();
                    break;

                default:
                    throw new ArgumentException(ErrorMessages.UnknownOption(
                        token,
                        new[] { "--reason", "-r", "--root" }));
            }
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException(ErrorMessages.MissingRequiredArgument(
                "--reason",
                "xeyth-planning <command> <name> --reason \"explanation\""));
        }

        return new DecisionOptions(name, Path.GetFullPath(root), reason);
    }
}
