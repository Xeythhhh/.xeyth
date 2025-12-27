using Contracts.Core.Models;

namespace Contracts.Core.Tests.Models;

public class ContractMetadataSnapshotTests
{
    [Fact]
    public Task ContractMetadata_Structure_MatchesSnapshot()
    {
        // Arrange
        var contract = new ContractMetadata
        {
            Target = new TargetConfiguration
            {
                Patterns = new List<string> { "**/*.task" },
                Exclude = new List<string> { "**/archive/**" }
            },
            Schema = new SchemaDefinition
            {
                RequiredSections = new List<RequiredSection>
                {
                    new RequiredSection
                    {
                        Name = "Delegation Prompt",
                        Level = 2,
                        Description = "Role-based delegation section"
                    }
                },
                RequiredFields = new List<RequiredFieldGroup>
                {
                    new RequiredFieldGroup
                    {
                        Section = "Delegation Prompt",
                        Fields = new List<RequiredField>
                        {
                            new RequiredField
                            {
                                Name = "Role",
                                Pattern = "^## Role:",
                                Description = "Required role specification"
                            }
                        }
                    }
                }
            },
            Naming = new NamingConvention
            {
                Pattern = "^[A-Z][a-zA-Z0-9]+\\.task$",
                Description = "PascalCase task files",
                Examples = new List<string> { "MyTask.task", "AnotherTask.task" }
            },
            Archiving = new ArchivingRules
            {
                Directory = "archive",
                Pattern = "^[A-Z][a-zA-Z0-9]+\\.\\d{4}-\\d{2}-\\d{2}\\.task$",
                Description = "Archived with date suffix",
                Examples = new List<string> { "MyTask.2024-12-24.task" }
            },
            Meta = new ContractMeta
            {
                Version = "1.0",
                Author = "AI Framework",
                Description = "Task contract definition",
                LastUpdated = "2024-12-24"
            },
            SourcePath = "/path/to/Task.template.metadata"
        };

        // Act & Assert
        return Verify(contract);
    }
}
