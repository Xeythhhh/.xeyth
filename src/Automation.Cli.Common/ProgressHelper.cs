using Spectre.Console;

namespace Automation.Cli.Common;

public static class ProgressHelper
{
    public static async Task RunAsync<T>(
        IAnsiConsole console,
        string description,
        IEnumerable<T> items,
        Func<T, Task> action,
        Func<T, string>? itemMessageFormatter = null)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var itemsList = items.ToList();

        // For single items or non-interactive, use StatusSpinner if message formatter provided
        if (itemsList.Count <= 1 || !ConsoleEnvironment.IsInteractive(console))
        {
            if (itemMessageFormatter is not null)
            {
                foreach (var item in itemsList)
                {
                    await StatusSpinner.RunAsync(console, itemMessageFormatter(item), () => action(item));
                }
            }
            else
            {
                foreach (var item in itemsList)
                {
                    await action(item);
                }
            }
            return;
        }

        // Interactive with multiple items: show progress bar
        await console.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn())
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask(description, maxValue: itemsList.Count);

                foreach (var item in itemsList)
                {
                    await action(item);
                    task.Increment(1);
                }
            });
    }

    public static async Task<IReadOnlyList<TResult>> RunAsync<T, TResult>(
        IAnsiConsole console,
        string description,
        IEnumerable<T> items,
        Func<T, Task<TResult>> action,
        Func<T, string>? itemMessageFormatter = null)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var itemsList = items.ToList();
        var results = new List<TResult>(itemsList.Count);

        // For single items or non-interactive, use StatusSpinner if message formatter provided
        if (itemsList.Count <= 1 || !ConsoleEnvironment.IsInteractive(console))
        {
            if (itemMessageFormatter is not null)
            {
                foreach (var item in itemsList)
                {
                    var result = await StatusSpinner.RunAsync(console, itemMessageFormatter(item), () => action(item));
                    results.Add(result);
                }
            }
            else
            {
                foreach (var item in itemsList)
                {
                    var result = await action(item);
                    results.Add(result);
                }
            }
            return results;
        }

        // Interactive with multiple items: show progress bar
        await console.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn())
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask(description, maxValue: itemsList.Count);

                foreach (var item in itemsList)
                {
                    var result = await action(item);
                    results.Add(result);
                    task.Increment(1);
                }
            });

        return results;
    }
}
