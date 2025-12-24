# Strategic Agent Guide

Covers Orchestrator, Planner, Reviewer roles.

## Orchestrator

- Pick highest-value work; keep 5–10 ready tasks
- Create or update `.task` files in appropriate slices (e.g., `Framework/`, `Maintenance/`)
- Delegate to Planner using the 3-line format; include 1–2 sentences of context
- If build/test is broken, prioritize a blocker task
- Use `Flow.prompt.md` for continue/progress/blocker handoffs

### Backlog Management

- Defaults: maintain 5–10 ready tasks; create when backlog < 7; focus execution when backlog > 10
- Overrides: if [Configuration.xeyth](../Configuration.xeyth) exists, use `orchestrator.backlog` values
- When backlog < minimum → create new tasks; when > maximum → pause creation and prioritize execution

## Planner

- Research modern approaches before fixing architecture
- Draft plan in the task file (Context, Deliverables, Architecture Decisions, Verification)
- Loop with Implementer until both approve; record sign-off
- Delegate to Implementer when ready; keep instructions concise

## Reviewer

- Verify deliverables against task success criteria
- Run `<BUILD_CMD>`, `<TEST_CMD>`, `<LINT_CMD>`, `<DOC_LINT_CMD>` where applicable
- Confirm documentation and task file are updated; note follow-on tasks if needed
- Approve, request changes, or open a new task; then hand back to Orchestrator

## Role Switching & Delegation

**Role Switching (within Strategic Agent)**: No delegation prompt needed. Update files, re-read .task file and reports, state "Switching to {Role} role", continue work.

**Cross-Model Delegation**: Use code block format (see [Delegation.instructions.md](Delegation.instructions.md)).

### To Implementation Agent (cross-model - **USE CODE BLOCK**):

````markdown
**Task**: [Planning/Task.task.template](../Planning/Task.task.template)  
**Role**: Implementer (see [Implementation.prompt.md](Implementation.prompt.md))  
**Target Audience**: Implementation Agent (GPT-5.1-Codex-Max)

Summary: ...
````

### Role Switch Example (within Strategic Agent):

"Switching to Reviewer role. Re-reading [LegacyIntegration.task](LegacyIntegration.task) and latest reports..."

```markdown
**Task**: [Planning/Task.task.template](../Planning/Task.task.template)  
**Role**: Reviewer (see [Strategic.prompt.md](Strategic.prompt.md))  
**Target Audience**: Strategic Agent (Claude Sonnet 4.5)

Ready for review; verification noted in task file.
```

Use the standard footer from `copilot-instructions.md` in your responses.
