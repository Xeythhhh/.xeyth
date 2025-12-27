using Contracts.Core.Models;
using Contracts.Core.Services;

namespace Contracts.Core.Tests.Services;

public class NamingValidatorTests
{
    private readonly NamingValidator _validator = new();

    [Fact]
    public void Validate_WhenNameDoesNotMatchPattern_ReturnsError()
    {
        var contract = new ContractMetadata
        {
            Target = new TargetConfiguration
            {
                Patterns = new List<string> { "**/*.task" }
            },
            Naming = new NamingConvention
            {
                Pattern = "^Valid.*\\.task$"
            }
        };

        var violations = _validator.Validate("Invalid.task", contract);

        var violation = Assert.Single(violations);
        Assert.Equal("invalid-name", violation.Code);
        Assert.Equal(ViolationSeverity.Error, violation.Severity);
    }

    [Fact]
    public void Validate_WhenArchivedNameOutsideDirectory_ReturnsWarning()
    {
        var contract = new ContractMetadata
        {
            Target = new TargetConfiguration
            {
                Patterns = new List<string> { "**/*.task" }
            },
            Naming = new NamingConvention
            {
                Pattern = "^Valid.*\\.task$"
            },
            Archiving = new ArchivingRules
            {
                Directory = "archive",
                Pattern = "^Valid.*\\.\\d{4}-\\d{2}-\\d{2}\\.task$"
            }
        };

        var violations = _validator.Validate("Valid.2024-12-24.task", contract);

        var violation = Assert.Single(violations);
        Assert.Equal("archive-directory-mismatch", violation.Code);
        Assert.Equal(ViolationSeverity.Warning, violation.Severity);
    }
}
