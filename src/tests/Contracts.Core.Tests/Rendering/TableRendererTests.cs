using System.IO;
using Contracts.Core.Models;
using Contracts.Core.Rendering;

namespace Contracts.Core.Tests.Rendering;

public class TableRendererTests
{
    [Fact]
    public Task TableRenderer_RendersSingleContract()
    {
        // Arrange
        var contract = CreateSampleContract("Task.template");
        var output = new StringWriter();
        var context = new RenderContext
        {
            Contract = contract,
            Output = output,
            Configuration = new RendererConfiguration { Style = RenderStyle.Table }
        };

        var renderer = new TableRenderer();

        // Act
        renderer.Render(context);

        // Assert
        return Verify(output.ToString());
    }

    [Fact]
    public Task TableRenderer_RendersMultipleContracts()
    {
        // Arrange
        var contract1 = CreateSampleContract("Task.template");
        var contract2 = CreateSampleContract("Plan.template");
        var contract3 = CreateSampleContract("Report.template");
        
        var output = new StringWriter();
        var context = new RenderContext
        {
            Contract = contract1,
            AdditionalContracts = new List<ContractMetadata> { contract2, contract3 },
            Output = output,
            Configuration = new RendererConfiguration { Style = RenderStyle.Table }
        };

        var renderer = new TableRenderer();

        // Act
        renderer.Render(context);

        // Assert
        return Verify(output.ToString());
    }

    private static ContractMetadata CreateSampleContract(string name)
    {
        var extension = Path.GetExtension(name);
        return new ContractMetadata
        {
            SourcePath = $"/test/path/{name}.metadata",
            Target = new TargetConfiguration
            {
                Patterns = new List<string> { $"**/*{extension}" }
            },
            Schema = new SchemaDefinition
            {
                RequiredSections = new List<RequiredSection>
                {
                    new RequiredSection { Name = "Section 1", Level = 2 },
                    new RequiredSection { Name = "Section 2", Level = 2 }
                }
            },
            Archiving = name.Contains("Plan") ? null : new ArchivingRules
            {
                Directory = "archive",
                Pattern = $"*.{extension}"
            }
        };
    }
}
