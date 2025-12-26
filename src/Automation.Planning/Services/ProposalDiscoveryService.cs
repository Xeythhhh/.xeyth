using Automation.Cli.Common.Paths;
using Automation.Planning.Models;

namespace Automation.Planning.Services;

public sealed class ProposalDiscoveryService
{
    private readonly ProposalParser _parser;

    public ProposalDiscoveryService(ProposalParser parser)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    public async Task<IReadOnlyList<Proposal>> DiscoverAsync(string rootPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("Root path is required", nameof(rootPath));
        }

        var fullRoot = AbsolutePath.From(rootPath);
        if (!Directory.Exists(fullRoot.Value))
        {
            throw new DirectoryNotFoundException($"Root path not found: {fullRoot}");
        }

        var proposalFiles = Directory
            .EnumerateFiles(fullRoot.Value, "*.proposal", SearchOption.AllDirectories)
            .ToList();

        var parseTasks = proposalFiles
            .Select(file => Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return _parser.Parse(file);
            }, cancellationToken))
            .ToArray();

        var parsed = await Task.WhenAll(parseTasks).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        return parsed
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<Proposal?> FindByNameAsync(string rootPath, string proposalName, CancellationToken cancellationToken = default)
    {
        var proposals = await DiscoverAsync(rootPath, cancellationToken);
        return proposals.FirstOrDefault(p => string.Equals(p.Name, proposalName, StringComparison.OrdinalIgnoreCase));
    }
}
