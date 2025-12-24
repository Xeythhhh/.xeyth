using Spectre.Console;

namespace Automation.Cli.Common;

public static class StatusSpinner
{
    public static T Run<T>(IAnsiConsole console, string message, Func<T> action)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (!ConsoleEnvironment.IsInteractive(console))
        {
            WritePlaceholder(console, message);
            return action();
        }

        T? result = default;
        console.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(new Style(ColorScheme.Primary))
            .Start(message, _ => { result = action(); });

        return result!;
    }

    public static async Task<T> RunAsync<T>(IAnsiConsole console, string message, Func<Task<T>> action)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (!ConsoleEnvironment.IsInteractive(console))
        {
            WritePlaceholder(console, message);
            return await action();
        }

        T? result = default;
        await console.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(new Style(ColorScheme.Primary))
            .StartAsync(message, async _ => { result = await action(); });

        return result!;
    }

    public static async Task RunAsync(IAnsiConsole console, string message, Func<Task> action)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (!ConsoleEnvironment.IsInteractive(console))
        {
            WritePlaceholder(console, message);
            await action();
            return;
        }

        await console.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(new Style(ColorScheme.Primary))
            .StartAsync(message, async _ => { await action(); });
    }

    private static void WritePlaceholder(IAnsiConsole console, string message)
    {
        if (ConsoleEnvironment.ColorsEnabled(console))
        {
            console.MarkupLine($"[dim]{Markup.Escape(message)}[/]");
            return;
        }

        console.WriteLine(message);
    }
}
