using Contracts.Core.Models;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Contracts.Core.Services;

/// <summary>
/// Implementation of contract discovery service using filesystem scanning and YAML parsing.
/// </summary>
public class ContractDiscoveryService : IContractDiscoveryService
{
    private readonly IDeserializer _yamlDeserializer;

    public ContractDiscoveryService()
    {
        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    public async Task<IReadOnlyList<ContractMetadata>> DiscoverContractsAsync(
        string rootPath,
        CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(rootPath))
        {
            return Array.Empty<ContractMetadata>();
        }

        var matcher = new Matcher();
        matcher.AddInclude("**/*.metadata");

        var matchResult = matcher.Execute(
            new DirectoryInfoWrapper(new DirectoryInfo(rootPath)));

        var contracts = new List<ContractMetadata>();

        foreach (var file in matchResult.Files)
        {
            var filePath = Path.Combine(rootPath, file.Path);

            try
            {
                var contract = await LoadContractFromFileAsync(filePath, cancellationToken);
                if (contract is not null)
                {
                    contracts.Add(contract);
                }
            }
            catch (Exception ex)
            {
                // Log or handle parse errors gracefully
                // For now, skip invalid files
                Console.Error.WriteLine($"Warning: Failed to load contract from {filePath}: {ex.Message}");
            }
        }

        return contracts;
    }

    public async Task<IReadOnlyList<ContractMetadata>> DiscoverAndMergeContractsAsync(
        IEnumerable<string> rootPaths,
        CancellationToken cancellationToken = default)
    {
        var allContracts = new Dictionary<string, ContractMetadata>();

        foreach (var rootPath in rootPaths)
        {
            var contracts = await DiscoverContractsAsync(rootPath, cancellationToken);

            foreach (var contract in contracts)
            {
                // Use filename as key for merging (user contracts override framework)
                var fileName = Path.GetFileName(contract.SourcePath ?? string.Empty);
                if (!string.IsNullOrEmpty(fileName))
                {
                    allContracts[fileName] = contract;
                }
            }
        }

        return allContracts.Values.ToList();
    }

    private async Task<ContractMetadata?> LoadContractFromFileAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        var yaml = await File.ReadAllTextAsync(filePath, cancellationToken);

        if (string.IsNullOrWhiteSpace(yaml))
        {
            return null;
        }

        var contract = _yamlDeserializer.Deserialize<ContractMetadata>(yaml);

        // Set source path for tracking
        return contract with { SourcePath = filePath };
    }
}
