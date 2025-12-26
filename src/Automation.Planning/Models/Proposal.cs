namespace Automation.Planning.Models;

public sealed record Proposal(
    string Name,
    string Title,
    ProposalStatus Status,
    string Path,
    string? Submitted,
    string? Author,
    string? RelatedTask,
    string Content);
