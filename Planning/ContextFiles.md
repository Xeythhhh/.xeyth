# Context Files

Context files are structured markdown files with specific extensions that serve as data containers for the AI framework. They are treated as markdown via workspace settings for syntax highlighting and editing convenience.

## Supported Context File Types

### `.task` - Task Definition Files

**Purpose**: Define work items with delegation prompts, deliverables, and progress tracking.

**Location**: Alongside related code in the appropriate slice (e.g., `Framework/`, `Maintenance/`)

**Schema**: See [Task.task.template](Task.task.template)

**Key Sections**:

- Delegation Prompt (role, objective, deliverables)
- Task Details (status, owner, priority)
- Architecture Decisions
- Success Criteria
- Progress Log

**Naming Convention**: `{TaskName}.task`

---

### `.plan` - Plan Context Files

**Purpose**: Document architectural decisions, implementation approach, and verification criteria for complex tasks.

**Location**: Created alongside related `.task` files

**Schema**: See [Plan.plan.template](Plan.plan.template)

**Key Sections**:

- Objective & Context
- Decisions with Rationale
- Implementation Notes
- Verification Checklist

**Naming Convention**: `{TaskName}.plan` or `{FeatureName}.plan`

---

### `.report` - Progress & Blocker Reports

**Purpose**: Capture status updates, verification evidence, or blocker details without cluttering task files.

**Location**: Created alongside the related `.task` file

**Schema**: See [ProgressReport.report.template](ProgressReport.report.template) or [BlockerReport.report.template](BlockerReport.report.template)

**Key Sections** (Progress):

- Summary of updates
- Deliverable status
- Verification evidence
- Next steps

**Key Sections** (Blocker):

- Blocker description
- Impact & blocked deliverables
- Attempted solutions
- Recommendation

**Naming Convention**: `{TaskName}.{ReportName}.report` (e.g., `SetupCI.Progress1.report`, `AuthFlow.Blocker.report`)

---

### `.template` - Template Files

**Purpose**: Provide scaffolding for creating new context files with standardized structure.

**Location**: `Planning/` directory in the framework

**Types**:

- `Task.task.template`
- `Plan.plan.template`
- `ProgressReport.report.template`
- `BlockerReport.report.template`

**Usage**: Copy template, rename with appropriate extension, fill in placeholders

---

### `.convention` - Convention Files

**Purpose**: Define enforceable standards and rules that apply across the codebase (commit messages, code style, naming conventions, architectural patterns).

**Location**: Placed in the relevant slice that owns the convention (e.g., `Git/` for commit conventions, `Framework/` for prompt conventions)

**Key Sections**:

- Format/Structure specification
- Rules and requirements
- Examples (good and bad)
- Validation checklist
- Tool recommendations

**Naming Convention**: `{ConventionName}.convention` (e.g., `Commit.convention`, `Naming.convention`, `CodeStyle.convention`)

**Characteristics**:

- **Prescriptive**: Defines what must be done, not just what is done
- **Enforceable**: Clear rules that can be validated (manually or with tools)
- **Actionable**: Provides examples and validation steps
- **Discoverable**: Referenced in delegation prompts and task files

**Example**: `Git/Commit.convention` - Conventional Commits 1.0.0 specification for commit message format

---

### `.inventory` - Inventory Files

**Purpose**: Track collections of related context files (plans, tasks, conventions) for auditing, automation, and discovery.

**Location**:

- Framework-level: `Planning/Plans.inventory` (tracks all active `.plan` files only)
- Scoped to context file: `{Name}.{ContextFileType}.{Scope}.inventory` (e.g., `RefactorPrompts.plan.Tasks.inventory`)

**Format**: Line-based list of file paths (relative to repository root)

**Key Characteristics**:

- **Line-based**: One file path per line for easy parsing
- **Relative paths**: Paths relative to Ai/ directory (e.g., `Framework/InventorySystem.task`)
- **Comments supported**: Lines starting with `#` are comments
- **Automation-friendly**: Simple format for scripts and tooling
- **Audit trail**: Tracks relationships between context files explicitly

**Naming Convention**:

- `Plans.inventory` - Framework-level tracking of `.plan` files only
- `{Name}.plan.Tasks.inventory` - Tasks related to a specific plan
- `{Name}.{ContextFileType}.{Scope}.inventory` - General pattern for scoped inventories

**Structure**:

```markdown
# {Inventory Name}

## {Optional Section Header}

{Slice}/{File}.{extension}
{Slice}/{File}.{extension}

## {Another Section}

{Slice}/{File}.{extension}
```

**Example** (`Planning/Plans.inventory`):

```markdown
# Active Plans

## Framework Plans

Framework/RefactorPrompts.plan

## Git Plans

(No active plans yet)
```

**Example** (`Framework/RefactorPrompts.plan.Tasks.inventory`):

```markdown
# Refactor Prompts Plan - Tasks

## Implementation Tasks

Framework/UpdateStrategic.task
Framework/UpdateImplementation.task
Framework/UpdateScaffold.task
```

**Usage**:

1. **Plan Creation**: When creating a `.plan` file, also create `{PlanName}.plan.Tasks.inventory`
2. **Task Addition**: Add new tasks to the appropriate `.plan.Tasks.inventory` file
3. **Archiving**: Move completed context files to slice-level `archive/` directory and update inventories
4. **Discovery**: Automation tools can read inventories to find related work

