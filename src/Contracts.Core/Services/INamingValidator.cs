using Contracts.Core.Models;

namespace Contracts.Core.Services;

public interface INamingValidator
{
    IReadOnlyList<Violation> Validate(string filePath, ContractMetadata contract);
}
