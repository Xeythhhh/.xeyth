using System.Text;
using Contracts.Core.Models;

namespace Contracts.Core.Rendering;

/// <summary>
/// Renders contracts in a tabular format for comparing multiple contracts.
/// </summary>
public sealed class TableRenderer : IContractRenderer
{
    private const int NameWidth = 20;
    private const int TypeWidth = 10;
    private const int SectionsWidth = 12;
    private const int ArchiveWidth = 10;
    private const int TruncateIndicatorLength = 3;

    public void Render(RenderContext context)
    {
        var contracts = new List<ContractMetadata> { context.Contract };
        if (context.AdditionalContracts is not null)
        {
            contracts.AddRange(context.AdditionalContracts);
        }

        RenderTable(context.Output, contracts);
    }

    private static void RenderTable(TextWriter output, List<ContractMetadata> contracts)
    {
        // Header
        output.WriteLine("┌" + new string('─', NameWidth) + "┬" + new string('─', TypeWidth) + "┬" + new string('─', SectionsWidth) + "┬" + new string('─', ArchiveWidth) + "┐");
        output.WriteLine("│ " + "Contract".PadRight(NameWidth - 2) + " │ " + "Type".PadRight(TypeWidth - 2) + " │ " + "Sections".PadRight(SectionsWidth - 2) + " │ " + "Archive".PadRight(ArchiveWidth - 2) + " │");
        output.WriteLine("├" + new string('─', NameWidth) + "┼" + new string('─', TypeWidth) + "┼" + new string('─', SectionsWidth) + "┼" + new string('─', ArchiveWidth) + "┤");

        // Rows
        foreach (var contract in contracts)
        {
            var name = GetContractName(contract);
            var type = GetContractType(contract);
            var sections = contract.Schema?.RequiredSections?.Count.ToString() ?? "0";
            var archive = contract.Archiving is not null ? "Yes" : "No";

            output.WriteLine("│ " + Truncate(name, NameWidth - 2).PadRight(NameWidth - 2) + " │ " + Truncate(type, TypeWidth - 2).PadRight(TypeWidth - 2) + " │ " + sections.PadRight(SectionsWidth - 2) + " │ " + archive.PadRight(ArchiveWidth - 2) + " │");
        }

        // Footer
        output.WriteLine("└" + new string('─', NameWidth) + "┴" + new string('─', TypeWidth) + "┴" + new string('─', SectionsWidth) + "┴" + new string('─', ArchiveWidth) + "┘");
    }

    private static string GetContractName(ContractMetadata contract)
    {
        if (contract.SourcePath is not null)
        {
            return Path.GetFileNameWithoutExtension(contract.SourcePath);
        }

        return "Unknown";
    }

    private static string GetContractType(ContractMetadata contract)
    {
        if (contract.Target?.Patterns is { Count: > 0 })
        {
            var firstPattern = contract.Target.Patterns[0];
            var extension = Path.GetExtension(firstPattern);
            if (!string.IsNullOrWhiteSpace(extension))
            {
                return extension;
            }
        }

        return "-";
    }

    private static string Truncate(string text, int maxLength)
    {
        if (text.Length <= maxLength)
        {
            return text;
        }

        return text[..(maxLength - TruncateIndicatorLength)] + "...";
    }
}
