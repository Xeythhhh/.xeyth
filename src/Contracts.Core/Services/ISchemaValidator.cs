using Contracts.Core.Models;

namespace Contracts.Core.Services;

public interface ISchemaValidator
{
    IReadOnlyList<Violation> Validate(string filePath, string content, ContractMetadata contract);
}
