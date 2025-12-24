using Contracts.Core.Models;

namespace Contracts.Core.Services;

/// <summary>
/// Service for discovering contract metadata files in the filesystem.
/// </summary>
public interface IContractDiscoveryService
{
    /// <summary>
    /// Discovers all contract metadata files in the specified root directory.
    /// </summary>
    /// <param name="rootPath">Root directory to search from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of discovered contract metadata.</returns>
    Task<IReadOnlyList<ContractMetadata>> DiscoverContractsAsync(
        string rootPath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Discovers contract metadata files from multiple root paths and merges them.
    /// User-defined contracts (later in the list) override framework defaults (earlier in the list).
    /// </summary>
    /// <param name="rootPaths">Root directories to search, in priority order (framework first, user last).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Merged collection of contract metadata.</returns>
    Task<IReadOnlyList<ContractMetadata>> DiscoverAndMergeContractsAsync(
        IEnumerable<string> rootPaths,
        CancellationToken cancellationToken = default);
}
