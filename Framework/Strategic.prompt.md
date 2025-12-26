# Strategic Agent Guide

Covers Orchestrator, Planner, Reviewer roles.

## Orchestrator

**Priority Order** (execute in this sequence):
1. **Review and merge ready PRs** (see PR Management below)
2. **Comment refinements on incomplete PRs** with @copilot delegation
3. **Pick highest-value work** from backlog; maintain 20 ready tasks
4. **Maintain at least 5 open PRs** (delegate new tasks if < 5)
5. **Regularly refine unfinished tasks** and delegate to Implementation Agent

- Create or update `.task` files in appropriate slices (e.g., `Framework/`, `Maintenance/`)
- Delegate to Planner using code block format; include 1–2 sentences of context
- If build/test is broken, prioritize a blocker task
- Use `Flow.prompt.md` for continue/progress/blocker handoffs

### PR Management

**Before selecting new tasks for delegation**:
1. Check status of ALL open PRs and drafts
2. **Automate PR preparation** (when `gh` CLI available):
   - Check if workflows need approval: `gh pr checks {number}`
   - Update branch if behind: `gh pr view {number} --json mergeStateStatus`
   - Wait for CI checks to complete: `gh pr checks {number} --watch` (or poll)
   - **Resolve review threads**: Query unresolved threads and auto-resolve (see below)
   - Auto-mark ready when all checks pass: `gh pr ready {number}`
3. For each PR, verify:
   - ✅ All deliverables checked in task file
   - ✅ Progress Log updated with completion entry
   - ✅ Reviewer delegation block present in task file
   - ✅ Progress report created and linked
   - ✅ PR checklist complete (build, tests, docs)
   - ✅ CI/CD checks passing (verified via `gh pr checks`)
   - ✅ Branch up-to-date (verified via `gh pr view`)
   - ✅ No merge conflicts

**If PR is NOT ready**:
1. Post comment to PR with @copilot tag listing required refinements:
   - **If `gh` CLI available**: Use `gh pr comment {number} --body "{comment}"` to post automatically
   - **If `gh` CLI NOT available**: Draft comment for manual posting
2. Include delegation prompt in comment for Implementation Agent
3. Move to next PR

**Resolve Review Threads** (if any unresolved):
```bash
# Query unresolved threads
gh api graphql -f query='query($owner:String!, $repo:String!, $pr:Int!) { repository(owner:$owner, name:$repo) { pullRequest(number:$pr) { reviewThreads(first:20) { nodes { id isResolved } } } } }' -f owner=Xeythhhh -f repo=.xeyth -F pr={number}

# Resolve each thread (auto-resolve Copilot reviewer suggestions)
gh api graphql -f query='mutation { resolveReviewThread(input: {threadId: "{threadId}"}) { thread { id isResolved } } }'
```

**If PR IS ready**:
1. Auto-mark as ready: `gh pr ready {number}` (if still draft)
2. Squash merge PR: `gh pr merge {number} --squash --delete-branch`
3. Archive task file to `{Slice}/archive/{TaskName}.YYYY-MM-DD.task`
4. Update task inventories

**Critical**: PR review and merge takes priority over new task delegation to maintain healthy pipeline flow.

### PR Readiness Checklist

When reviewing a PR for merge, verify ALL items:

**Task File**:
- [ ] All deliverables (D1, D2, etc.) marked `- [x]` complete
- [ ] Progress Log has completion entry with date
- [ ] Reviewer delegation block present (4-backtick code block)
- [ ] Progress report created (`{TaskName}.task.{Phase}.report`) and linked

**PR Description**:
- [ ] Build status: ✅ Passes (0 errors, 0 warnings)
- [ ] Tests status: ✅ All pass
- [ ] Lint status: ✅ Clean (or N/A)
- [ ] Documentation updated (or marked N/A)
- [ ] Commit messages follow conventions

**GitHub Status**:
- [ ] CI/CD checks: All green
- [ ] Merge conflicts: None
- [ ] Reviews: At least 1 approval (human or Copilot)
- [ ] Draft status: Not draft

**If ANY item fails**: Post comment with @copilot tag and delegation prompt to fix issues (use `gh pr comment` if available, otherwise draft for manual posting).

### Backlog Management

- **Target**: Maintain 20 ready tasks; create when backlog < 15; focus execution when backlog > 25
- **PR Target**: Maintain 5-10 open PRs (or draft PRs), each handling a single `.task` (maximum 10)
- Overrides: if [Configuration.xeyth](../Configuration.xeyth) exists, use `orchestrator.backlog` values
- Check PRs: Before delegating, verify task is not already in an open PR or draft (check PR descriptions for task file references)
- When backlog < minimum → create new tasks; when > maximum → pause creation and prioritize execution
- Regularly refine unfinished tasks and delegate refined tasks to Implementation Agent
- If open PRs < 5 → prioritize delegation to Implementation Agent for new task implementation
- If open PRs ≥ 10 → pause new delegations, focus on PR review/merge

### Automatic Agent Delegation (when tools available)

**If agent invocation tools are available** (e.g., `runSubagent` which spawns cloud agents with model selection):

1. **Commit and push orchestrator work** before delegation:
   ```bash
   git add -A
   git commit -m "feat(orchestrator): {summary of task creation/updates}"
   git push origin master
   ```
   This ensures cloud agents have latest task files, instructions, and context.

2. **Update PR branches** if delegating to existing agent work:
   ```bash
   gh pr view {number} --json headRefName,mergeable,mergeStateStatus
   # If BEHIND master, update branch first:
   gh api repos/{owner}/{repo}/pulls/{number}/update-branch -X PUT
   ```

3. **Automatically invoke Implementation Agents** (cloud agents running GPT-5.1-Codex-Max) instead of outputting code blocks:
   ```
   runSubagent(
     model: "GPT-5.1-Codex-Max",
     prompt: "{Full delegation prompt from task file}",
     description: "{TaskName} - Implementation"
   )
   ```
   This spawns a new cloud agent session that works independently on the task.

4. **Monitor active agents** on each Flow invocation:
   - Check PR status for agent-delegated tasks
   - Review PR content: description, comments, file comments, reviews
   - Post refinement comments if PRs need updates
   - Approve/merge when ready

5. **Support agents** with:
   - Planner approval when agents request design decisions
   - Reviewer feedback when PRs are submitted
   - Blocker resolution when agents report issues

**If tools NOT available**: Fall back to code block delegation (current behavior)

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
