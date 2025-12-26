using System.IO;
using Contracts.Core.Models;
using Contracts.Core.Rendering;

namespace Contracts.Core.Tests.Rendering;

public class TreeRendererTests
{
    [Fact]
    public Task TreeRenderer_RendersSingleContract()
    {
        // Arrange
        var contract = CreateSampleContract();
        var output = new StringWriter();
        var context = new RenderContext
        {
            Contract = contract,
            Output = output,
            Configuration = new RendererConfiguration { Style = RenderStyle.Tree }
        };

        var renderer = new TreeRenderer();

        // Act
        renderer.Render(context);

        // Assert
        return Verify(output.ToString());
    }

    [Fact]
    public Task TreeRenderer_RendersWithoutArchiving()
    {
        // Arrange
        var contract = CreateSampleContract();
        contract = contract with { Archiving = null };
        
        var output = new StringWriter();
        var context = new RenderContext
        {
            Contract = contract,
            Output = output,
            Configuration = new RendererConfiguration { Style = RenderStyle.Tree }
        };

        var renderer = new TreeRenderer();

        // Act
        renderer.Render(context);

        // Assert
        return Verify(output.ToString());
    }

    private static ContractMetadata CreateSampleContract()
    {
        return new ContractMetadata
        {
            SourcePath = "/test/path/Task.template.metadata",
            Target = new TargetConfiguration
            {
                Patterns = new List<string> { "**/*.task" }
            },
            Schema = new SchemaDefinition
            {
                RequiredSections = new List<RequiredSection>
                {
                    new RequiredSection { Name = "Delegation Prompt", Level = 2 },
                    new RequiredSection { Name = "Task Details", Level = 2 }
                }
            },
            Naming = new NamingConvention
            {
                Pattern = "^[A-Z][a-zA-Z0-9]+\\.task$"
            },
            Archiving = new ArchivingRules
            {
                Directory = "archive",
                Pattern = "*.task"
            }
        };
    }
}
