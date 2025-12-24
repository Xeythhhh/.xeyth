using Contracts.Core.Models;

namespace Contracts.Core.Services;

public interface IContractValidationService
{
    Task<ValidationResult> ValidateAsync(
        string filePath,
        ContractMetadata contract,
        CancellationToken cancellationToken = default);

    Task<ValidationResult> ValidateAsync(
        string filePath,
        IEnumerable<ContractMetadata> contracts,
        string? rootPath = null,
        CancellationToken cancellationToken = default);

    ValidationResult Validate(
        string filePath,
        string content,
        ContractMetadata contract);

    ValidationResult Validate(
        string filePath,
        string content,
        IEnumerable<ContractMetadata> contracts,
        string? rootPath = null);
}
