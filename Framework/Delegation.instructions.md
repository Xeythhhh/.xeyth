---
applyTo: "**"
---

---
applyTo: "**"
---

# Delegation Format Instructions

When delegating work to another role, use the **code block delegation format**.

## Code Block Delegation Format

**Template**:
````markdown
**Task**: <a href="{TaskPath}">{TaskPath}</a>  
**Role**: {Role} (see <a href="../Framework/{Prompt}.prompt.md">Framework/{Prompt}.prompt.md</a>)

{1-2 sentence context}
````

**Rules**:

1. **Use 4 backticks** (````markdown ... ````) for delegation blocks
2. **Include full relative paths** to task and prompt files
3. **Keep context brief** (1-2 sentences max)
4. **Specify role** (Orchestrator, Planner, Reviewer, Implementer)

## Examples

## Examples

### Planner → Implementer

````markdown
**Task**: <a href="../Automation/Feature.task">Automation/Feature.task</a>  
**Role**: Implementer (see <a href="Implementation.prompt.md">Framework/Implementation.prompt.md</a>)

Implement the approved architecture for the automation feature.
````

### Implementer → Reviewer

````markdown
**Task**: <a href="../Automation/Feature.task">Automation/Feature.task</a>  
**Role**: Reviewer (see <a href="Strategic.prompt.md">Framework/Strategic.prompt.md</a>)

Implementation complete - all deliverables met, tests passing, ready for review.
````

## Role Switching

When switching roles (Orchestrator ↔ Planner ↔ Reviewer):

1. Update task/plan/report files as needed
2. Re-read the task file and latest reports
3. State role switch in response
4. Continue work as new role
5. NO delegation prompt needed

## Pre-Delegation Checklist (Orchestrator Only)

Before delegating a task, verify:

- [ ] Task status is "Not Started" or "Planning Complete"
- [ ] Task is not referenced in any open PR or PR draft
- [ ] No blocking dependencies on other in-progress tasks
