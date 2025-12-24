using Automation.Cli.Common;
using Spectre.Console.Testing;

namespace Automation.Cli.Common.Tests;

public sealed class StatusSpinnerTests
{
    [Fact]
    public void Run_ExecutesAction_WhenNonInteractive()
    {
        var console = new TestConsole();
        var originalCi = Environment.GetEnvironmentVariable("CI");
        Environment.SetEnvironmentVariable("CI", "true");
        try
        {
            var executed = false;

            var result = StatusSpinner.Run(console, "Validating...", () =>
            {
                executed = true;
                return 42;
            });

            Assert.True(executed);
            Assert.Equal(42, result);
        }
        finally
        {
            Environment.SetEnvironmentVariable("CI", originalCi);
        }
    }
}
