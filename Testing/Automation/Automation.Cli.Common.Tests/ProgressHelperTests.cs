using Automation.Cli.Common;
using Spectre.Console.Testing;

namespace Automation.Cli.Common.Tests;

public sealed class ProgressHelperTests
{
    private sealed class CiEnvironmentScope : IDisposable
    {
        private static readonly object CiLock = new();
        private readonly string? _originalValue;
        private bool _lockTaken;

        public CiEnvironmentScope()
        {
            Monitor.Enter(CiLock, ref _lockTaken);
            _originalValue = Environment.GetEnvironmentVariable("CI");
            Environment.SetEnvironmentVariable("CI", "true");
        }

        public void Dispose()
        {
            try
            {
                Environment.SetEnvironmentVariable("CI", _originalValue);
            }
            finally
            {
                if (_lockTaken)
                {
                    Monitor.Exit(CiLock);
                    _lockTaken = false;
                }
            }
        }
    }

    [Fact]
    public async Task RunAsync_ExecutesAction_WhenNonInteractive()
    {
        using var console = new TestConsole();
        using var ciScope = new CiEnvironmentScope();

        var executedItems = new List<int>();
        var items = new[] { 1, 2, 3 };

        await ProgressHelper.RunAsync(
            console,
            "Processing...",
            items,
            async item =>
            {
                executedItems.Add(item);
                await Task.CompletedTask;
            });

        Assert.Equal(items, executedItems);
    }

    [Fact]
    public async Task RunAsync_WithResults_ReturnsResults_WhenNonInteractive()
    {
        using var console = new TestConsole();
        using var ciScope = new CiEnvironmentScope();

        var items = new[] { 1, 2, 3 };

        var results = await ProgressHelper.RunAsync(
            console,
            "Processing...",
            items,
            async item => await Task.FromResult(item * 2));

        Assert.Equal(new[] { 2, 4, 6 }, results);
    }

    [Fact]
    public async Task RunAsync_WithItemFormatter_ShowsMessages_WhenNonInteractive()
    {
        using var console = new TestConsole();
        using var ciScope = new CiEnvironmentScope();

        var items = new[] { 1, 2, 3 };

        await ProgressHelper.RunAsync(
            console,
            "Processing...",
            items,
            async item => await Task.CompletedTask,
            item => $"Processing item {item}...");

        var output = console.Output;
        Assert.Contains("Processing item 1...", output);
        Assert.Contains("Processing item 2...", output);
        Assert.Contains("Processing item 3...", output);
    }

    [Fact]
    public async Task RunAsync_WithResults_AndItemFormatter_ShowsMessages_WhenNonInteractive()
    {
        using var console = new TestConsole();
        using var ciScope = new CiEnvironmentScope();

        var items = new[] { 1, 2, 3 };

        var results = await ProgressHelper.RunAsync(
            console,
            "Processing...",
            items,
            async item => await Task.FromResult(item * 2),
            item => $"Processing item {item}...");

        Assert.Equal(new[] { 2, 4, 6 }, results);

        var output = console.Output;
        Assert.Contains("Processing item 1...", output);
        Assert.Contains("Processing item 2...", output);
        Assert.Contains("Processing item 3...", output);
    }

    [Fact]
    public async Task RunAsync_ExecutesAction_WhenSingleItem()
    {
        using var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;

        var executedItems = new List<int>();
        var items = new[] { 42 };

        await ProgressHelper.RunAsync(
            console,
            "Processing...",
            items,
            async item =>
            {
                executedItems.Add(item);
                await Task.CompletedTask;
            });

        Assert.Equal(items, executedItems);
    }

    [Fact]
    public async Task RunAsync_WithResults_ExecutesAction_WhenSingleItem()
    {
        using var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;

        var items = new[] { 42 };

        var results = await ProgressHelper.RunAsync(
            console,
            "Processing...",
            items,
            async item => await Task.FromResult(item * 2));

        Assert.Equal(new[] { 84 }, results);
    }

    [Fact]
    public async Task RunAsync_ShowsProgressBar_WhenInteractiveWithMultipleItems()
    {
        using var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;

        var executedItems = new List<int>();
        var items = new[] { 1, 2, 3, 4, 5 };

        await ProgressHelper.RunAsync(
            console,
            "Processing items",
            items,
            async item =>
            {
                executedItems.Add(item);
                await Task.CompletedTask;
            });

        // Verify all items were executed in order
        Assert.Equal(items, executedItems);
    }

    [Fact]
    public async Task RunAsync_WithResults_ShowsProgressBar_WhenInteractiveWithMultipleItems()
    {
        using var console = new TestConsole();
        console.Profile.Capabilities.Interactive = true;

        var items = new[] { 1, 2, 3, 4, 5 };

        var results = await ProgressHelper.RunAsync(
            console,
            "Processing items",
            items,
            async item => await Task.FromResult(item * 2));

        // Verify all results were collected correctly
        Assert.Equal(new[] { 2, 4, 6, 8, 10 }, results);
    }

    [Fact]
    public async Task RunAsync_ThrowsArgumentNullException_WhenConsoleIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await ProgressHelper.RunAsync<int>(
                null!,
                "Processing...",
                new[] { 1, 2, 3 },
                async _ => await Task.CompletedTask));
    }

    [Fact]
    public async Task RunAsync_ThrowsArgumentNullException_WhenItemsIsNull()
    {
        using var console = new TestConsole();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await ProgressHelper.RunAsync<int>(
                console,
                "Processing...",
                null!,
                async _ => await Task.CompletedTask));
    }

    [Fact]
    public async Task RunAsync_ThrowsArgumentNullException_WhenActionIsNull()
    {
        using var console = new TestConsole();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await ProgressHelper.RunAsync(
                console,
                "Processing...",
                new[] { 1, 2, 3 },
                null!));
    }

    [Fact]
    public async Task RunAsync_ThrowsArgumentException_WhenDescriptionIsNull()
    {
        using var console = new TestConsole();

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await ProgressHelper.RunAsync(
                console,
                null!,
                new[] { 1, 2, 3 },
                async _ => await Task.CompletedTask));
    }

    [Fact]
    public async Task RunAsync_ThrowsArgumentException_WhenDescriptionIsEmpty()
    {
        using var console = new TestConsole();

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await ProgressHelper.RunAsync(
                console,
                "",
                new[] { 1, 2, 3 },
                async _ => await Task.CompletedTask));
    }
}
