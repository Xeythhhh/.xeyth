# Copilot Instructions (Generic)

**Purpose**: Lightweight, reusable agent system for any project. Project specifics remain as placeholders to be filled by the Cleanup role.

**Integration**: This file provides the generic framework. Host repositories create a project-specific `.github/copilot-instructions.md` that references this file and adds local context. See [InitializeAiFramework.prompt.md](../Maintenance/InitializeAiFramework.prompt.md) for setup instructions.

## Roles

- **Strategic Agent** (Orchestrator/Planner/Reviewer): select work, design plans, review outcomes
  - **Model Requirement**: Claude Sonnet 4.5 only
- **Implementation Agent**: plan review + implementation in one flow
  - **Model Requirement**: GPT-5.1-Codex-Max only
- **Scaffold Agent**: boilerplate generation only
- **Cleanup Agent**: replace placeholders with project values and trim unused sections

## Core Rules

- Keep work local; do not push/pull/publish
- Create and maintain tasks in appropriate slices (e.g., `Framework/`, `Maintenance/`); archive completed tasks
- Use concise, actionable writing; avoid duplication across files
- Prefer deterministic automation; document anything that affects reproducibility
- Use `Flow.prompt.md` for continue/progress/blocker actions

**Integration Model**: 
- **Generic (this file)**: Roles, workflows, conventions — consumed by all projects
- **Project-specific** (`.github/copilot-instructions.md` in host repo): Quality gates, tech stack, domain rules
- **Precedence**: When conflicts arise, project-specific rules override generic defaults

## Configuration Overrides

- Optional: [Configuration/Configuration.xeyth](../../Configuration/Configuration.xeyth) to override defaults (orchestrator backlog thresholds, Verify diff tool, archiving format, model labels)
- If the file is absent, built-in defaults apply (backlog 5–10, archive directory `archive`, date format `yyyy-MM-dd`)

## Quality Bar (fill via Cleanup)

**CRITICAL**: All work must pass quality gates before commit.

- Builds: `<BUILD_CMD>` (0 errors, 0 warnings - no exceptions)
- Tests: `<TEST_CMD>` (0 failures, 0 skips unless justified in task)
- Lint: `<LINT_CMD>` and `<DOC_LINT_CMD>` (0 errors, 0 warnings - no exceptions)
- Contracts: `xeyth-contracts validate --strict` (if applicable)
- Platform focus: `<PRIMARY_ENV>` (secondary: `<SECONDARY_ENV>`, if any)

**Diagnostic Resolution**:
- Errors: MUST be fixed (no exceptions)
- Warnings: MUST be fixed unless documented as blocker
- Unresolvable diagnostics: Create blocker report, document in task, reference in commit
- See [Implementation.prompt.md](Implementation.prompt.md) for workflow

## Planning & Sign-off

- Each task has a top-level delegation prompt (see Task.task.template)
- Planner and Implementer iterate until both approve; record sign-off in the task file
- Mirror decisions into relevant docs in the host repo

## Continuous Workflow

Agents operate in **continuous flow mode**:
1. Complete assigned work
2. Create progress/blocker reports as `{TaskName}.task.{ReportName}.report` files alongside `.task`
3. Delegate to next role via code block (cross-model) or role switch (same model)
4. **Strategic Agent**: After delegation, assume Orchestrator role and select next task
5. Repeat until stop condition reached

**Backlog Management** (Orchestrator):
- Maintain 5-10 ready tasks at all times
- If backlog < 5: Create new enhancement/feature tasks
- If backlog > 10: Focus on execution, defer new planning

**Stop Conditions**:
- User explicitly requests halt
- Blocker requires external/user input  
- Backlog empty and no new work identified

## Delegation Format

**CRITICAL**: See [Delegation.instructions.md](Delegation.instructions.md) for complete delegation format rules and examples.

**Key Rules**:
- Cross-model delegation (Strategic ↔ Implementation): **Use 4-backtick code block**
- Same-model delegation (Strategic role switching): Regular 3-backtick format
- Always specify exact model (never `<ModelName>` placeholder)

**Model Requirements**:
- Strategic roles (Orchestrator/Planner/Reviewer): Claude Sonnet 4.5
- Implementation role: GPT-5.1-Codex-Max

## Response Footer (standard)

```
------------------------------------------------

### Affected files

**Added**: {List or None}  
**Modified**: {List or None}  
**Deleted**: {List or None}

------------------------------------------------

> **Task**: {TaskFile.task}
> **Role**: `{AssignedRole}`
> **Agent Identifier**: `{AgentId}`
> **Target Audience**: `{TargetRole} ({Model})`

------------------------------------------------
```

Keep files short; extend in the host repo if stricter rules are required. Context files (`.task`, `.plan`, `.report`, `.template`, `.convention`, `.inventory`) are markdown—set file associations in your workspace settings.