**Scoped Naming Pattern**: `{Name}.{ContextFileType}.{Scope}.{extension}`

Examples:

- `RefactorPrompts.plan.Tasks.inventory` - Tasks for a plan
- `APIv2.task.Progress.report` - Progress report for a task
- `Authentication.plan.Decisions.report` - Decision report for a plan

---

### `.metadata` - Contract Schema Files

**Purpose**: Define structure, validation rules, and conventions for context file types using YAML. The automation tool discovers these contracts at runtime to validate context files.

**Location**: Next to the template they describe (e.g., `Planning/Task.template.metadata`)

**Schema**: YAML format with sections for target patterns, schema definition, naming conventions, archiving rules, and validation rules.

**Key Sections**:

- `target`: Glob patterns specifying which files this contract applies to
- `schema`: Required sections and fields with validation patterns
- `naming`: File naming conventions and patterns
- `archiving`: Archiving directory and naming rules
- `relatedFiles`: Associated files (reports, inventories)
- `validation`: Custom validation rules
- `meta`: Contract metadata (version, author, description)

**Naming Convention**: `{ContextFileType}.{extension}.metadata` (e.g., `Task.template.metadata`, `Convention.convention.metadata`)

**Discovery**: The automation CLI tool discovers all `.metadata` files in the framework (shipped defaults) and user repositories (custom extensions), then merges them to create a complete validation contract set.

**Example**: See [Task.template.metadata](Task.template.metadata) for the task file contract definition.

---

### `.workflow` - Workflow Definition Files

**Purpose**: Document multi-stage workflows, processes, and procedures that guide agent and developer actions across different phases of work.

**Location**: `Planning/` directory or co-located with related work in appropriate slices

**Schema**: See [Workflow.workflow.template](Workflow.workflow.template)

**Key Sections**:

- Workflow Overview
- Stage Definitions (triggers, processes, outputs)
- Quality Standards
- Integration Points
- Metrics & Monitoring
- Best Practices

**Naming Convention**: `{WorkflowName}.workflow` (e.g., `TaskRefinement.workflow`, `ReleaseProcess.workflow`, `CodeReview.workflow`)

**Characteristics**:

- **Multi-stage**: Defines multiple connected stages or phases
- **Prescriptive**: Provides step-by-step guidance for each stage
- **Actionable**: Includes triggers, processes, and expected outputs
- **Measurable**: Specifies metrics and success criteria
- **Discoverable**: Referenced in task files, conventions, and documentation

**Example**: `Planning/TaskRefinement.workflow` - 6-stage workflow from task creation through implementation to archival

---

### `.xeyth` - Framework Configuration Files

**Purpose**: Provide optional configuration overrides for the AI framework (e.g., backlog thresholds, Verify diff tool, archiving defaults, model labels).

**Location**: `Ai/Configuration.xeyth` in the framework root.

**Format**: YAML

**Behavior**:

- Optional; framework falls back to built-in defaults when absent
- Consumed by prompts and automation to tune orchestrator backlog and conventions

---

## File Association

Context files are configured as markdown in the workspace settings (`.code-workspace`):

```json
{
  "files.associations": {
    "*.task": "markdown",
    "*.plan": "markdown",
    "*.report": "markdown",
    "*.template": "markdown",
    "*.convention": "markdown",
    "*.inventory": "markdown",
    "*.workflow": "markdown",
    "*.metadata": "yaml",
    "*.xeyth": "yaml"
  }
}
```

This enables:

- Syntax highlighting
- Markdown preview
- IntelliSense and formatting tools
- Consistent editing experience

---

## Workflow Integration

1. **Task Creation**: Copy `Task.task.template`, rename to `{TaskName}.task`, place in relevant slice
2. **Planning**: Create `{PlanName}.plan` if architecture decisions are needed; also create `{PlanName}.plan.Tasks.inventory`
3. **Progress Tracking**: Create `{TaskName}.task.{ReportName}.report` for updates; link in task Progress Log
4. **Archiving**: Move completed context files to slice-level `archive/` subdirectory (e.g., `Framework/archive/`, `Git/archive/`)

---

## Archiving Convention

**Structure**: Each slice maintains an `archive/` subdirectory for completed work

**Examples**:

- `Framework/archive/` - Archived framework tasks, plans, and reports
- `Git/archive/` - Archived git-related work
- `Planning/archive/` - Archived framework-level planning artifacts

**Process**:

1. Mark context file as complete in its Progress Log or status
2. Move the context file and all related files (inventories, reports) to `{Slice}/archive/`
3. Update parent inventories (e.g., remove from `Plans.inventory` or plan's `.plan.Tasks.inventory`)
4. Optionally add archival date to filename: `{Name}.{Type}` → `archive/{Name}.YYYY-MM-DD.{Type}`

**Retention**: Keep `archive/` directories versioned in git for historical reference and audit trail

---

## Best Practices

- **Keep reports separate**: Don't inline full reports into task files; link to `.report` files instead
- **Atomic commits**: Commit context file changes together with related code changes
- **Naming clarity**: Use descriptive, concise names that indicate the task/feature
- **Version control**: Track context files in git alongside code
- **Cleanup**: Archive completed tasks regularly to keep active work visible
