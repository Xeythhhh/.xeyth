using Contracts.Core.Models;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Contracts.Core.Services;

public class ContractValidationService : IContractValidationService
{
    private readonly ISchemaValidator _schemaValidator;
    private readonly INamingValidator _namingValidator;

    public ContractValidationService()
        : this(new SchemaValidator(), new NamingValidator())
    {
    }

    public ContractValidationService(
        ISchemaValidator schemaValidator,
        INamingValidator namingValidator)
    {
        _schemaValidator = schemaValidator ?? throw new ArgumentNullException(nameof(schemaValidator));
        _namingValidator = namingValidator ?? throw new ArgumentNullException(nameof(namingValidator));
    }

    public async Task<ValidationResult> ValidateAsync(
        string filePath,
        ContractMetadata contract,
        CancellationToken cancellationToken = default)
    {
        var content = await File.ReadAllTextAsync(filePath, cancellationToken);
        return Validate(filePath, content, contract);
    }

    public async Task<ValidationResult> ValidateAsync(
        string filePath,
        IEnumerable<ContractMetadata> contracts,
        string? rootPath = null,
        CancellationToken cancellationToken = default)
    {
        var content = await File.ReadAllTextAsync(filePath, cancellationToken);
        return Validate(filePath, content, contracts, rootPath);
    }

    public ValidationResult Validate(
        string filePath,
        string content,
        ContractMetadata contract)
    {
        var violations = new List<Violation>();
        violations.AddRange(_namingValidator.Validate(filePath, contract));
        violations.AddRange(_schemaValidator.Validate(filePath, content, contract));

        return ValidationResult.FromViolations(filePath, violations);
    }

    public ValidationResult Validate(
        string filePath,
        string content,
        IEnumerable<ContractMetadata> contracts,
        string? rootPath = null)
    {
        var matchingContract = FindContractForFile(filePath, contracts, rootPath);
        if (matchingContract is null)
        {
            return ValidationResult.FromViolations(
                filePath,
                new[]
                {
                    new Violation(
                        Code: "contract-not-found",
                        Message: "No contract matched this file; skipping schema validation.",
                        Severity: ViolationSeverity.Warning,
                        FilePath: filePath)
                });
        }

        return Validate(filePath, content, matchingContract);
    }

    private static ContractMetadata? FindContractForFile(
        string filePath,
        IEnumerable<ContractMetadata> contracts,
        string? rootPath)
    {
        var pathForMatching = GetPathForMatching(filePath, rootPath);

        foreach (var contract in contracts)
        {
            if (MatchesContract(contract, pathForMatching))
            {
                return contract;
            }
        }

        return null;
    }

    private static bool MatchesContract(ContractMetadata contract, string path)
    {
        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        matcher.AddIncludePatterns(contract.Target.Patterns);

        if (contract.Target.Exclude is { Count: > 0 })
        {
            matcher.AddExcludePatterns(contract.Target.Exclude);
        }

        return matcher.Match(path).HasMatches;
    }

    private static string GetPathForMatching(string filePath, string? rootPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            return NormalizePath(filePath);
        }

        try
        {
            var relative = Path.GetRelativePath(rootPath, filePath);
            if (!relative.StartsWith("..", StringComparison.Ordinal))
            {
                return NormalizePath(relative);
            }
        }
        catch
        {
            // Fall back to absolute path when relative resolution fails.
        }

        return NormalizePath(filePath);
    }

    private static string NormalizePath(string path) =>
        path.Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/');
}
