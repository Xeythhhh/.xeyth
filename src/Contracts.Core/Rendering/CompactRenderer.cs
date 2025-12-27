using Contracts.Core.Models;

namespace Contracts.Core.Rendering;

/// <summary>
/// Renders contracts in a compact single-line format.
/// </summary>
public sealed class CompactRenderer : IContractRenderer
{
    public void Render(RenderContext context)
    {
        var contract = context.Contract;
        var output = context.Output;

        var name = GetContractName(contract);
        var type = GetContractType(contract);
        var sections = contract.Schema?.RequiredSections?.Count ?? 0;
        var patterns = contract.Target?.Patterns is { Count: > 0 } 
            ? string.Join(",", contract.Target.Patterns) 
            : "none";

        output.WriteLine($"{name} ({type}): {sections} sections, patterns: {patterns}");
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

        return "unknown";
    }
}
