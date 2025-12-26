using Spectre.Console;

namespace Automation.Planning.Commands;

internal interface IPlanningCommand
{
    string Name { get; }
    string Description { get; }
    Task<int> ExecuteAsync(string[] args, CancellationToken cancellationToken);
    void WriteUsage(IAnsiConsole console);
}
