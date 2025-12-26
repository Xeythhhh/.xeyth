using System.Text;
using System.Text.RegularExpressions;
using Automation.Planning.Models;

namespace Automation.Planning.Services;

internal static class ProposalFormatter
{
    private static readonly Regex StatusLine = new(@"(?m)^\*\*Status\*\*:\s*.+$", RegexOptions.Compiled);

    internal static string UpdateStatus(string content, ProposalStatus status)
    {
        var replacement = $"**Status**: {status}";

        if (StatusLine.IsMatch(content))
        {
            return StatusLine.Replace(content, replacement);
        }

        var builder = new StringBuilder(content.TrimEnd());
        builder.AppendLine();
        builder.AppendLine(replacement);
        return builder.ToString();
    }

    internal static string AppendDecision(string content, ProposalStatus status, DateOnly decisionDate, string? rationale, string? taskPath)
    {
        var builder = new StringBuilder(content.TrimEnd());

        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine("## Decision");
        builder.AppendLine();
        builder.AppendLine($"**Status**: {status}");
        builder.AppendLine($"**Decision Date**: {decisionDate:yyyy-MM-dd}");

        if (!string.IsNullOrWhiteSpace(rationale))
        {
            builder.AppendLine($"**Rationale**: {rationale}");
        }

        if (!string.IsNullOrWhiteSpace(taskPath))
        {
            builder.AppendLine($"**Task Created**: {taskPath}");
        }

        return builder.ToString();
    }
}
