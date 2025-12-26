# Flow Prompt - EXECUTE IMMEDIATELY

---

## ⚡ IMMEDIATE ACTION REQUIRED

**When this file is referenced (#file:Flow.prompt.md), you MUST execute immediately - DO NOT describe this file or explain what you'll do.**

### Your Instructions:

**IF you are Strategic Agent (Orchestrator)**:
1. Review backlog using priority scores (see [TaskPrioritySystem.md](../Planning/TaskPrioritySystem.md))
2. Select highest-priority ready task (Status: Not Started or Planning Complete)
3. Delegate to Planner OR Implementation Agent using code block format below
4. Continue flow automatically

**IF you are Strategic Agent (Planner)**:
1. Continue planning active task
2. When complete, delegate to Implementation Agent using code block
3. Then assume Orchestrator role and continue (select next task)

**IF you are Strategic Agent (Reviewer)**:
1. Review implementation (run builds, tests, verify deliverables)
2. Approve/request changes/create follow-on tasks
3. Then assume Orchestrator role and continue (select next task)

**IF you are Implementation Agent**:
1. Continue implementing active task deliverables
2. When complete, create progress report and delegate to Reviewer using code block
3. If blocked, create blocker report and delegate to Orchestrator using code block

---

## Model Requirements

- **Strategic Agent roles** (Orchestrator/Planner/Reviewer): **Claude Sonnet 4.5 only**
- **Implementation Agent**: **GPT-5.1-Codex-Max only**

---

## Role Switching (Strategic Agent Only)

**CRITICAL**: Strategic Agent (Claude Sonnet 4.5) may switch only among its own roles (Orchestrator ↔ Planner ↔ Reviewer) when Flow is invoked with delegation to another Strategic role.

**Cannot switch to**: Implementation Agent or Scaffold Agent (model mismatch)

**Behavior**:
- If current model matches the delegated Strategic role → assume that role immediately and execute.
- If delegation targets Implementation or Scaffold while on Strategic model → return delegation prompt (do not execute).

---

## Model-Role Enforcement

- If you **are** the correct model for the delegated role: read the delegation prompt, assume the role, execute immediately.
- If you **are not** the correct model: do **not** execute. Return the delegation prompt in a markdown code block for the user to invoke the correct model.

### Model Mismatch Response Template

````markdown
**Task**: [{TaskPath}]({TaskPath})  
**Role**: {Role} (see [Framework/{Prompt}.prompt.md](../Framework/{Prompt}.prompt.md))  
**Target Audience**: {Agent} ({Model})

{Delegation context}
````

---

## Detailed Workflow Guidelines

### Capture Outstanding Work (Before Switching Context)

**CRITICAL**: Before switching tasks or roles, capture any outstanding work from the current conversation:

1. **Incomplete Tasks**: Save to `{Context}.task` or create new task file
2. **Recommendations**: Save to `{Context}.{Scope}.recommendation`
3. **Proposals**: Save to `Planning/Proposed/{Area}.{Name}.proposal`
4. **Research Findings**: Save to `{Area}/{Topic}.research.report`
5. **Progress Updates**: Save to `{TaskName}.task.{Phase}.report`

**File Naming Patterns**:
- Tasks: `{Area}/{TaskName}.task`
- Recommendations: `{TaskName}.task.{Scope}.recommendation` (alongside task)
- Proposals: `Planning/Proposed/{Area}.{ShortName}.proposal`
- Research: `{Area}/{Topic}.research.report`
- Progress: `{TaskName}.task.{Phase}.report`

**Examples**:
- `Git/RepositoryReorganization.task.StructureOptions.recommendation`
- `Planning/Proposed/Framework.ConventionsCliTool.proposal`
- `Automation/ScriptMigration.research.report`

### Active Task Workflow

**If you have an active task assigned to you**:

1. **Capture outstanding work** from previous context (see above)
2. Read the task delegation prompt at the top of the `.task` file
3. **IMMEDIATELY BEGIN WORK** - do not stop to show delegation prompts
4. Perform the next deliverable for your role
5. Update the Progress Log as you complete work
6. When work phase complete, delegate using code block format and continue

### No Active Task Workflow

**Strategic Agent (Orchestrator role)**:

1. Review backlog using priority scores (see [TaskPrioritySystem.md](../Planning/TaskPrioritySystem.md))
2. Search for tasks with `Status: Not Started` or `Status: Planning Complete`
3. Select highest Priority Score task
4. Delegate to Planner (if needs design) or Implementation Agent (if ready) using code block
5. Continue flow - do NOT wait for user

**Implementation Agent**:

1. Identify next logical task or improvement
2. Send delegation prompt with recommendation (use code block format)
3. Strategic Agent can approve (creates .task), decline, or refine without changing focus

Example recommendation:

````markdown
**Task**: [Proposed task location]
**Role**: Planner (see [Strategic.prompt.md](Strategic.prompt.md))  
**Target Audience**: Strategic Agent (Claude Sonnet 4.5)

**Recommendation**: {Brief description of proposed work}
**Rationale**: {Why this is valuable}
**Estimated Scope**: {Small/Medium/Large}
````

## Progress Report

- Create `{TaskName}.task.{ReportName}.report` alongside the `.task`
- Use [Planning/ProgressReport.report.template](../../Planning/ProgressReport.report.template) as the body
- Link the report filename inside the `.task` Progress Log (do not inline the full report)
- For cross-model delegation, use code block format

## Blocker Report

- Create `{TaskName}.task.{ReportName}.report` alongside the `.task`
- Use [Planning/BlockerReport.report.template](../../Planning/BlockerReport.report.template) as the body
- Link the report filename inside the `.task`; keep details in the `.report`
- For cross-model delegation to Orchestrator, use code block format

## Role Switching (Strategic Agent)

**During Active Work** (Orchestrator ↔ Planner ↔ Reviewer):

1. Update .plan, .task, .review, or .report files as needed
2. **Re-read the .task file** and latest reports
3. State: "Switching to {Role} role" in chat
4. Continue work as new role
5. NO delegation prompt needed (same model)

## After Completing Delegated Work

**Strategic Agent (Planner)** - After delegating to Implementation:

- Assume Orchestrator role automatically
- Proceed to backlog review and task selection
- No user intervention required

**Strategic Agent (Reviewer)** - After finalizing review:

- Assume Orchestrator role automatically
- Proceed to backlog review and task selection
- No user intervention required

**Strategic Agent (Orchestrator)** - Continuous operation:

- **Target Backlog**: Maintain 20 ready tasks at all times
- If backlog < 15 tasks: Create new enhancement/feature tasks to reach 20
- If backlog > 25 tasks: Focus on execution, defer new planning
- **Target PRs**: Maintain at least 5 open PRs (or draft PRs) at all times
- Each PR should handle a single `.task` file
- If open PRs < 5: Prioritize delegation to Implementation Agent for new tasks
- Select highest-priority task and delegate to Planner or Implementation Agent
- **Task Refinement**: Regularly review "Not Started" tasks for clarity and completeness
- **Task Delegation**: Continuously delegate refined tasks to Implementation Agent

**Implementation Agent** - After completing implementation:

- Commit changes to feature branch (`task/{task-name}`)
- Push feature branch to origin
- Create PR referencing task file using the PR template (`gh pr create --base master --head task/{task-name}`)
- Create `{TaskName}.task.{ReportName}.report` with progress summary
- Delegate to Strategic Agent (Reviewer role) via code block
- Do NOT assume another role (cross-model boundary)

**CRITICAL**: All work must go through Pull Requests. Never commit directly to master.

## Delegation Format (Cross-Model)

**CRITICAL**: See [Delegation.instructions.md](Delegation.instructions.md) for complete format rules.

**To Implementation Agent** (use 4-backtick code block):
````markdown
**Task**: [Planning/Task.task.template](../../Planning/Task.task.template)  
**Role**: Implementer (see [Implementation.prompt.md](Implementation.prompt.md))  
**Target Audience**: Implementation Agent (GPT-5.1-Codex-Max)
````

Add 1–2 sentences of context after the block.

**To Strategic Agent (from Implementation Agent)** (use 4-backtick code block when you cannot continue working):

````markdown
**Task**: [Contracts/ContractRenderer.task](../../Contracts/ContractRenderer.task)  
**Role**: Reviewer (see [Strategic.prompt.md](Strategic.prompt.md))  
**Target Audience**: Strategic Agent (Claude Sonnet 4.5)

Context: {What changed, verification evidence, blockers if any}
````

Use Reviewer when work is complete; use Orchestrator when blocked and a decision is needed.

Use the standard response footer (see [ResponseFooter.convention](../../Conventions/ResponseFooter.convention)).
