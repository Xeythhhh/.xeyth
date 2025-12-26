# Integration Guide

**Goal**: Use this framework as a drop-in submodule under `.xeyth/` in your host repository.

**Quick Start**: Run [InitializeAiFramework.prompt.md](../Maintenance/InitializeAiFramework.prompt.md) for step-by-step setup.

---

## Architecture

### Two-Layer System

1. **Generic Framework** (this submodule):
   - Roles, workflows, conventions
   - Context file system (`.task`, `.plan`, `.report`, etc.)
   - Templates and planning tools
   - **Location**: `.xeyth/Framework/copilot-instructions.md`
   - **Maintenance**: Read-only for consumers; updated via submodule pulls

2. **Project-Specific Integration** (your host repo):
   - Quality gates (build/test/lint commands)
   - Tech stack and environment details
   - Coding standards and architectural constraints
   - Domain-specific knowledge
   - **Location**: `.github/copilot-instructions.md` in host repository root
   - **Maintenance**: Customized by your team

### How GitHub Copilot Sees It

```text
GitHub Copilot reads:
  your-repo/.github/copilot-instructions.md
    ↓ references via relative paths
  your-repo/.xeyth/Framework/copilot-instructions.md
    ↓ includes
  your-repo/.xeyth/Framework/Strategic.prompt.md
  your-repo/.xeyth/Framework/Implementation.prompt.md
  ... (other roles and conventions)
```

---

## Setup Steps (Summary)

See [InitializeAiFramework.prompt.md](../Maintenance/InitializeAiFramework.prompt.md) for detailed instructions.

1. **Add Submodule**: `git submodule add <repo-url> .xeyth`
2. **Create Integration File**: Add `.github/copilot-instructions.md` in host repo root with single reference line to framework
3. **Add Project Rules**: Below the integration line, add your quality gates, tech stack, coding standards
4. **Configure**: Set file associations for `.task`, `.plan`, `.report`, etc. → markdown
5. **Validate**: Test delegation to roles; verify reference link resolves

**Why minimal integration?** The template is intentionally just one line + comment to avoid prescribing structure or conflicting with your existing conventions. You control what goes in your file; the framework stays read-only in the submodule.

---

## Customization Points

### What to Add (Not Required Placeholders)

Your `.github/copilot-instructions.md` is minimal by design. Add sections based on your project needs:

**Common additions**:
- **Quality Gates**: Build, test, lint commands and acceptance criteria
- **Tech Stack**: Languages, frameworks, databases, tools
- **Coding Standards**: Style preferences, architectural patterns, conventions
- **Domain Knowledge**: Business rules, user roles, workflow states
- **Architecture**: Directory structure, module boundaries, integration points
- **Security**: Validation requirements, auth patterns, data handling rules

**Example minimal file**:
```markdown
# Copilot Instructions

This project uses the AI Framework from the `.xeyth` submodule. See [.xeyth/Framework/copilot-instructions.md](.xeyth/Framework/copilot-instructions.md) for roles, workflows, and conventions.

---

## Quality Gates

- Build: `npm run build` (0 errors)
- Test: `npm test` (80% coverage)
- Lint: `npm run lint`

## Tech Stack

TypeScript 5, React 18, Node 20, PostgreSQL 15

## Standards

- Use React hooks
- Zod validation for all APIs
- Feature flags via LaunchDarkly
```

### Precedence

When conflicts arise:
- **Project-specific** (`.github/copilot-instructions.md`) wins over generic
- **Example**: Generic says "keep files short"; project adds "max 500 lines per module"

---

## Working with Tasks

1. **Create**: Use [Planning/Task.task.template](../Planning/Task.task.template)
2. **Organize**: Place in appropriate slice (`Framework/`, `Maintenance/`, etc.)
3. **Delegate**: Reference roles via 3-line format (see [Strategic.prompt.md](Strategic.prompt.md))
4. **Archive**: Move completed tasks to `archive/` within each slice

---

## File Associations

Add to `.code-workspace` or user settings:

```json
{
  "files.associations": {
    "*.task": "markdown",
    "*.plan": "markdown",
    "*.report": "markdown",
    "*.template": "markdown",
    "*.convention": "markdown",
    "*.inventory": "markdown"
  }
}
```

**Why**: Enables markdown preview, syntax highlighting, and editing tools for context files.

---

## Maintenance

### Updating the Submodule

```bash
cd .xeyth
git pull origin main
cd ..
git add .xeyth
git commit -m "chore: update AI framework submodule"
```

**When to update**:
- New roles or workflows added
- Bug fixes in prompts or templates
- Structural changes (re-run [InitializeAiFramework.prompt.md](../Maintenance/InitializeAiFramework.prompt.md) if needed)

### Keeping Host Instructions in Sync

- Review `.github/copilot-instructions.md` after submodule updates
- Verify relative paths still resolve
- Update project-specific rules as your codebase evolves

---

## Troubleshooting

**Copilot doesn't see framework files**: Ensure `.github/copilot-instructions.md` is in host repo root with the reference link

**Context files not rendering as markdown**: Check file associations in workspace settings

**Integration link broken**: Verify submodule is at `.xeyth/` with `Framework/copilot-instructions.md`

**Reference link doesn't work**: Use relative path `.xeyth/Framework/copilot-instructions.md` from host repo root

---

## Examples

See [InitializeAiFramework.prompt.md](../Maintenance/InitializeAiFramework.prompt.md) for:
- Complete directory structure after setup
- Sample delegation prompts
- Validation checklists

---

**Next**: Run the initialization prompt to get started.
