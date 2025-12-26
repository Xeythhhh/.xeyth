````prompt
# Initialize AI Framework

**Role**: Orchestrator  
**When**: First adding the `.xeyth` submodule to a host repository  
**Goal**: Set up file associations, integrate with GitHub Copilot, and prepare the framework for use

---

## Overview

This prompt walks through the one-time setup needed to integrate the AI Framework into your host repository. After completion, agents will have access to roles, conventions, and planning tools.

---

## Steps

### 1. Create Minimal Integration File

Create `.github/copilot-instructions.md` in your **host repository root** with this single integration line:

```markdown
# Copilot Instructions

This project uses the AI Framework from the `.xeyth` submodule. See [.xeyth/Framework/copilot-instructions.md](.xeyth/Framework/copilot-instructions.md) for roles, workflows, and conventions.

---

<!-- Add your project-specific rules, quality gates, and conventions below -->
```

**Why minimal?**
- Avoids prescribing structure that may conflict with your existing conventions
- GitHub Copilot reads both files automatically (host file + referenced framework)
- You control what goes in your file; framework stays read-only in submodule
- No placeholders to replace; add only what you need

**Directory structure**:
```
your-repo/                        ← Host repository
  .github/
    copilot-instructions.md       ← Your file (minimal integration + your rules)
  .xeyth/                          ← Submodule (read-only)
    Ai/
      Framework/
        copilot-instructions.md   ← Generic framework (roles, workflows)
  src/
  ...
```

**How it works**:
- GitHub Copilot reads `.github/copilot-instructions.md` in your repo
- The reference link makes framework roles/conventions available
- You add project-specific rules below the integration line
- Your rules take precedence when conflicts arise

---

### 2. Add Your Project-Specific Rules

Below the integration line, add sections relevant to your project. **Examples** (pick what you need):

```markdown
## Quality Gates

- **Build**: `npm run build` (0 errors/warnings)
- **Test**: `npm test` (minimum 80% coverage)
- **Lint**: `npm run lint` (0 errors)

## Tech Stack

- TypeScript 5.x, React 18+, Node 20
- Database: PostgreSQL 15
- API: tRPC with Zod validation

## Coding Standards

- Use React hooks; avoid class components
- All API routes must validate input with Zod schemas
- Prefer composition over inheritance
- Maximum 300 lines per module

## Architecture

- Feature-sliced design in `src/features/`
- Database migrations in `migrations/` with sequential numbering
- All public APIs require OpenAPI spec updates

## Domain Knowledge

- User roles: Admin, Editor, Viewer (see `src/auth/roles.ts`)
- Workflow states: Draft → Review → Published → Archived
- Feature flags managed via LaunchDarkly
```

**Guiding principles**:
- Add only what agents need to know to work on your codebase
- Be specific ("Use Zod for validation") not generic ("Write good code")
- Link to key files when helpful
- Update as your project evolves

---

### 3. Configure File Associations and Chat Instructions

Add to your workspace `.code-workspace` file or user `settings.json`:

```json
{
  "settings": {
    "files.associations": {
      "*.task": "markdown",
      "*.plan": "markdown",
      "*.report": "markdown",
      "*.template": "markdown",
      "*.convention": "markdown",
      "*.inventory": "markdown"
    },
    "chat.instructionsFiles.locations": [
      ".github/instructions",
      "Ai",
      "Framework"
    ]
  }
}
```

**Why**: 
- **File associations**: Context files use markdown format but have custom extensions for semantic clarity. This enables syntax highlighting, preview, and editing tools.
- **Chat instructions**: Tells GitHub Copilot where to find `.instructions.md` files for custom instructions. VS Code will automatically discover and load all `.instructions.md` files in these locations and make them available in chat sessions.

**Automation**:
- Keep the setting in sync with actual `.instructions.md` files by running `dotnet run --project src/Automation.Framework -- update-chat-locations`.
- Validate coverage (CI-friendly) with `dotnet run --project src/Automation.Framework -- validate-chat-locations`.

---

### 4. Validate Integration

Check that the setup is complete:

