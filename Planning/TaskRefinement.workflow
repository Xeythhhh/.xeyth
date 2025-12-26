# Task Refinement Workflow

**Purpose**: Define the workflow for creating, refining, and delegating tasks to ensure high-quality, actionable work items.

## Overview

The task refinement workflow ensures all tasks meet quality standards before delegation to the Implementation Agent. This creates a smooth pipeline from task creation → refinement → delegation → implementation → review.

## Workflow Stages

### Stage 1: Task Creation (Orchestrator)

**Trigger**: Backlog below target (< 15 tasks)

**Process**:
1. Orchestrator identifies gap in backlog
2. Creates new task file in appropriate slice
3. Populates basic metadata (Status, Priority, Effort, Owner)
4. Adds delegation prompt with 4-backtick code block
5. Includes minimal Context and Objective
6. Sets Status: Not Started
7. Updates inventory file

**Output**: Task file created but not refined

**Quality Level**: Basic - needs refinement

**Example**:
```markdown
# New Feature Task

Status: Not Started
Priority: Medium
Effort: Medium

## DELEGATION PROMPT
````markdown
**Task**: [Feature/NewFeature.task](Feature/NewFeature.task)
**Role**: Planner
**Target Audience**: Strategic Agent (Claude Sonnet 4.5)

Implement new feature for X
````

## Context
Brief description of need

## Objective
Add new feature
```

### Stage 2: Task Refinement (Planner)

**Trigger**: Orchestrator delegates task to Planner for refinement

**Process**:
1. Planner reads task delegation prompt
2. Expands Context section with current/desired state
3. Details Objective with specific outcomes
4. Defines concrete Deliverables with acceptance criteria
5. Makes Architecture Decisions with rationale
6. Specifies Success Criteria (build, test, lint, task-specific)
7. Adds Implementation Guidance if needed
8. Updates Status: Planning or Planning Complete
9. Updates Progress Log

**Output**: Refined task ready for implementation

**Quality Level**: High - ready for delegation

**Quality Checks**:
- [ ] All required sections present
- [ ] Deliverables are specific and measurable
- [ ] Success criteria are testable
- [ ] Architecture decisions include rationale
- [ ] Links to related files are valid
- [ ] Delegation prompt properly formatted

**Example**:
```markdown
# Contract Validation Enhancement

Status: Planning Complete
Priority: High
Effort: Medium
Priority Score: 9

## DELEGATION PROMPT
````markdown
**Task**: [Contracts/ContractValidation.task](Contracts/ContractValidation.task)
**Role**: Implementer
**Target Audience**: Implementation Agent (GPT-5.1-Codex-Max)

Enhance contract validation with schema checks
````

## Context

**Current State**:
- Basic naming validation exists
- No schema validation
- Manual verification required

**Desired State**:
- Automated schema validation
- Comprehensive error reporting
- Integration with CI/CD

## Objective

Add JSON Schema validation to contract validation system

## Deliverables

1. **D1**: Schema validation service
   - Acceptance: Validates contracts against JSON Schema
   - Acceptance: Reports violations with line numbers

2. **D2**: Integration with existing validation
   - Acceptance: Runs after naming validation
   - Acceptance: Aggregates all violations

## Architecture Decisions

**AD-1**: Use NJsonSchema library
- Rationale: Mature, well-tested, .NET native
- Alternative: Custom parser - too complex

## Success Criteria

- [ ] `dotnet build` succeeds (0 warnings)
- [ ] `dotnet test` passes all tests
- [ ] Schema validation detects violations
- [ ] Error messages include line numbers
```

### Stage 3: Quality Gate (Automated)

**Trigger**: Before delegation to Implementation Agent

**Process**:
1. Run task quality validation (future automation)
2. Check required sections present
3. Validate metadata completeness
4. Calculate quality score
5. Block delegation if score < 70
6. Return refinement checklist if failed

**Output**: Pass/Fail + refinement suggestions

