using System;
using System.Threading.Tasks;
using Contracts.Cli;
using Spectre.Console.Testing;

namespace Contracts.Core.Tests.Cli;

public sealed class CliAppHeaderTests
{
    [Fact]
    public async Task RunAsync_WritesHeader_WhenShowingHelp()
    {
        var console = new TestConsole();
        console.Profile.Capabilities.Interactive = false;

        var originalNoColor = Environment.GetEnvironmentVariable("NO_COLOR");
        Environment.SetEnvironmentVariable("NO_COLOR", "1");

        try
        {
            var exitCode = await CliApp.RunAsync(new[] { "help" }, console);

            Assert.Equal(0, exitCode);
            Assert.Contains("xeyth-contracts", console.Output);
        }
        finally
        {
            Environment.SetEnvironmentVariable("NO_COLOR", originalNoColor);
        }
    }
}