- [ ] `.github/copilot-instructions.md` exists in **host repository** root with integration line
- [ ] Added project-specific rules (quality gates, tech stack, coding standards)
- [ ] File associations configured in workspace or user settings
- [ ] Can preview context files (`.task`, `.plan`, etc.) as markdown
- [ ] Reference link to `.xeyth/Framework/copilot-instructions.md` resolves correctly
- [ ] `chat.instructionsFiles.locations` configured in workspace settings
- [ ] `chat.instructionsFiles.locations` validated (`dotnet run --project src/Automation.Framework -- validate-chat-locations`)
- [ ] All `.instructions.md` files appear in GitHub Copilot settings (Settings → GitHub Copilot → Chat: Instructions Files Locations)

**Test delegation**: Start a new Copilot conversation and delegate to a role:

```
You are the Planner. Review the strategic goals in Ai.plan and propose 3 high-value tasks.
```

If Copilot can access the roles and framework files, integration succeeded.

**Verify instructions discovery**: Open VS Code Settings (Ctrl+,), search for "Chat: Instructions Files Locations", and verify all configured paths appear with checkmarks showing discovered `.instructions.md` files.

---

### 5. Optional: Run Cleanup

The Cleanup Agent can help normalize files and remove obsolete content. Delegate to it:

```
You are the Cleanup Agent (see .xeyth/Framework/Cleanup.prompt.md).
Review the workspace for:
- Stray or duplicate files
- Inconsistent naming conventions
- Obsolete placeholders or comments

Archive completed tasks to `archive/` subdirectories within each slice.
```

---

### 6. Record Initialization

Update your **host repository's** README with:

```markdown
## AI Framework

This project uses the [.xeyth](https://github.com/your-org/xeyth) AI Framework for agent-driven development.

- **Initialized**: <INITIALIZATION_DATE>
- **Owner**: <OWNER>
- **Instructions**: See [.github/copilot-instructions.md](.github/copilot-instructions.md)
```

---

## Instruction Files Reference

The AI Framework includes the following instruction files that should auto-load via `chat.instructionsFiles.locations`:

### Core Framework Instructions
- [Framework/Delegation.instructions.md](.xeyth/Framework/Delegation.instructions.md) - Delegation format rules for cross-model and role-switching workflows

**Soft References in Copilot Instructions**:
The `.github/copilot-instructions.md` file should reference these instruction files to ensure agents know where to find them if auto-loading fails:

```markdown
## Instruction Files

This project uses the following instruction files for agent workflows:

- **Delegation Format**: [Framework/Delegation.instructions.md](.xeyth/Framework/Delegation.instructions.md)

See workspace settings (`chat.instructionsFiles.locations`) for auto-loading configuration.
```

**Cloud Agent Initialization**:
When delegating to cloud agents, ensure they:
1. Clone repository with submodules (`git clone --recursive`)
2. Open workspace file (`.code-workspace`)
3. Reload VS Code window (Ctrl+Shift+P → "Developer: Reload Window")
4. Verify instruction files loaded (Settings → GitHub Copilot → Chat: Instructions Files Locations)
5. Read assigned task file before starting work

---

## Next Steps

1. **Create your first task**: Use [.xeyth/Ai/Planning/Task.task.template](.xeyth/Ai/Planning/Task.task.template)
2. **Delegate work**: See examples in [.xeyth/Framework/Strategic.prompt.md](.xeyth/Framework/Strategic.prompt.md)
3. **Iterate**: Planner and Implementer collaborate until tasks are complete
4. **Maintain**: Pull submodule updates periodically; re-run this prompt if framework structure changes

---

## Troubleshooting

**Copilot doesn't see framework files**: Ensure `.github/copilot-instructions.md` is in the host repository root with the reference link to `.xeyth/Framework/copilot-instructions.md`.

**Context files not rendering as markdown**: Check file associations in workspace settings.

**Integration link broken**: Verify submodule is at `.xeyth/` and contains `Framework/copilot-instructions.md`.

**Roles not accessible**: Check that the reference link in your `.github/copilot-instructions.md` uses the correct relative path.

**Instructions files not discovered**: 
- Verify `chat.instructionsFiles.locations` paths are correct in workspace settings
- Check that `.instructions.md` files exist in specified locations
- Reload VS Code window (Ctrl+Shift+P → "Developer: Reload Window")
- Open Settings UI and search for "Chat: Instructions Files Locations" to see which files were discovered

---

Use the standard response footer from [copilot-instructions.md](../Framework/copilot-instructions.md) in your responses.

````
