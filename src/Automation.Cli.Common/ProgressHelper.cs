using Spectre.Console;

namespace Automation.Cli.Common;

public static class ProgressHelper
{
    /// <summary>
    /// Executes an async action for each item with progress tracking.
    /// </summary>
    /// <param name="console">The console to render to.</param>
    /// <param name="description">The description shown in the progress bar (required for interactive mode).</param>
    /// <param name="items">The items to process.</param>
    /// <param name="action">The action to execute for each item.</param>
    /// <param name="itemMessageFormatter">Optional formatter for per-item messages in non-interactive mode.</param>
    public static async Task RunAsync<T>(
        IAnsiConsole console,
        string description,
        IEnumerable<T> items,
        Func<T, Task> action,
        Func<T, string>? itemMessageFormatter = null)
    {
        ValidateParameters(console, description, items, action);

        var itemsList = items.ToList();

        if (ShouldUseProgressBar(console, itemsList))
        {
            await RunWithProgressBarAsync(console, description, itemsList, action);
        }
        else
        {
            await RunWithOptionalSpinnerAsync(console, itemsList, action, itemMessageFormatter);
        }
    }

    /// <summary>
    /// Executes an async action for each item with progress tracking and returns results.
    /// </summary>
    /// <param name="console">The console to render to.</param>
    /// <param name="description">The description shown in the progress bar (required for interactive mode).</param>
    /// <param name="items">The items to process.</param>
    /// <param name="action">The action to execute for each item that returns a result.</param>
    /// <param name="itemMessageFormatter">Optional formatter for per-item messages in non-interactive mode.</param>
    public static async Task<IReadOnlyList<TResult>> RunAsync<T, TResult>(
        IAnsiConsole console,
        string description,
        IEnumerable<T> items,
        Func<T, Task<TResult>> action,
        Func<T, string>? itemMessageFormatter = null)
    {
        ValidateParameters(console, description, items, action);

        var itemsList = items.ToList();

        if (ShouldUseProgressBar(console, itemsList))
        {
            return await RunWithProgressBarAsync(console, description, itemsList, action);
        }
        else
        {
            return await RunWithOptionalSpinnerAsync(console, itemsList, action, itemMessageFormatter);
        }
    }

    private static void ValidateParameters<T>(
        IAnsiConsole console,
        string description,
        IEnumerable<T> items,
        object action)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException(ErrorMessages.RequiredValue("Description", "for progress display"));
        }

        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }
    }

    private static bool ShouldUseProgressBar<T>(IAnsiConsole console, IReadOnlyList<T> items)
    {
        return items.Count > 1 && ConsoleEnvironment.IsInteractive(console);
    }

    private static async Task RunWithProgressBarAsync<T>(
        IAnsiConsole console,
        string description,
        IReadOnlyList<T> items,
        Func<T, Task> action)
    {
        await console.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn())
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask(description, maxValue: items.Count);

                foreach (var item in items)
                {
                    await action(item);
                    task.Increment(1);
                }
            });
    }

    private static async Task<IReadOnlyList<TResult>> RunWithProgressBarAsync<T, TResult>(
        IAnsiConsole console,
        string description,
        IReadOnlyList<T> items,
        Func<T, Task<TResult>> action)
    {
        var results = new List<TResult>(items.Count);

        await console.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn())
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask(description, maxValue: items.Count);

                foreach (var item in items)
                {
                    var result = await action(item);
                    results.Add(result);
                    task.Increment(1);
                }
            });

        return results;
    }

    private static async Task RunWithOptionalSpinnerAsync<T>(
        IAnsiConsole console,
        IReadOnlyList<T> items,
        Func<T, Task> action,
        Func<T, string>? itemMessageFormatter)
    {
        if (itemMessageFormatter is not null)
        {
            foreach (var item in items)
            {
                await StatusSpinner.RunAsync(console, itemMessageFormatter(item), () => action(item));
            }
        }
        else
        {
            foreach (var item in items)
            {
                await action(item);
            }
        }
    }

    private static async Task<IReadOnlyList<TResult>> RunWithOptionalSpinnerAsync<T, TResult>(
        IAnsiConsole console,
        IReadOnlyList<T> items,
        Func<T, Task<TResult>> action,
        Func<T, string>? itemMessageFormatter)
    {
        var results = new List<TResult>(items.Count);

        if (itemMessageFormatter is not null)
        {
            foreach (var item in items)
            {
                var result = await StatusSpinner.RunAsync(console, itemMessageFormatter(item), () => action(item));
                results.Add(result);
            }
        }
        else
        {
            foreach (var item in items)
            {
                var result = await action(item);
                results.Add(result);
            }
        }

        return results;
    }
}
