using Contracts.Core.Models;

namespace Contracts.Core.Rendering;

/// <summary>
/// Renders contracts in a hierarchical tree view.
/// </summary>
public sealed class TreeRenderer : IContractRenderer
{
    public void Render(RenderContext context)
    {
        var contract = context.Contract;
        var output = context.Output;

        var name = GetContractName(contract);
        output.WriteLine($"📄 {name}");

        // Target patterns
        if (contract.Target?.Patterns is { Count: > 0 })
        {
            output.WriteLine("├─ 🎯 Target: " + string.Join(", ", contract.Target.Patterns));
        }

        // Schema
        if (contract.Schema?.RequiredSections is { Count: > 0 })
        {
            output.WriteLine("├─ 📋 Schema");
            for (int i = 0; i < contract.Schema.RequiredSections.Count; i++)
            {
                var section = contract.Schema.RequiredSections[i];
                var isLast = i == contract.Schema.RequiredSections.Count - 1 && 
                             contract.Naming is null && 
                             contract.Archiving is null;
                var prefix = isLast ? "└─" : "├─";
                output.WriteLine($"│  {prefix} {section.Name} (required)");
            }
        }

        // Naming
        if (contract.Naming is not null)
        {
            var hasArchiving = contract.Archiving is not null;
            var prefix = hasArchiving ? "├─" : "└─";
            output.WriteLine($"{prefix} ✏️  Naming: {contract.Naming.Pattern}");
        }

        // Archiving
        if (contract.Archiving is not null)
        {
            output.WriteLine($"└─ 📁 Archiving: {contract.Archiving.Directory}/{contract.Archiving.Pattern}");
        }
    }

    private static string GetContractName(ContractMetadata contract)
    {
        if (contract.SourcePath is not null)
        {
            return Path.GetFileNameWithoutExtension(contract.SourcePath) + " Contract";
        }

        return "Contract";
    }
}
