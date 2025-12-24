# Legacy System Analysis & Adoption Proposal

**Date**: 2024-12-24  
**Purpose**: Analyze legacy Balhacii instruction system to identify valuable patterns for .xeyth framework

---

## Key Findings from Legacy System

### 1. Proceed.prompt.md (Flow Mechanism)

**Current Capability**:
- Single prompt for "continue work"
- Role-specific behavior sections (Orchestrator/Planner/Reviewer/Implementer/Scaffold)
- **Role switching for Strategic Agent**: Can switch between its 3 roles (Orchestrator/Planner/Reviewer) seamlessly
- **Model-role enforcement**: Different agents use different models

**Key Quote**:
> "Strategic Agent Role Switching: When a Strategic Agent delegates to another Strategic Agent role (Orchestrator → Planner, Planner → Reviewer) and `Proceed.prompt.md` is invoked, assume the requested role immediately and fulfill the request without interrupting execution."

**Critical Constraint**:
> "CRITICAL: Strategic Agent can ONLY switch between its own 3 roles (Orchestrator/Planner/Reviewer). Strategic Agent can NEVER become Implementation Agent or Scaffold Agent."

**Model Mismatch Handling**:
- When wrong model invoked for a role, delegation prompt returned in code block (see screenshot)
- Agent doesn't execute, just shows delegation template for user to copy/paste

### 2. Delegation Format

**Simple 3-line syntax**:
```markdown
**Task**: [docs/planning/{TaskFile}.task](docs/planning/{TaskFile}.task)  
**Role**: {Role} (see [{RolePrompt}.prompt.md](.github/{RolePrompt}.prompt.md))  
**Target Audience**: {AgentRole} ({ModelName})

{1-2 sentence context}
```

**Always in code block when delegating to different agent type** (Strategic → Implementation, Implementation → Strategic)

### 3. Orchestrator Backlog Management

**Configured Size**: 5-10 pending tasks

**Quote**:
> "Proactive Planning: Strategic Agent (Orchestrator) should preemptively create `.task` files for future work (implementation, refactoring, research, tooling improvements) to maintain a ready queue (5-10 pending tasks)."

**Backlog Health Check** in Proceed.prompt.md:
- If backlog < 5 tasks, create new enhancement tasks
- If backlog > 10 tasks, focus on execution
- If no tasks available, report status to user

### 4. Configuration File Concept

**Not explicitly present in legacy**, but implied by:
- Hardcoded "5-10" backlog size
- Could be extracted to configuration

---

## Adoption Proposals

### Proposal 1: Enhanced Flow.prompt.md with Role Switching

**What to Add**:
1. **Model detection logic** in Flow.prompt.md:
   - If current model = Claude Sonnet 4.5 and delegated role = Strategic (Orchestrator/Planner/Reviewer) → **assume role and execute**
   - If current model = GPT-5.1-Codex-Max and delegated role = Implementer → **assume role and execute**
   - If model mismatch → **return delegation code block, don't execute**

2. **Explicit role-switching behavior**:
   ```markdown
   ## Role Switching (Strategic Agent Only)
   
   **CRITICAL**: Strategic Agent (Claude Sonnet 4.5) can switch between its 3 roles:
   - Orchestrator ↔ Planner ↔ Reviewer
   
   **Cannot switch to**:
   - Implementation Agent (requires GPT-5.1-Codex-Max)
   - Scaffold Agent
   
   When Flow is invoked with delegation to another Strategic role, assume that role immediately.
   ```

3. **Model mismatch response template**:
   ```markdown
   ## If Model Cannot Assume Delegated Role
   
   Return delegation prompt in code block for user to invoke correct model:
   
   ````markdown
   **Task**: [Contracts/ContractValidation.task](../Contracts/ContractValidation.task)
   **Role**: Implementer (see Framework/Implementation.prompt.md)
   **Target Audience**: Implementation Agent (GPT-5.1-Codex-Max)
   
   {Context from delegation}
   ````
   ```

**Benefit**: Enables smooth workflow when correct model is available, graceful degradation when wrong model

**Risk**: None - improves UX significantly

**Recommendation**: ✅ **ADOPT**

---

### Proposal 2: Orchestrator Backlog Configuration

**What to Add**:
1. Create `.xeyth` context file type for configuration
2. Add `Configuration/Configuration.xeyth` file:
   ```yaml
   orchestrator:
     backlog:
       minimum: 5
       maximum: 10
       createThreshold: 7  # Create new tasks when backlog drops below this
   
   testing:
     verify:
       diffTool: VisualStudioCodeInsiders
   
   conventions:
     archiving:
       dateFormat: "yyyy-MM-dd"
       directoryName: "archive"
   ```

