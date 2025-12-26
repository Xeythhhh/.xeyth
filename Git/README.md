# Git Configuration Guide

## Overview

The `.xeyth` repository uses templated commits based on **Conventional Commits 1.0.0** with AI Framework extensions. This system ensures consistent, meaningful commit messages across all contributions.

## Branch Protection (master)

- Visibility: public
- Enforce admins: enabled
- Reviews: minimum 0 approvals (solo development); conversations must be resolved before merge
- Force pushes: blocked; branch deletions: blocked
- Status checks: strict mode enabled; required checks:
  - `build` - Build workflow (ubuntu)
  - `test (ubuntu-latest)` - Test workflow (Ubuntu)
  - `test (windows-latest)` - Test workflow (Windows)
  - `test (macos-latest)` - Test workflow (macOS)

CI workflows (GitHub Actions):

[![Build](https://github.com/Xeythhhh/.xeyth/actions/workflows/build.yml/badge.svg)](https://github.com/Xeythhhh/.xeyth/actions/workflows/build.yml)
[![Test](https://github.com/Xeythhhh/.xeyth/actions/workflows/test.yml/badge.svg)](https://github.com/Xeythhhh/.xeyth/actions/workflows/test.yml)

Check settings:

```bash
gh api repos/Xeythhhh/.xeyth/branches/master/protection
```

## PR Workflow (Task Progress)

All task work must be merged via Pull Requests; direct commits to `master` are blocked by branch protection.

1. **Create feature branch**: `git checkout -b task/{task-name}` (use branch naming below)
2. **Commit locally**: Conventional Commit messages (see format below)
3. **Push branch**: `git push origin task/{task-name}`
4. **Open PR**: Use `.github/pull_request_template.md` and reference the task file in **Related Task**
5. **Reviews**: Resolve comments and obtain required approvals/status checks
6. **After merge**: Update the task Progress Log with the PR link and marked deliverables

### Branch Naming

See [Conventions/BranchNamingConvention.convention](../Conventions/BranchNamingConvention.convention) for full rules.

- `task/{task-name}` — task execution (default for .task files)
- `feature/{feature-name}` — feature delivery
- `fix/{issue-description}` — bug fixes
- `chore/{description}` — maintenance and tooling

### Task ↔ PR Linking

- Every PR must include a **Task File** link in the template (e.g., `Git/PrWorkflowEnforcement.task`)
- Use the **deliverables** checklist in the PR body to track progress for that task
- After merge, add the PR URL to the task Progress Log and mark deliverables complete

## Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type (Required)

Select one:
- `feat` - New feature or capability
- `fix` - Bug fix or correction
- `docs` - Documentation only changes
- `style` - Code style (formatting, whitespace, no logic)
- `refactor` - Code restructuring without behavior change
- `perf` - Performance improvements
- `test` - Adding or updating tests
- `chore` - Maintenance, dependencies, tooling
- `ci` - CI/CD pipeline changes
- `build` - Build system or external dependencies
- `revert` - Reverting a previous commit

### Scope (Optional)

Common scopes:
- `framework` - Core instruction files
- `planning` - Planning slice (templates, context files)
- `maintenance` - Maintenance slice
- `git` - Git conventions and tooling
- `contracts` - Contract metadata system
- `automation` - Automation tools and infrastructure
- `workspace` - Workspace configuration
- `docs` - Documentation files

### Subject (Required)

Rules:
- Use imperative mood: "add" not "added" or "adds"
- Start with lowercase
- No period at end
- Maximum 50 characters
- Be specific and clear

### Body (Optional)

Guidelines:
- Explain what and why, not how
- Wrap lines at 72 characters
- Use bullet points for multiple items (prefix with "- ")
- Separate from subject with blank line

### Footer (Optional)

Common footers:
- `BREAKING CHANGE: <description>` - For incompatible changes
- `Agent: <AgentId> (<Role>)` - AI agent attribution
- `Context: <file.task>, <...>` - Related context files
- `Slice: <SliceName>` - Explicit slice reference
- `Refs: <task or issue>` - Link to tasks/issues
- `Closes: <issue>` - Auto-close issues
- `Co-authored-by: Name <email>` - Multiple authors

## Examples

### Simple Feature

```
feat(git): add commit message template
```

### Bug Fix with Body

```
fix(planning): correct template path references

Updated all prompt files to reference Planning/ instead of 
docs/planning/ after directory restructuring.
```

### Breaking Change

```
refactor(framework)!: restructure directory layout

Moved templates from Framework/planning/ to Planning/ to align
with vertical slice architecture.

BREAKING CHANGE: Template paths changed. Host projects using old
paths must update references.
```

### With Agent Attribution

```
feat(contracts): add validation engine

Implements validation engine for context files using YAML metadata.

Agent: Strategic Agent (Planner)
Context: Contracts/ContractValidation.task
Refs: Ai.plan
```

## Validation

The `commit-msg` hook validates:

✓ Type is valid and present
✓ Subject is present and ≤ 50 characters
✓ Subject uses lowercase first letter
✓ Subject has no trailing period
✓ Scope is valid (if provided)
⚠ Body lines wrap at 72 characters

### Validation Levels

Configure with:
```bash
git config xeyth.commitValidation [strict|warn|disable]
```

- `strict` - Reject commits with errors (default)
- `warn` - Show errors but allow commit
- `disable` - No validation

### Bypass Validation

Not recommended, but possible:
```bash
git commit --no-verify
```

## Git Hooks

The repository includes PowerShell-based hooks for cross-platform compatibility:

- `prepare-commit-msg` - Injects template into commit editor
- `commit-msg` - Validates commit message format

Hooks are located in `.git/hooks/` and require PowerShell Core (`pwsh`).

## Configuration

Git is configured with:

```bash
# Commit template
git config commit.template .gitmessage

# Validation level
git config xeyth.commitValidation strict

# User info (customize for your environment)
git config user.name "Your Name"
git config user.email "your.email@example.com"

# Line endings
git config core.autocrlf input

# Default branch
git config init.defaultBranch main
```

## Template File

The `.gitmessage` template file contains:
- Complete format reference
- All type and scope options
- Validation rules
- Comprehensive examples
- Footer format guide

View it: `cat .gitmessage`

## Troubleshooting

### Hook not executing

Ensure PowerShell Core is installed:
```bash
pwsh --version
```

### Validation errors

Check commit message format:
```bash
git log -1 --pretty=%B
```

Compare against examples in `.gitmessage`.

### Template not showing

Verify git config:
```bash
git config --get commit.template
# Should output: .gitmessage
```

### Windows line ending warnings

These are normal - git converts CRLF → LF per `.gitattributes`:
```
warning: in the working copy of '.gitignore', CRLF will be replaced by LF
```

## Integration with Host Projects

When using `.xeyth` as a submodule, host projects can:

1. **Adopt same commit system**:
   ```bash
   # In host repo
   ln -s .xeyth/.gitmessage .gitmessage
   ln -s .xeyth/.git/hooks/prepare-commit-msg.ps1 .git/hooks/prepare-commit-msg.ps1
   ln -s .xeyth/.git/hooks/commit-msg.ps1 .git/hooks/commit-msg.ps1
   # Create shell wrappers
   ```

2. **Customize template** - Copy and modify `.gitmessage` with project-specific scopes

3. **Extend validation** - Modify `commit-msg.ps1` with additional rules

## References

- [Conventional Commits 1.0.0](https://www.conventionalcommits.org/en/v1.0.0/)
- [Conventions/CommitMessageFormat.convention](../Conventions/CommitMessageFormat.convention) - AI Framework commit specification
- [Git Hooks Documentation](https://git-scm.com/docs/githooks)
- [PowerShell Core](https://github.com/PowerShell/PowerShell)
