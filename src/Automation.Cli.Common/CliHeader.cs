using Spectre.Console;
using Spectre.Console.Rendering;

namespace Automation.Cli.Common;

public sealed class CliHeader
{
    private readonly string _toolName;
    private readonly string? _version;
    private readonly string? _subtitle;
    private readonly string? _icon;

    public CliHeader(string toolName, string? version = null, string? subtitle = null, string? icon = null)
    {
        _toolName = string.IsNullOrWhiteSpace(toolName)
            ? throw new ArgumentException(ErrorMessages.RequiredValue("Tool name"))
            : toolName;
        _version = version;
        _subtitle = subtitle;
        _icon = icon;
    }

    public void Render(IAnsiConsole console)
    {
        var content = BuildContent();
        var headerText = string.IsNullOrWhiteSpace(_icon) ? _toolName : $"{_icon} {_toolName}";

        var panel = new Panel(content)
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(ColorScheme.Primary))
            .Header($"[bold]{Markup.Escape(headerText)}[/]");

        console.Write(panel);
        console.WriteLine();
    }

    private IRenderable BuildContent()
    {
        var renderables = new List<IRenderable>();

        if (ConsoleEnvironment.FigletAllowed && ConsoleEnvironment.IsInteractiveEnvironment)
        {
            renderables.Add(new FigletText(_toolName).Centered().Color(ColorScheme.Primary));
        }
        else
        {
            var name = new Markup($"[bold]{Markup.Escape(_toolName)}[/]");
            renderables.Add(new Align(name, HorizontalAlignment.Center));
        }

        if (!string.IsNullOrWhiteSpace(_version))
        {
            var version = new Markup($"[dim]v{Markup.Escape(_version)}[/]");
            renderables.Add(new Align(version, HorizontalAlignment.Center));
        }

        if (!string.IsNullOrWhiteSpace(_subtitle))
        {
            var subtitle = new Markup(Markup.Escape(_subtitle));
            renderables.Add(new Align(subtitle, HorizontalAlignment.Center));
        }

        return new Rows(renderables);
    }
}