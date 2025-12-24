using Contracts.Core.Models;
using Contracts.Core.Services;

namespace Contracts.Core.Tests.Services;

public class ContractDiscoveryServiceTests
{
    private readonly ContractDiscoveryService _service;

    public ContractDiscoveryServiceTests()
    {
        _service = new ContractDiscoveryService();
    }

    [Fact]
    public async Task DiscoverContractsAsync_WithNonExistentPath_ReturnsEmptyList()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act
        var result = await _service.DiscoverContractsAsync(nonExistentPath);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task DiscoverContractsAsync_WithEmptyDirectory_ReturnsEmptyList()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            // Act
            var result = await _service.DiscoverContractsAsync(tempPath);

            // Assert
            Assert.Empty(result);
        }
        finally
        {
            Directory.Delete(tempPath, true);
        }
    }

    [Fact]
    public async Task DiscoverContractsAsync_WithValidMetadataFile_ReturnsContract()
    {
        // Arrange
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        var metadataPath = Path.Combine(tempPath, "Test.template.metadata");
        var yamlContent = @"
target:
  patterns:
    - '**/*.test'
schema:
  requiredSections:
    - name: 'Test Section'
      level: 2
      description: 'A test section'
";
        await File.WriteAllTextAsync(metadataPath, yamlContent);

        try
        {
            // Act
            var result = await _service.DiscoverContractsAsync(tempPath);

            // Assert
            Assert.Single(result);
            var contract = result[0];
            Assert.NotNull(contract.Target);
            Assert.Contains("**/*.test", contract.Target.Patterns);
            Assert.NotNull(contract.Schema);
            Assert.Single(contract.Schema.RequiredSections!);
        }
        finally
        {
            Directory.Delete(tempPath, true);
        }
    }

    [Fact]
    public async Task DiscoverAndMergeContractsAsync_WithMultiplePaths_MergesCorrectly()
    {
        // Arrange
        var frameworkPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var userPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(frameworkPath);
        Directory.CreateDirectory(userPath);

        // Framework contract
        var frameworkMetadata = Path.Combine(frameworkPath, "Task.template.metadata");
        await File.WriteAllTextAsync(frameworkMetadata, @"
target:
  patterns:
    - '**/*.task'
naming:
  pattern: '^[A-Z].*\.task$'
  description: 'Framework default'
");

        // User contract (should override)
        var userMetadata = Path.Combine(userPath, "Task.template.metadata");
        await File.WriteAllTextAsync(userMetadata, @"
target:
  patterns:
    - '**/*.task'
naming:
  pattern: '^custom.*\.task$'
  description: 'User override'
");

        try
        {
            // Act
            var result = await _service.DiscoverAndMergeContractsAsync(
                new[] { frameworkPath, userPath });

            // Assert
            Assert.Single(result); // Merged into one contract
            var contract = result[0];
            Assert.NotNull(contract.Naming);
            Assert.Equal("User override", contract.Naming.Description); // User overrides framework
        }
        finally
        {
            Directory.Delete(frameworkPath, true);
            Directory.Delete(userPath, true);
        }
    }
}