3. Update Strategic.prompt.md Orchestrator section:
   ```markdown
   ### Backlog Management
   
   - Maintain {backlog.minimum}-{backlog.maximum} ready tasks (configured in Configuration.xeyth)
   - Create new tasks when backlog < {backlog.createThreshold}
   - Prioritize execution when backlog > {backlog.maximum}
   ```

4. Add `.xeyth` to workspace file associations

**Benefit**: Makes framework configurable for different team sizes/velocities

**Risk**: Adds complexity, might be overkill for small teams

**Recommendation**: ✅ **ADOPT** - makes framework more reusable

---

### Proposal 3: Task File Delegation Prompt Section

**Current State**: Task files don't have delegation prompts at top

**Legacy Pattern**: Every task has `---\n# DELEGATION PROMPT\n---` section at top with:
- Role assignment
- Objective
- Context
- Responsibilities
- Deliverables
- Implementation guidance
- Completion delegation

**What to Add**:
Update `Planning/Task.task.template` to include delegation prompt section at top:

```markdown
---
# DELEGATION PROMPT
---

## Role: {Orchestrator|Planner|Reviewer|Implementer}

**Task File**: {Slice}/{TaskName}.task  
**Objective**: {One-line objective}

**Role Reference**: See [Framework/{Role}.prompt.md](../Framework/{Role}.prompt.md)

### Context
{Why this exists}

### Your Responsibilities
1. {Responsibility}

### Deliverables
- [ ] **D1**: {Acceptance criteria}

### Upon Completion
{How to delegate to next role}

---
# TASK DETAILS
---

{Current task template content}
```

**Benefit**: Makes task files self-contained with clear agent instructions

**Risk**: Adds verbosity to task files

**Recommendation**: ✅ **ADOPT** - significantly improves clarity

---

### Proposal 4: Response Footer Template

**Current State**: Flow.prompt.md has footer template, but not as strict

**Legacy Pattern**: EVERY response must end with:
```markdown
------------------------------------------------

### Affected files
**Added**: {List or None}
**Modified**: {List or None}
**Deleted**: {List or None}

------------------------------------------------

> **Task**: {TaskFile.task}
> **Role**: `{Role}`
> **Agent Identifier**: `Copilot-{Model}-{Role}-{Feature}`
> **Target Audience**: `{TargetRole} ({TargetModel})`

------------------------------------------------
```

**What to Add**: Make this mandatory in all prompt files

**Benefit**: Consistent tracking, clear audit trail

**Risk**: Verbose, might be annoying for simple responses

**Recommendation**: ⚠️ **OPTIONAL** - useful for complex work, overkill for simple queries

---

## Implementation Priority

### Phase 1: Critical (Immediate)
1. ✅ **Enhanced Flow.prompt.md with role switching** (Proposal 1)
   - Add model detection and role-switching logic
   - Add model mismatch response template
   - Update delegation examples

2. ✅ **Configuration.xeyth file** (Proposal 2)
   - Create .xeyth context file type
   - Add Configuration/Configuration.xeyth with orchestrator backlog settings
   - Update workspace file associations

### Phase 2: High Priority (Next Session)
3. ✅ **Task file delegation prompt section** (Proposal 3)
   - Update Planning/Task.task.template
   - Migrate active tasks to new format
   - Update documentation

### Phase 3: Optional (Future)
4. ⚠️ **Strict response footer** (Proposal 4)
   - Evaluate in practice first
   - Only adopt if audit trail proves valuable

---

## Detailed Implementation Plan

### Implementation 1: Flow.prompt.md Enhancement

**Changes to Flow.prompt.md**:

```markdown
# Flow Prompt (Continue, Progress, Blocker)

**Model Requirement**: Use appropriate model for your role (Strategic: Claude Sonnet 4.5, Implementation: GPT-5.1-Codex-Max)

## Role Switching (Strategic Agent Only)

**CRITICAL**: Strategic Agent (Claude Sonnet 4.5) can seamlessly switch between its 3 roles when delegated:
- Orchestrator ↔ Planner ↔ Reviewer

**Cannot switch to**:
- Implementation Agent (requires GPT-5.1-Codex-Max)
- Scaffold Agent (requires different model)

**Behavior**:
- When Flow is invoked with delegation to another Strategic role → **assume that role immediately and execute**
- When Flow is invoked with delegation to Implementation Agent → **return delegation code block (don't execute)**

## Model-Role Enforcement

**If you ARE the delegated model for the role**:
- Read delegation prompt
- Assume delegated role
- Execute immediately

**If you ARE NOT the delegated model for the role**:
- DO NOT execute
- Return delegation prompt in markdown code block for user to invoke correct model
- Use format:

  ````markdown
  **Task**: [{TaskPath}]({TaskPath})
  **Role**: {Role} (see [Framework/{Prompt}.prompt.md](../Framework/{Prompt}.prompt.md))
  **Target Audience**: {Agent} ({Model})
  
  {Delegation context}
  ````

## When Continuing

{existing content}
```

