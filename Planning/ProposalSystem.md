# Proposal System

**Purpose**: Capture optimization ideas, improvements, and brilliant insights discovered during task implementation without disrupting current work focus.

## Overview

While working on tasks, agents often discover unrelated optimizations, refactoring opportunities, or feature ideas. The proposal system provides a lightweight mechanism to capture these insights for later review by the Orchestrator without context-switching.

## Workflow

### 1. Capture Proposal (Implementation Agent)

When discovering an optimization/idea during implementation:

1. Create `.proposal` file in `Planning/Proposed/` or relevant slice/sub-slice
2. Use naming convention: `{Area}.{ShortName}.proposal`
3. Fill proposal template with context, benefits, effort
4. Continue current task without interruption
5. Optionally note proposal in task Progress Log

### 2. Review Proposals (Orchestrator)

Orchestrator periodically reviews pending proposals:

1. Run `xeyth-planning list-proposals --pending` (CLI tool)
2. Review each proposal for value, priority, feasibility
3. Make decision: Accept (create task), Defer, Reject
4. Update proposal status
5. Create `.plan` or `.task` for accepted proposals

### 3. Track Decisions

Proposal lifecycle:
- **Pending** → **Accepted** (task created) | **Deferred** (low priority) | **Rejected** (not viable)
- Accepted proposals link to created task file
- Deferred/Rejected proposals move to `archive/` with decision rationale

## Proposal File Format

```markdown
# Proposal: {Title}

**Status**: Pending  
**Submitted**: YYYY-MM-DD  
**Author**: {Agent Role}  
**Related Task**: {Link to task where discovered}

## Context

{What prompted this proposal? What problem does it solve?}

## Proposal

{Detailed description of the optimization/idea/feature}

## Benefits

- {Benefit 1}
- {Benefit 2}
- {Benefit 3}

## Effort Estimate

**Size**: Small | Medium | Large

**Impact**:
- {What needs to change}
- {Dependencies}
- {Risks}

## Implementation Notes

{Any specific guidance, patterns, or constraints}

## Decision

**Status**: Pending | Accepted | Deferred | Rejected  
**Decision Date**: YYYY-MM-DD  
**Rationale**: {Why accepted/deferred/rejected}  
**Task Created**: {Link to .task or .plan if accepted}
```

## File Location

### Global Proposals (cross-cutting)
`Ai/Planning/Proposed/{Area}.{ShortName}.proposal`

Examples:
- `Ai/Planning/Proposed/Framework.RoleEnhancements.proposal`
- `Ai/Planning/Proposed/Testing.SnapshotValidation.proposal`

### Slice-Specific Proposals
`{Slice}/Proposed/{ShortName}.proposal`

Examples:
- `Contracts/Proposed/YamlSchemaExtensions.proposal`
- `Automation/Proposed/ProgressReporting.proposal`

### Sub-Slice Proposals
`{Slice}/{SubSlice}/Proposed/{ShortName}.proposal`

Examples:
- `Framework/Proposed/DelegationTemplates.proposal`

## Naming Convention

Format: `{Area}.{ShortName}.proposal`

- **Area**: Component/slice affected (e.g., Contracts, Framework, CLI)
- **ShortName**: 2-4 words describing proposal (e.g., YamlSchemaExtensions, ProgressReporting)
- **Extension**: Always `.proposal`

Examples:
- `Contracts.ValidationCaching.proposal`
- `CLI.InteractiveMode.proposal`
- `Framework.ModelMetadata.proposal`

## CLI Tool Integration

The `xeyth-planning` tool provides proposal management:

```bash
# List all pending proposals
xeyth-planning list-proposals --pending

# List all proposals (any status)
xeyth-planning list-proposals --all

# Show proposal details
xeyth-planning show-proposal Framework.RoleEnhancements

# Accept proposal (creates task)
xeyth-planning accept-proposal Framework.RoleEnhancements --task Framework/RoleEnhancements.task

# Defer proposal
xeyth-planning defer-proposal CLI.InteractiveMode --reason "Low priority for current release"

# Reject proposal
xeyth-planning reject-proposal Testing.SnapshotValidation --reason "Already solved by Verify library"
```

## Best Practices

### When to Create Proposals

✅ **Do create proposals for**:
- Performance optimizations discovered during implementation
- Refactoring opportunities that improve maintainability
- Feature ideas that enhance existing functionality
- Cross-cutting improvements (logging, error handling, validation)
- Tooling enhancements that improve developer experience

❌ **Don't create proposals for**:
- Bugs discovered in current task (fix immediately)
- Required changes to complete current deliverables (part of task)
- Trivial changes (typos, formatting) - just fix them
- Ideas without clear value proposition

