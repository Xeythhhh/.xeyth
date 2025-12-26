using Automation.Cli.Common;
using Spectre.Console.Testing;

namespace Automation.Cli.Common.Tests;

public sealed class ProgressHelperTests
{
    [Fact]
    public async Task RunAsync_ExecutesAction_WhenNonInteractive()
    {
        var console = new TestConsole();
        var originalCi = Environment.GetEnvironmentVariable("CI");
        Environment.SetEnvironmentVariable("CI", "true");
        try
        {
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
        finally
        {
            Environment.SetEnvironmentVariable("CI", originalCi);
        }
    }

    [Fact]
    public async Task RunAsync_WithResults_ReturnsResults_WhenNonInteractive()
    {
        var console = new TestConsole();
        var originalCi = Environment.GetEnvironmentVariable("CI");
        Environment.SetEnvironmentVariable("CI", "true");
        try
        {
            var items = new[] { 1, 2, 3 };

            var results = await ProgressHelper.RunAsync(
                console,
                "Processing...",
                items,
                async item => await Task.FromResult(item * 2));

            Assert.Equal(new[] { 2, 4, 6 }, results);
        }
        finally
        {
            Environment.SetEnvironmentVariable("CI", originalCi);
        }
    }

    [Fact]
    public async Task RunAsync_WithItemFormatter_ShowsMessages_WhenNonInteractive()
    {
        var console = new TestConsole();
        var originalCi = Environment.GetEnvironmentVariable("CI");
        Environment.SetEnvironmentVariable("CI", "true");
        try
        {
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
        finally
        {
            Environment.SetEnvironmentVariable("CI", originalCi);
        }
    }

    [Fact]
    public async Task RunAsync_WithResults_AndItemFormatter_ShowsMessages_WhenNonInteractive()
    {
        var console = new TestConsole();
        var originalCi = Environment.GetEnvironmentVariable("CI");
        Environment.SetEnvironmentVariable("CI", "true");
        try
        {
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
        finally
        {
            Environment.SetEnvironmentVariable("CI", originalCi);
        }
    }

    [Fact]
    public async Task RunAsync_ExecutesAction_WhenSingleItem()
    {
        var console = new TestConsole();
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
        // Single item should not show progress bar, just execute
    }

    [Fact]
    public async Task RunAsync_WithResults_ExecutesAction_WhenSingleItem()
    {
        var console = new TestConsole();
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
        var console = new TestConsole();

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
        var console = new TestConsole();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await ProgressHelper.RunAsync(
                console,
                "Processing...",
                new[] { 1, 2, 3 },
                null!));
    }
}
