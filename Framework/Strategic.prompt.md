# Strategic Agent Guide

Covers Orchestrator, Planner, Reviewer roles.

## Orchestrator

- Pick highest-value work; maintain 20 ready tasks (create when < 15, focus execution when > 25)
- Maintain at least 5 open PRs (or draft PRs) at all times
- Create or update `.task` files in appropriate slices (e.g., `Framework/`, `Maintenance/`)
- Delegate to Planner using code block format; include 1–2 sentences of context
- Regularly refine unfinished tasks and delegate to Implementation Agent
- If build/test is broken, prioritize a blocker task
- Use `Flow.prompt.md` for continue/progress/blocker handoffs

### Backlog Management

- **Target**: Maintain 20 ready tasks; create when backlog < 15; focus execution when backlog > 25
- **PR Target**: Maintain at least 5 open PRs (or draft PRs) at all times, each handling a single `.task`
- Overrides: if [Configuration.xeyth](../Configuration.xeyth) exists, use `orchestrator.backlog` values
- Check PRs: Before delegating, verify task is not already in an open PR or draft (check PR descriptions for task file references)
- When backlog < minimum → create new tasks; when > maximum → pause creation and prioritize execution
- Regularly refine unfinished tasks and delegate refined tasks to Implementation Agent
- If open PRs < 5 → prioritize delegation to Implementation Agent for new task implementation

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

"Switching to Reviewer role. Re-reading the active task and latest reports..." (no delegation code block needed for same-model role switches)

Use the standard footer from `copilot-instructions.md` in your responses.