**Quality Thresholds**:
- **90-100**: Excellent - approve immediately
- **70-89**: Good - approve with minor suggestions
- **50-69**: Fair - request refinement
- **< 50**: Poor - block delegation

### Stage 4: Delegation (Planner → Implementation)

**Trigger**: Task refined and quality gate passed

**Process**:
1. Planner completes refinement
2. Updates Status: Planning Complete
3. Creates delegation prompt in 4-backtick code block
4. Delegates to Implementation Agent
5. Updates Progress Log
6. Assumes Orchestrator role for next task

**Output**: Implementation Agent receives refined task

**Delegation Format**:
````markdown
**Task**: [Slice/TaskName.task](Slice/TaskName.task)
**Role**: Implementer (see [Framework/Implementation.prompt.md](Framework/Implementation.prompt.md))
**Target Audience**: Implementation Agent (GPT-5.1-Codex-Max)

Brief context about what needs implementation
````

### Stage 5: Implementation (Implementation Agent)

**Trigger**: Receives delegation from Planner

**Process**:
1. Reviews task file and all deliverables
2. Creates feature branch `task/{task-name}`
3. Implements deliverables iteratively
4. Updates Progress Log as work completes
5. Runs quality gates (build, test, lint)
6. Creates PR when deliverables complete
7. Delegates to Reviewer

**Output**: PR with completed implementation

### Stage 6: Review (Reviewer)

**Trigger**: Implementation Agent delegates after completion

**Process**:
1. Reviews deliverables against task Success Criteria
2. Runs quality gates
3. Verifies documentation updated
4. Approves or requests changes
5. Merges PR or returns to Implementation
6. Archives task on approval
7. Assumes Orchestrator role for next task

**Output**: Task complete and archived or returned for changes

## Task Quality Standards

### Required Sections

All tasks MUST include:
- **DELEGATION PROMPT**: 4-backtick code block with task path, role, target audience
- **Status**: Not Started | Planning | Planning Complete | In Progress | Complete | Blocked
- **Owner**: Role responsible for current stage
- **Priority**: Critical | High | Medium | Low
- **Effort**: Small | Medium | Large
- **Objective**: Clear, one-sentence objective
- **Context**: Current state, desired state, why task exists
- **Deliverables**: Numbered list with acceptance criteria
- **Success Criteria**: Testable verification steps

### Optional Sections

Tasks MAY include:
- **Priority Score**: Calculated based on impact/urgency/complexity
- **Architecture Decisions**: Design choices with rationale
- **Implementation Guidance**: Patterns, constraints, notes
- **Verification**: Manual verification steps
- **Progress Log**: Timestamped entries of work done

### Delegation Prompt Format

**CRITICAL**: Must use 4-backtick code block for cross-model delegation

```markdown
## DELEGATION PROMPT

````markdown
**Task**: [Slice/TaskName.task](Slice/TaskName.task)
**Role**: {Planner|Implementer|Reviewer}
**Target Audience**: {Agent} ({Model})

{1-2 sentence context}
````
```

## Refinement Triggers

### Automatic Triggers

Tasks enter refinement when:
- Created by Orchestrator (always needs refinement)
- Returned from quality gate (validation failed)
- Blocked during implementation (needs re-planning)
- Failed review (deliverables changed)

### Manual Triggers

Tasks may be refined when:
- Stale (>7 days without update)
- Scope changed based on findings
- Dependencies changed
- User requests update

## Staleness Detection

### Staleness Rules

- **Not Started** + 14 days = Stale
- **Planning** + 3 days = Needs decision
- **In Progress** + 7 days = Needs update
- Any status + 30 days = Review or archive

### Staleness Actions

1. **Alert**: Notify Orchestrator
2. **Review**: Planner re-reads and updates
3. **Refine**: Update context, deliverables, decisions
4. **Archive**: If no longer relevant
5. **Continue**: If still valid

## Continuous Flow Integration

### Orchestrator Responsibilities

1. **Backlog Management**:
   - Maintain 20 tasks (15-25 range)
   - Create new tasks when < 15
   - Pause creation when > 25

