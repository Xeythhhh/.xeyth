using System.IO;
using Contracts.Core.Models;
using Contracts.Core.Rendering;

namespace Contracts.Core.Tests.Rendering;

public class CardRendererTests
{
    [Fact]
    public Task CardRenderer_RendersSingleContract()
    {
        // Arrange
        var contract = CreateSampleContract();
        var output = new StringWriter();
        var context = new RenderContext
        {
            Contract = contract,
            Output = output,
            Configuration = new RendererConfiguration { Style = RenderStyle.Card }
        };

        var renderer = new CardRenderer();

        // Act
        renderer.Render(context);

        // Assert
        return Verify(output.ToString());
    }

    [Fact]
    public Task CardRenderer_RendersWithVerbose()
    {
        // Arrange
        var contract = CreateSampleContract();
        var output = new StringWriter();
        var context = new RenderContext
        {
            Contract = contract,
            Output = output,
            Configuration = new RendererConfiguration { Style = RenderStyle.Card, Verbose = true },
            RootPath = "/test/path"
        };

        var renderer = new CardRenderer();

        // Act
        renderer.Render(context);

        // Assert
        return Verify(output.ToString());
    }

    [Fact]
    public Task CardRenderer_RendersWithCustomWidth()
    {
        // Arrange
        var contract = CreateSampleContract();
        var output = new StringWriter();
        var context = new RenderContext
        {
            Contract = contract,
            Output = output,
            Configuration = new RendererConfiguration { Style = RenderStyle.Card, MaxWidth = 60 }
        };

        var renderer = new CardRenderer();

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
                Patterns = new List<string> { "**/*.task" },
                Exclude = new List<string> { "**/archive/**" }
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
                Pattern = "^[A-Z][a-zA-Z0-9]+\\.\\d{4}-\\d{2}-\\d{2}\\.task$"
            },
            RelatedFiles = new List<RelatedFilePattern>
            {
                new RelatedFilePattern { Pattern = "*.report", Required = false },
                new RelatedFilePattern { Pattern = "*.review", Required = false }
            }
        };
    }
}
