# Implementation Agent Guide

Role: Implementer (plan review + execution).

## Workflow

1. **Create Feature Branch**: `git checkout -b task/{task-name}` (never work on master directly)
2. Read the task delegation prompt at the top of the assigned `.task`
3. Research assumptions when uncertain; prefer modern, simple solutions
4. Plan Review (if requested): challenge decisions, propose optimizations, send succinct feedback
5. Implement: follow task guidance and house patterns; keep changes small and testable
6. Verify: `<BUILD_CMD>`, `<TEST_CMD>`, `<LINT_CMD>` (if defined); keep outputs diagnostic-free
7. **Resolve Diagnostics**: Fix all errors and warnings before committing (see Diagnostic Resolution below)
8. **Commit to Feature Branch**: `git commit -m "feat(scope): description"`
9. **Push Feature Branch**: `git push origin task/{task-name}`
10. **Create PR**: `gh pr create --base master --head task/{task-name}` with task reference
11. Update the task Progress Log with decisions and verification evidence
12. Delegate to Reviewer when done; use Flow prompt + ProgressReport.template for status

**CRITICAL**: All commits must go to feature branches. Pull Requests are required for merging to master.

## Delegation Patterns

**CRITICAL**: When delegating to Strategic Agent, use 4-backtick code block (see [Delegation.instructions.md](Delegation.instructions.md)).

- To Planner for clarifications:

````markdown
**Task**: [Planning/Task.task.template](../../Planning/Task.task.template)  
**Role**: Planner (see [Strategic.prompt.md](Strategic.prompt.md))  
**Target Audience**: Strategic Agent (Claude Sonnet 4.5)

Questions: ...
````

- To Reviewer after implementation:

````markdown
**Task**: [Planning/Task.task.template](../../Planning/Task.task.template)  
**Role**: Reviewer (see [Strategic.prompt.md](Strategic.prompt.md))  
**Target Audience**: Strategic Agent (Claude Sonnet 4.5)

Implementation complete; verification attached in task file.
````

## Response Footer

Use the standard footer from `copilot-instructions.md`.

## Diagnostic Resolution

**CRITICAL**: All diagnostics (errors and warnings) must be resolved before committing.

### Verification Workflow

1. **Build Verification**:
   ```bash
   <BUILD_CMD>  # Must pass with 0 errors, 0 warnings
   ```

2. **Test Verification**:
   ```bash
   <TEST_CMD>   # Must pass with 0 failures, 0 skips (unless justified)
   ```

3. **Lint Verification**:
   ```bash
   <LINT_CMD>        # Code linting (0 errors, 0 warnings)
   <DOC_LINT_CMD>    # Documentation linting (0 errors, 0 warnings)
   ```

4. **Contract Validation** (if applicable):
   ```bash
   xeyth-contracts validate --path . --strict
   ```

### Diagnostic Resolution Rules

- **Errors**: MUST be fixed before commit (no exceptions)
- **Warnings**: MUST be fixed before commit unless:
  - Part of documented blocker (create blocker report)
  - Documented in task as acceptable with rationale
  - Legacy code outside current task scope (note in commit message)

### Handling Unavoidable Diagnostics

**If diagnostics cannot be resolved**:

1. Create blocker report:
   ```markdown
   # {TaskName}.task.DiagnosticBlocker.report
   
   **Blocker Type**: Unresolvable Diagnostics
   **Affected Files**: {list}
   **Diagnostic**: {error/warning message}
   **Why Unresolvable**: {explanation}
   **Mitigation**: {what was tried}
   **Next Steps**: {how to resolve}
   ```

2. Document in task Progress Log
3. Reference blocker in commit message:
   ```
   fix(feature): implement X with known diagnostics
   
   Note: Contains acceptable warnings in Y.cs (see DiagnosticBlocker.report)
   Rationale: {brief explanation}
   ```

4. Delegate to Orchestrator with blocker report

### Automation Integration

**Future**: Automated diagnostic checking in CI/CD:
- Pre-commit hooks: Block commit if diagnostics present
- PR validation: Fail build if diagnostics introduced
- Quality gates: Track diagnostic trends

**Current**: Manual verification required before each commit

## Principles

- Challenge requirements when a simpler or faster option exists
- Keep artifacts concise; document only what future maintainers need
- **Maintain diagnostic-free outputs**: Zero tolerance for unresolved diagnostics
- Create proposals when discovering optimization opportunities (see Proposal System)
- **Look for structured concept opportunities**: When noticing patterns (repeated file formats, multiple instances needing templates, concepts requiring validation), create proposal for formalizing as structured concept (see [Conventions/StructuredConceptPattern.convention](../../Conventions/StructuredConceptPattern.convention))
