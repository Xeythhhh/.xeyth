using Spectre.Console;
using Spectre.Console.Rendering;

namespace Automation.Cli.Common;

/// <summary>
/// Helper for creating live-updating displays with environment awareness.
/// </summary>
public static class LiveDisplayHelper
{
    /// <summary>
    /// Runs an action with a live-updating display in interactive mode,
    /// or falls back to static output in non-interactive mode.
    /// </summary>
    /// <param name="console">The console to render to.</param>
    /// <param name="initialRenderable">The initial content to display.</param>
    /// <param name="action">The action to execute that updates the display.</param>
    public static async Task RunAsync(
        IAnsiConsole console,
        IRenderable initialRenderable,
        Func<LiveDisplayContext, Task> action)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (initialRenderable is null)
        {
            throw new ArgumentNullException(nameof(initialRenderable));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (!ConsoleEnvironment.IsInteractive(console))
        {
            // In non-interactive mode, just run the action without live updates
            var context = new NonInteractiveLiveDisplayContext(console);
            await action(context);
            return;
        }

        await console.Live(initialRenderable)
            .StartAsync(async ctx => await action(new InteractiveLiveDisplayContext(ctx)));
    }

    /// <summary>
    /// Runs a synchronous action with a live-updating display in interactive mode,
    /// or falls back to static output in non-interactive mode.
    /// </summary>
    /// <param name="console">The console to render to.</param>
    /// <param name="initialRenderable">The initial content to display.</param>
    /// <param name="action">The action to execute that updates the display.</param>
    public static void Run(
        IAnsiConsole console,
        IRenderable initialRenderable,
        Action<LiveDisplayContext> action)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        if (initialRenderable is null)
        {
            throw new ArgumentNullException(nameof(initialRenderable));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (!ConsoleEnvironment.IsInteractive(console))
        {
            // In non-interactive mode, just run the action without live updates
            var context = new NonInteractiveLiveDisplayContext(console);
            action(context);
            return;
        }

        console.Live(initialRenderable)
            .Start(ctx => action(new InteractiveLiveDisplayContext(ctx)));
    }
}

/// <summary>
/// Abstraction for live display context that works in both interactive and non-interactive modes.
/// </summary>
public abstract class LiveDisplayContext
{
    /// <summary>
    /// Updates the display with new content.
    /// </summary>
    /// <param name="renderable">The new content to display.</param>
    public abstract void Update(IRenderable renderable);

    /// <summary>
    /// Gets whether the display supports live updates.
    /// </summary>
    public abstract bool IsLive { get; }
}

internal sealed class InteractiveLiveDisplayContext : LiveDisplayContext
{
    private readonly Spectre.Console.LiveDisplayContext _context;

    internal InteractiveLiveDisplayContext(Spectre.Console.LiveDisplayContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public override void Update(IRenderable renderable)
    {
        _context.UpdateTarget(renderable);
    }

    public override bool IsLive => true;
}

internal sealed class NonInteractiveLiveDisplayContext : LiveDisplayContext
{
    private readonly IAnsiConsole _console;

    internal NonInteractiveLiveDisplayContext(IAnsiConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public override void Update(IRenderable renderable)
    {
        // In non-interactive mode, write each update as a new line
        _console.Write(renderable);
        _console.WriteLine();
    }

    public override bool IsLive => false;
}