2. **Task Selection**:
   - Review priority scores
   - Select highest-priority task
   - Delegate to Planner for refinement

3. **Quality Monitoring**:
   - Track task quality scores
   - Identify stale tasks
   - Ensure refinement completeness

### Planner Responsibilities

1. **Refinement**:
   - Expand all required sections
   - Detail deliverables with acceptance
   - Make architecture decisions
   - Define success criteria

2. **Quality Assurance**:
   - Ensure task is actionable
   - Validate all links and references
   - Check delegation prompt format
   - Update metadata

3. **Delegation**:
   - Create delegation prompt
   - Delegate to Implementation Agent
   - Update Progress Log
   - Assume Orchestrator role

### Implementation Agent Responsibilities

1. **Execution**:
   - Follow deliverables exactly
   - Update Progress Log frequently
   - Run quality gates early and often
   - Request clarification if blocked

2. **PR Creation**:
   - Create branch `task/{task-name}`
   - Reference task file in PR
   - Include deliverables in PR description
   - Link to verification evidence

## Metrics & Monitoring

### Key Metrics

- **Task Quality Score**: Average across all active tasks
- **Refinement Cycle Time**: Time from creation to Planning Complete
- **Staleness Rate**: % of tasks stale
- **Delegation Success Rate**: % of tasks passing quality gate
- **Implementation Cycle Time**: Time from delegation to PR creation

### Health Indicators

- **Green**: Quality score > 80, staleness < 10%, delegation success > 90%
- **Yellow**: Quality score 60-80, staleness 10-20%, delegation success 70-90%
- **Red**: Quality score < 60, staleness > 20%, delegation success < 70%

### Optimization Targets

- Average quality score: > 85
- Staleness rate: < 5%
- Delegation success rate: > 95%
- Refinement cycle time: < 2 days
- Implementation cycle time: < 3 days

## Best Practices

### For Task Creation

1. **Start Simple**: Create task with basic structure, let Planner refine
2. **Use Templates**: Follow Task.task.template structure
3. **Link Related Work**: Reference related tasks, plans, files
4. **Set Priority**: Use priority guidelines from TaskPrioritySystem.md
5. **Estimate Effort**: Be realistic about Small/Medium/Large

### For Task Refinement

1. **Research First**: Understand problem before planning solution
2. **Be Specific**: Vague deliverables lead to unclear implementations
3. **Define Acceptance**: How will you know deliverable is complete?
4. **Consider Alternatives**: Document why you chose this approach
5. **Plan Verification**: How will implementation be tested?

### For Quality Assurance

1. **Check Completeness**: All required sections present
2. **Validate Format**: Delegation prompt uses 4-backtick code block
3. **Test Links**: All file references are valid
4. **Calculate Score**: Use priority score formula
5. **Review Clarity**: Would someone else understand this task?

## Troubleshooting

### Issue: Task stuck in Planning

**Symptoms**: Task in Planning status for > 3 days

**Causes**:
- Unclear requirements
- Too broad scope
- Missing information

**Solutions**:
- Break into smaller tasks
- Gather more context
- Delegate research task first

### Issue: Task fails quality gate

**Symptoms**: Validation returns errors/warnings

**Causes**:
- Missing required sections
- Vague deliverables
- Invalid links

**Solutions**:
- Follow refinement checklist
- Use task template
- Validate before delegation

### Issue: Task becomes stale

**Symptoms**: No updates for > 7 days

**Causes**:
- Lost priority
- Blocked on dependencies
- Unclear next steps

**Solutions**:
- Re-evaluate priority
- Break dependencies
- Refine or archive

## See Also

- [Task.task.template](Task.task.template) - Task file template
- [TaskPrioritySystem.md](TaskPrioritySystem.md) - Priority scoring
- [Delegation.instructions.md](../Framework/Delegation.instructions.md) - Delegation format
- [Flow.prompt.md](../Framework/Flow.prompt.md) - Continuous workflow
- [Strategic.prompt.md](../Framework/Strategic.prompt.md) - Planner role guidance
