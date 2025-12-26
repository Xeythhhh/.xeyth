using Automation.Planning.Models;

namespace Automation.Planning.Services;

public sealed class ProposalDecisionService
{
    public string ArchiveDecision(Proposal proposal, ProposalStatus status, string rootPath, string? rationale = null, string? taskPath = null)
    {
        if (proposal is null)
        {
            throw new ArgumentNullException(nameof(proposal));
        }

        var timestamp = DateTime.UtcNow;
        var decisionDate = DateOnly.FromDateTime(timestamp);
        var updated = ProposalFormatter.UpdateStatus(proposal.Content, status);
        updated = ProposalFormatter.AppendDecision(updated, status, decisionDate, rationale, taskPath);

        var archiveDirectory = Path.Combine(Path.GetDirectoryName(proposal.Path) ?? rootPath, "archive");
        Directory.CreateDirectory(archiveDirectory);

        var archiveFileName = $"{proposal.Name}.{timestamp:yyyy-MM-dd-HHmmss-fff}.{Guid.NewGuid():N}.proposal";
        var archivePath = Path.Combine(archiveDirectory, archiveFileName);

        File.WriteAllText(archivePath, updated);
        File.Delete(proposal.Path);

        return archivePath;
    }
}
