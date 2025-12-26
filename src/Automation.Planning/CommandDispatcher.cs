using Automation.Planning.Commands;
using Spectre.Console;

namespace Automation.Planning;

internal sealed class CommandDispatcher
{
    private readonly IAnsiConsole _console;
    private readonly PlanningReporter _reporter;
    private readonly Dictionary<string, IPlanningCommand> _commands;

    internal CommandDispatcher(IAnsiConsole console, PlanningReporter reporter, IEnumerable<IPlanningCommand> commands)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
        _commands = commands?.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase)
            ?? throw new ArgumentNullException(nameof(commands));
    }

    internal Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken = default)
    {
        if (args.Length == 0 || IsHelp(args[0]))
        {
            _reporter.Help(_commands.Values.Select(c => (c.Name, c.Description)));
            return Task.FromResult(0);
        }

        var commandName = args[0];
        if (!_commands.TryGetValue(commandName, out var command))
        {
            _reporter.Error($"Unknown command: {commandName}");
            _reporter.Help(_commands.Values.Select(c => (c.Name, c.Description)));
            return Task.FromResult(1);
        }

        try
        {
            return command.ExecuteAsync(args.Skip(1).ToArray(), cancellationToken);
        }
        catch (ArgumentException ex)
        {
            _reporter.Error(ex.Message);
            _reporter.Info($"Use '{command.Name} --help' for usage.");
            return Task.FromResult(2);
        }
    }

    private static bool IsHelp(string token) => token is "--help" or "-h" or "help";
}
