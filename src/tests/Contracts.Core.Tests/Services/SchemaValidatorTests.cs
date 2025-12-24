using Contracts.Core.Models;
using Contracts.Core.Services;

namespace Contracts.Core.Tests.Services;

public class SchemaValidatorTests
{
    private readonly SchemaValidator _validator = new();

    [Fact]
    public void Validate_WhenRequiredSectionMissing_ReturnsViolation()
    {
        var contract = BuildContract(requiredSection: "Delegation Prompt");
        var content = "## Task Details\nStatus: Not Started";

        var violations = _validator.Validate("Task.task", content, contract);

        var violation = Assert.Single(violations);
        Assert.Equal("missing-section", violation.Code);
        Assert.Equal(ViolationSeverity.Error, violation.Severity);
    }

    [Fact]
    public void Validate_WhenRequiredFieldMissing_ReturnsViolation()
    {
        var contract = BuildContract(requiredSection: "Delegation Prompt", requiredFieldPattern: "^## Role:");
        var content = "## Delegation Prompt\n**Objective**: Test objective";

        var violations = _validator.Validate("Task.task", content, contract);

        var violation = Assert.Single(violations);
        Assert.Equal("missing-field", violation.Code);
        Assert.Equal("Delegation Prompt", violation.Section);
    }

    private static ContractMetadata BuildContract(string requiredSection, string? requiredFieldPattern = null)
    {
        return new ContractMetadata
        {
            Target = new TargetConfiguration
            {
                Patterns = new List<string> { "**/*.task" }
            },
            Schema = new SchemaDefinition
            {
                RequiredSections = new List<RequiredSection>
                {
                    new RequiredSection
                    {
                        Name = requiredSection,
                        Level = 2
                    }
                },
                RequiredFields = requiredFieldPattern is null
                    ? new List<RequiredFieldGroup>()
                    : new List<RequiredFieldGroup>
                    {
                        new RequiredFieldGroup
                        {
                            Section = requiredSection,
                            Fields = new List<RequiredField>
                            {
                                new RequiredField
                                {
                                    Name = "Role",
                                    Pattern = requiredFieldPattern
                                }
                            }
                        }
                    }
            }
        };
    }
}
