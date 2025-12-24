using Spectre.Console;

namespace Automation.Cli.Common;

public static class ConsoleEnvironment
{
    private const string CiVariable = "CI";
    private const string NoColorVariable = "NO_COLOR";
    private const string DisableFigletVariable = "XEYTH_NO_FIGLET";

    public static bool IsCi => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(CiVariable));

    public static bool NoColorRequested => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(NoColorVariable));

    public static bool FigletAllowed => !NoColorRequested && !IsCi && Environment.GetEnvironmentVariable(DisableFigletVariable) is null;

    public static bool IsInteractiveEnvironment => !Console.IsOutputRedirected && !IsCi;

    public static bool IsInteractive(IAnsiConsole console)
    {
        return IsInteractiveEnvironment && console.Profile.Capabilities.Interactive;
    }

    public static bool ColorsEnabled(IAnsiConsole console)
    {
        return !NoColorRequested && console.Profile.Capabilities.Ansi;
    }

    public static IAnsiConsole CreateConsole()
    {
        var settings = new AnsiConsoleSettings
        {
            Ansi = NoColorRequested ? AnsiSupport.No : AnsiSupport.Detect,
            ColorSystem = NoColorRequested ? ColorSystemSupport.NoColors : ColorSystemSupport.Standard,
            Interactive = IsInteractiveEnvironment ? InteractionSupport.Yes : InteractionSupport.No
        };

        return AnsiConsole.Create(settings);
    }
}
