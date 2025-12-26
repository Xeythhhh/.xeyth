using System.Text;
using Contracts.Core.Models;

namespace Contracts.Core.Rendering;

/// <summary>
/// Renders contracts as formatted card-style boxes.
/// </summary>
public sealed class CardRenderer : IContractRenderer
{
    private const int DefaultWidth = 80;
    private const string TopLeft = "╭";
    private const string TopRight = "╮";
    private const string BottomLeft = "╰";
    private const string BottomRight = "╯";
    private const string Horizontal = "─";
    private const string Vertical = "│";
    private const string VerticalRight = "├";
    private const string VerticalLeft = "┤";

    public void Render(RenderContext context)
    {
        var config = context.Configuration ?? new RendererConfiguration();
        var width = config.MaxWidth ?? DefaultWidth;
        var contract = context.Contract;

        RenderCard(context.Output, contract, width, config, context.RootPath);
    }

    private static void RenderCard(TextWriter output, ContractMetadata contract, int width, RendererConfiguration config, string? rootPath)
    {
        var innerWidth = width - 4; // Account for borders and padding

        // Top border
        output.WriteLine($"{TopLeft}{new string(Horizontal[0], width - 2)}{TopRight}");

        // Title
        var title = GetContractTitle(contract);
        RenderLine(output, title, innerWidth);

        // Type
        var type = GetContractType(contract);
        if (!string.IsNullOrWhiteSpace(type))
        {
            RenderLine(output, $"Type: {type}", innerWidth);
        }

        // Separator
        output.WriteLine($"{VerticalRight}{new string(Horizontal[0], width - 2)}{VerticalLeft}");

        // Target patterns
        if (contract.Target?.Patterns is { Count: > 0 })
        {
            RenderLine(output, $"📋 Target Pattern: {string.Join(", ", contract.Target.Patterns)}", innerWidth);
        }

        // Schema sections
        if (contract.Schema?.RequiredSections is { Count: > 0 })
        {
            RenderLine(output, $"📝 Required Sections: {contract.Schema.RequiredSections.Count}", innerWidth);
            foreach (var section in contract.Schema.RequiredSections)
            {
                RenderLine(output, $"    • {section.Name}", innerWidth);
            }
        }

        // Naming convention
        if (contract.Naming?.Pattern is not null)
        {
            RenderLine(output, $"✓ Naming: {contract.Naming.Pattern}", innerWidth);
        }

        // Archiving rules
        if (contract.Archiving is not null)
        {
            RenderLine(output, $"📁 Archive: {contract.Archiving.Directory}/{contract.Archiving.Pattern}", innerWidth);
        }

        // Related files
        if (contract.RelatedFiles is { Count: > 0 })
        {
            RenderLine(output, $"🔗 Related Files: {contract.RelatedFiles.Count}", innerWidth);
            if (config.Verbose)
            {
                foreach (var relatedFile in contract.RelatedFiles)
                {
                    var required = relatedFile.Required ? "required" : "optional";
                    RenderLine(output, $"    • {relatedFile.Pattern} ({required})", innerWidth);
                }
            }
        }

        // Source path
        if (config.Verbose && !string.IsNullOrWhiteSpace(contract.SourcePath))
        {
            var sourcePath = contract.SourcePath;
            if (!string.IsNullOrWhiteSpace(rootPath))
            {
                sourcePath = Path.GetRelativePath(rootPath, contract.SourcePath);
            }
            RenderLine(output, $"Source: {sourcePath}", innerWidth);
        }

        // Bottom border
        output.WriteLine($"{BottomLeft}{new string(Horizontal[0], width - 2)}{BottomRight}");
    }

    private static void RenderLine(TextWriter output, string content, int maxWidth)
    {
        var lines = WrapText(content, maxWidth);
        foreach (var line in lines)
        {
            var padded = line.PadRight(maxWidth);
            output.WriteLine($"{Vertical} {padded} {Vertical}");
        }
    }

    private static IEnumerable<string> WrapText(string text, int maxWidth)
    {
        if (text.Length <= maxWidth)
        {
            yield return text;
            yield break;
        }

        var words = text.Split(' ');
        var currentLine = new StringBuilder();

        foreach (var word in words)
        {
            if (currentLine.Length + word.Length + 1 > maxWidth)
            {
                if (currentLine.Length > 0)
                {
                    yield return currentLine.ToString();
                    currentLine.Clear();
                }

                // Handle words longer than maxWidth
                if (word.Length > maxWidth)
                {
                    yield return word[..maxWidth];
                    var remaining = word[maxWidth..];
                    while (remaining.Length > maxWidth)
                    {
                        yield return remaining[..maxWidth];
                        remaining = remaining[maxWidth..];
                    }
                    if (remaining.Length > 0)
                    {
                        currentLine.Append(remaining);
                    }
                }
                else
                {
                    currentLine.Append(word);
                }
            }
            else
            {
                if (currentLine.Length > 0)
                {
                    currentLine.Append(' ');
                }
                currentLine.Append(word);
            }
        }

        if (currentLine.Length > 0)
        {
            yield return currentLine.ToString();
        }
    }

    private static string GetContractTitle(ContractMetadata contract)
    {
        if (contract.SourcePath is not null)
        {
            var fileName = Path.GetFileNameWithoutExtension(contract.SourcePath);
            return $"Contract: {fileName}";
        }

        return "Contract";
    }

    private static string GetContractType(ContractMetadata contract)
    {
        // Infer from target patterns
        if (contract.Target?.Patterns is { Count: > 0 })
        {
            var firstPattern = contract.Target.Patterns[0];
            var extension = Path.GetExtension(firstPattern);
            if (!string.IsNullOrWhiteSpace(extension))
            {
                return extension;
            }
        }

        return string.Empty;
    }
}
