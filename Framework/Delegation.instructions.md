---
applyTo: "**"
---

# Delegation Format Instructions

**CRITICAL**: When delegating work to another agent (different model or role), use the **code block delegation format**.

## When to Use Code Block Delegation

**Use code block when**:
- Strategic Agent delegating to Implementation Agent
- Implementation Agent delegating to Strategic Agent
- Any cross-model delegation (Claude Sonnet 4.5 ↔ GPT-5.1-Codex-Max)

**Do NOT use code block when**:
- Strategic Agent switching roles within itself (Orchestrator → Planner → Reviewer)
- Continuing work in same role (Flow.prompt.md)

**Role Switching Workflow (Strategic Agent only)**:
When switching roles within Strategic Agent:
1. Update .plan, .task, .review, or .report files as needed
2. Re-read the .task file and latest reports
3. Switch to new role and continue work
4. NO delegation prompt needed (same agent/model)
5. All progress preserved in task file

## Code Block Delegation Format

**Template**:
````markdown
**Task**: [{TaskPath}]({TaskPath})  
**Role**: {Role} (see [Framework/{Prompt}.prompt.md](../Framework/{Prompt}.prompt.md))  
**Target Audience**: {Agent} ({Model})

{1-2 sentence context}
````

**Critical Rules**:
1. **Always use 4 backticks** (````markdown ... ````) for delegation blocks
2. **Always specify exact model** (never use `<ModelName>` placeholder)
3. **Include full relative paths** to task and prompt files
4. **Keep context brief** (1-2 sentences max)

## Examples

### Strategic Agent → Implementation Agent (Plan Review)

````markdown
**Task**: [Framework/LegacyIntegration.task](Framework/LegacyIntegration.task)  
**Role**: Implementer (Plan Review - Iteration 1) (see [Framework/Implementation.prompt.md](Framework/Implementation.prompt.md))  
**Target Audience**: Implementation Agent (GPT-5.1-Codex-Max)

Review architecture for integrating legacy patterns - validate approach and provide sign-off.
````

### Implementation Agent → Strategic Agent (Planner Validation)

````markdown
**Task**: [Framework/LegacyIntegration.task](Framework/LegacyIntegration.task)  
**Role**: Planner (Validation) - Iteration 1 Response (see [Framework/Strategic.prompt.md](Framework/Strategic.prompt.md))  
**Target Audience**: Strategic Agent (Claude Sonnet 4.5)

## Iteration 1 Review

**Approval Status**: ✅ APPROVED

**Rationale**: Architecture is sound, implementation plan is clear.
````

### Implementation Agent → Strategic Agent (Review)

````markdown
**Task**: [Contracts/ContractValidation.task](Ai/Contracts/ContractValidation.task)  
**Role**: Reviewer (see [Framework/Strategic.prompt.md](Framework/Strategic.prompt.md))  
**Target Audience**: Strategic Agent (Claude Sonnet 4.5)

Implementation complete - all deliverables met, tests passing, ready for review.
````

## Model-Role Mapping Reference

| Agent | Model | Roles |
|-------|-------|-------|
| Strategic Agent | Claude Sonnet 4.5 | Orchestrator, Planner, Reviewer |
| Implementation Agent | GPT-5.1-Codex-Max | Implementer (Plan Review + Implementation) |

## Pre-Delegation Checklist (Orchestrator Only)

Before delegating a task, verify:
- [ ] Task status is "Not Started" or "Planning Complete"
- [ ] Task is not referenced in any open PR or PR draft
- [ ] No blocking dependencies on other in-progress tasks

## Common Mistakes

❌ **Using model placeholder**:
```markdown
**Target Audience**: Implementation Agent (<ModelName>)
```

✅ **Correct - specify model**:
```markdown
**Target Audience**: Implementation Agent (GPT-5.1-Codex-Max)
```

❌ **Missing code block fences**:
Just writing delegation text without backticks

✅ **Correct - wrapped in code block**:
````markdown
{delegation content}
````

❌ **Wrong number of backticks** (using 3 instead of 4):
```markdown
{delegation content}
```

✅ **Correct - 4 backticks for nested code**:
````markdown
{delegation content}
````

## Why Code Block Format?

**Purpose**: Enables user to copy/paste delegation when:
- Current model cannot assume delegated role
- User needs to switch models manually
- Workflow requires explicit model invocation

**Visual Cue**: Code block makes it obvious this is a delegation template, not executed work.

## Integration with Other Files

This delegation format is referenced in:
- [Framework/Flow.prompt.md](Flow.prompt.md) - Model mismatch handling
- [Framework/Strategic.prompt.md](Strategic.prompt.md) - Delegation examples
- [Framework/Implementation.prompt.md](Implementation.prompt.md) - Delegation examples
- [Framework/copilot-instructions.md](copilot-instructions.md) - Delegation format section
- [Planning/Task.task.template](../Planning/Task.task.template) - Completion delegation

**See**: [Framework/ModelRequirements.md](ModelRequirements.md) for detailed model-role mapping.
