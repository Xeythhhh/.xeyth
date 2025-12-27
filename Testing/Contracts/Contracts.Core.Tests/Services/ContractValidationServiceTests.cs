using Contracts.Core.Models;
using Contracts.Core.Services;

namespace Contracts.Core.Tests.Services;

public class ContractValidationServiceTests
{
    private readonly ContractValidationService _service = new();

    [Fact]
    public Task Validate_AggregatesViolations_FromNamingAndSchema()
    {
        var contract = new ContractMetadata
        {
            Target = new TargetConfiguration
            {
                Patterns = new List<string> { "**/*.task" },
                Exclude = new List<string> { "**/archive/**" }
            },
            Naming = new NamingConvention
            {
                Pattern = "^Valid.*\\.task$",
                Description = "PascalCase task name"
            },
            Archiving = new ArchivingRules
            {
                Directory = "archive",
                Pattern = "^Valid.*\\.\\d{4}-\\d{2}-\\d{2}\\.task$",
                Description = "Archived with date suffix"
            },
            Schema = new SchemaDefinition
            {
                RequiredSections = new List<RequiredSection>
                {
                    new RequiredSection
                    {
                        Name = "Delegation Prompt",
                        Level = 2,
                        Description = "Role-based instructions"
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
                                Description = "Role assignment"
                            }
                        }
                    }
                }
            }
        };

        var content = "## Delegation Prompt\n**Objective**: Build validation engine";

        var result = _service.Validate("workspace/Invalid.task", content, contract);

        return Verify(result);
    }

    [Fact]
    public void Validate_WithNoMatchingContract_ReturnsWarning()
    {
        var contracts = new[]
        {
            new ContractMetadata
            {
                Target = new TargetConfiguration
                {
                    Patterns = new List<string> { "**/*.plan" }
                },
                Naming = new NamingConvention
                {
                    Pattern = "^Valid.*\\.plan$"
                }
            }
        };

        var result = _service.Validate(
            filePath: "workspace/Task.task",
            content: "# Task",
            contracts: contracts,
            rootPath: "workspace");

        Assert.True(result.HasWarnings);
        Assert.True(result.IsSuccess);
        var warning = Assert.Single(result.Violations);
        Assert.Equal("contract-not-found", warning.Code);
    }
}