### Proposal Quality

Good proposals include:
- **Clear context** - Why does this matter?
- **Concrete benefits** - What improves?
- **Realistic effort** - How much work?
- **Implementation hints** - How to approach it?

### Avoid Scope Creep

- **Stay focused** - Don't implement proposals while on another task
- **Capture quickly** - Spend 5-10 minutes writing proposal, not hours
- **Trust the process** - Orchestrator will review and prioritize
- **No analysis paralysis** - Brief proposal is better than perfect one

## Examples

### Example 1: Performance Optimization

```markdown
# Proposal: Contract Validation Caching

**Status**: Pending  
**Submitted**: 2024-12-24  
**Author**: Implementation Agent  
**Related Task**: Contracts/ContractValidation.task

## Context

While implementing contract validation, noticed that the same YAML schemas
are parsed multiple times for the same file in a single validation run.
This causes unnecessary file I/O and parsing overhead.

## Proposal

Implement in-memory caching of parsed YAML schemas during validation runs.
Cache key: absolute file path + last modified timestamp. Clear cache between
validation runs.

## Benefits

- Reduce file I/O by ~60% for workspaces with many contracts
- Improve validation performance (estimated 30-40% faster)
- Lower memory allocation (parse once, reuse)
- No behavior change (cache is transparent)

## Effort Estimate

**Size**: Small

**Impact**:
- Add CacheService to Contracts.Core
- Update YamlSchemaValidator to use cache
- Add cache clearing to ValidateCommandRunner
- Write cache expiration tests

## Implementation Notes

Use `ConcurrentDictionary<string, ParsedSchema>` with file path + timestamp
as key. Invalidate on file modification. Consider LRU eviction if memory
becomes concern (unlikely for typical workspace sizes).

## Decision

**Status**: Pending
```

### Example 2: Feature Enhancement

```markdown
# Proposal: Interactive CLI Mode

**Status**: Pending  
**Submitted**: 2024-12-24  
**Author**: Implementation Agent  
**Related Task**: Automation/CliVisualEnhancement.task

## Context

Current CLI tools require all parameters via flags. For exploratory workflows
(e.g., discovering available contracts, selecting validation targets), users
must know exact file paths upfront.

## Proposal

Add `--interactive` flag to CLI tools that enables guided workflows:
- xeyth-contracts: Browse available contracts, select for validation
- xeyth-verify: Interactive setup wizard with file browser
- xeyth-planning: Task selection from backlog

## Benefits

- Improved discoverability (users explore without docs)
- Reduced errors (validation during input)
- Better UX for infrequent users
- Still scriptable (default non-interactive mode)

## Effort Estimate

**Size**: Medium

**Impact**:
- Extend Automation.Cli.Common with interactive prompts
- Add SelectionPrompt, ConfirmationPrompt wrappers
- Update each tool's CommandRunner with interactive paths
- Write interactive mode tests (Spectre.Console.Testing)

## Implementation Notes

Use Spectre.Console SelectionPrompt, MultiSelectionPrompt, and TextPrompt.
Gracefully degrade in CI/non-interactive (show error if --interactive used).
Maintain backward compatibility (all flags still work).

## Decision

**Status**: Pending
```

## Archiving

When proposal reaches terminal state (Accepted/Deferred/Rejected):

1. Update **Decision** section with status, date, rationale
2. Move to `{Location}/archive/` (same directory as proposal)
3. Rename: `{Area}.{ShortName}.{Status}.YYYY-MM-DD.proposal`
4. Link to created task (if accepted)

Examples:
- `Planning/Proposed/archive/Framework.RoleEnhancements.Accepted.2024-12-24.proposal`
- `Contracts/Proposed/archive/ValidationCaching.Rejected.2024-12-25.proposal`

## Integration with Task System

Accepted proposals become tasks:

1. Orchestrator reviews proposal, decides to accept
2. Creates task file in appropriate slice: `{Slice}/{TaskName}.task`
3. Links proposal to task in proposal Decision section
4. Links task to proposal in task Context section
5. Archives proposal with Accepted status
6. Task proceeds through normal workflow (planning, implementation, review)

## Continuous Improvement

The proposal system enables continuous improvement without disrupting execution:

- **Capture insights** - Don't lose good ideas
- **Maintain focus** - Finish current task first
- **Prioritize effectively** - Orchestrator reviews all proposals together
- **Prevent scope creep** - Clear separation between current work and future ideas
- **Knowledge sharing** - Proposals document rationale and context

