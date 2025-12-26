using Automation.Planning.Models;

namespace Automation.Planning.Services;

public sealed class ProposalDiscoveryService
{
    private readonly ProposalParser _parser;

    public ProposalDiscoveryService(ProposalParser parser)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    public Task<IReadOnlyList<Proposal>> DiscoverAsync(string rootPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("Root path is required", nameof(rootPath));
        }

        var fullRoot = Path.GetFullPath(rootPath);
        if (!Directory.Exists(fullRoot))
        {
            throw new DirectoryNotFoundException($"Root path not found: {fullRoot}");
        }

        var proposals = Directory
            .EnumerateFiles(fullRoot, "*.proposal", SearchOption.AllDirectories)
            .Select(_parser.Parse)
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Task.FromResult<IReadOnlyList<Proposal>>(proposals);
    }

    public async Task<Proposal?> FindByNameAsync(string rootPath, string proposalName, CancellationToken cancellationToken = default)
    {
        var proposals = await DiscoverAsync(rootPath, cancellationToken);
        return proposals.FirstOrDefault(p => string.Equals(p.Name, proposalName, StringComparison.OrdinalIgnoreCase));
    }
}