**Changes to Strategic.prompt.md**:

Add after Role 1: Orchestrator section:

```markdown
### Backlog Management

**Configuration**: See [Configuration.xeyth](../Configuration.xeyth) for backlog thresholds

**Workflow**:
1. Check pending tasks in slice directories
2. If backlog < minimum threshold → create new enhancement tasks
3. If backlog > maximum threshold → focus on execution
4. Select highest-priority task from healthy backlog
5. Delegate to Planner

**Backlog Health**:
- **Healthy**: {backlog.minimum}-{backlog.maximum} ready tasks
- **Low**: < {backlog.minimum} → create 2-3 new tasks
- **High**: > {backlog.maximum} → pause creation, focus execution
```

### Implementation 2: Configuration.xeyth

**File**: `Configuration/Configuration.xeyth`

```yaml
# .xeyth Framework Configuration
# This file configures the AI instruction framework behavior

## Orchestrator Settings
orchestrator:
  backlog:
    minimum: 5          # Create new tasks when backlog drops below this
    maximum: 10         # Pause task creation when backlog exceeds this
    createThreshold: 7  # Optimal backlog size to maintain

## Testing Configuration
testing:
  verify:
    diffTool: VisualStudioCodeInsiders  # Preferred diff viewer for Verify snapshots
    
## Convention Settings
conventions:
  archiving:
    dateFormat: "yyyy-MM-dd"  # Format for archive filename dates
    directoryName: "archive"  # Name of archive directories
    
## Model Requirements (informational - enforcement in prompts)
models:
  strategic: "Claude Sonnet 4.5"
  implementation: "GPT-5.1-Codex-Max"
```

**Workspace Update**: Add to `.xeyth.code-workspace`:

```json
{
  "files.associations": {
    "*.task": "markdown",
    "*.plan": "markdown",
    "*.report": "markdown",
    "*.template": "markdown",
    "*.convention": "markdown",
    "*.inventory": "markdown",
    "*.metadata": "yaml",
    "*.xeyth": "yaml"
  }
}
```

### Implementation 3: Task Template Enhancement

**Update Planning/Task.task.template** to include delegation section:

```markdown
---
# DELEGATION PROMPT
---

## Role: {Orchestrator|Planner|Reviewer|Implementer}

**Task File**: {Slice}/{TaskName}.task  
**Objective**: {One-line objective}

**Role Reference**: See [Framework/{Role}.prompt.md](../Framework/{Role}.prompt.md)

### Context

{2-3 sentences: why this task exists, what problem it solves}

### Your Responsibilities

1. {Primary responsibility}
2. {Secondary responsibility}

### Deliverables

- [ ] **D1**: {Deliverable with acceptance criteria}
- [ ] **D2**: {Another deliverable}

### Implementation Guidance (if Implementer role)

**Patterns to Follow**:
- {Reference file demonstrating pattern}

**Verification Steps**:
- [ ] `dotnet build` succeeds (0 errors, 0 warnings)
- [ ] `dotnet test` passes

### Upon Completion

1. Update Progress Log
2. Delegate to **{NextRole}** using:

   ````markdown
   **Task**: [{TaskPath}]({TaskPath})
   **Role**: {NextRole} (see [Framework/{NextPrompt}.prompt.md](../Framework/{NextPrompt}.prompt.md))
   **Target Audience**: {NextAgent} ({NextModel})
   
   {Brief context}
   ````

---
# TASK DETAILS
---

{Current template content}
```

---

## Summary Recommendations

| Proposal | Priority | Adopt? | Rationale |
|----------|----------|--------|-----------|
| Enhanced Flow with role switching | Critical | ✅ YES | Core workflow improvement |
| Configuration.xeyth | Critical | ✅ YES | Makes framework configurable |
| Task delegation prompt section | High | ✅ YES | Improves task clarity |
| Strict response footer | Optional | ⚠️ MAYBE | Evaluate in practice |

**Next Steps**:
1. Review this analysis with user
2. Get approval for proposals
3. Implement Phase 1 (Critical items)
4. Test workflow with new capabilities
5. Iterate based on feedback
