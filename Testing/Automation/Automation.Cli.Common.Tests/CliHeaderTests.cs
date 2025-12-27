using Automation.Cli.Common;
using Spectre.Console.Testing;

namespace Automation.Cli.Common.Tests;

public sealed class CliHeaderTests
{
    [Fact]
    public void Render_WritesToolNameAndVersion()
    {
        var console = new TestConsole();
        var header = new CliHeader("xeyth-verify", "1.2.3", "DiffEngine configuration helper");

        header.Render(console);

        var output = console.Output;
        Assert.Contains("xeyth-verify", output);
        Assert.Contains("1.2.3", output);
    }
}
