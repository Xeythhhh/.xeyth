# AI Instruction Framework (Submodule Ready)

A compact, role-based instruction system you can vend as a git submodule under `.xeyth`, with the framework slice in `.xeyth/Ai/`. It keeps agents aligned while leaving project specifics as placeholders to be filled by a cleanup pass.

## What This Provides

- Standard roles:
  - **Strategic** (Orchestrator/Planner/Reviewer) - Claude Sonnet 4.5 only
  - **Implementation** - GPT-5.1-Codex-Max only
  - **Scaffold** - Boilerplate generation
  - **Cleanup** - Replace placeholders
- Self-contained task files with delegation prompts
- Single flow prompt for continue/progress/blocker
- Placeholder-based quality gates (build/test/lint) to be project-filled
- Context files (.task, .plan, .template) are markdown; configure your editor (see Maintenance/InitializeAiFramework.prompt.md).

## Quick Start

1. **Add the submodule**: `git submodule add <repo-url> .xeyth` (or use existing `.xeyth` submodule)
2. **Create integration file**: Add `.github/copilot-instructions.md` in your host repo root that references `.xeyth/Framework/copilot-instructions.md`
3. **Configure workspace**: Open `.xeyth.code-workspace` or add file associations for `*.task`, `*.plan`, `*.report`, `*.convention`, `*.inventory`, `*.template` → markdown
4. **Create your first task**: Use `Planning/Task.task.template` as a starting point
5. **Delegate work**: Reference roles in `Framework/` (Strategic.prompt.md, Implementation.prompt.md, etc.)

See [Maintenance/InitializeAiFramework.prompt.md](Maintenance/InitializeAiFramework.prompt.md) for detailed integration steps.

## How to Integrate

1. Add as submodule (example): `git submodule add <your-repo-url> .xeyth`
2. Open the workspace using `.xeyth/Ai/.code-workspace` to enable context file associations
3. Create `.github/copilot-instructions.md` in host repo root (see `Maintenance/InitializeAiFramework.prompt.md` for template)
4. Run Cleanup to replace placeholders (project name, commands, paths, tooling)
5. Run `Maintenance/InitializeAiFramework.prompt.md` to validate setup
6. Keep the instruction files versioned with the host repo; treat this submodule as read-only for consumers

## Placeholder Keys

- `<PROJECT_NAME>`, `<CODEBASE_ROOT>`, `<TECH_STACK>`
- `<BUILD_CMD>`, `<TEST_CMD>`, `<LINT_CMD>`, `<DOC_LINT_CMD>`
- `<PRIMARY_ENV>` (e.g., Windows/Linux/macOS), `<SECONDARY_ENV>` if any
- `<OWNER>` (team/lead), `<REPO_URL>`

## File Map

- `Framework/copilot-instructions.md`: Core rules and quality bar
- `Framework/Strategic.prompt.md`: Orchestrator/Planner/Reviewer workflows
- `Framework/Implementation.prompt.md`: Plan review + implementation workflow
- `Framework/Scaffold.prompt.md`: Boilerplate execution guidance
- `Framework/Cleanup.prompt.md`: Broad cleanup guidance (placeholders + stray file/layout fixes)
- `Framework/Flow.prompt.md`: Continue/progress/blocker flow
- `Framework/Integration.md`: Submodule usage and customization
- `Planning/Task.task.template`: Standard task scaffold (context file)
- `Planning/*.plan.template`: Plan context templates
- `Planning/*.report.template`: Report/blocker templates
- `Planning/ContextFiles.md`: Documentation of all context file types and schemas
- `Planning/Plans.inventory`: Framework-level inventory of active `.plan` files
- `Maintenance/InitializeAiFramework.prompt.md`: One-time init instructions
- `.code-workspace`: VS Code workspace with context file associations

## Context File Naming Conventions

**Scoped Files Pattern**: `{Name}.{ContextFileType}.{Scope}.{extension}`

Examples:

- `RefactorPrompts.plan.Tasks.inventory` - Tasks inventory for a plan
- `APIv2.task.Progress.report` - Progress report for a task  
- `Authentication.plan.Decisions.report` - Decision report for a plan

## Archiving

Completed context files move to slice-level `archive/` directories:

- `Framework/archive/` - Archived framework work
- `Git/archive/` - Archived git-related work
- `{Slice}/archive/` - Each slice maintains its own archive

Keep files short and focused; extend in the host repo, not here.
