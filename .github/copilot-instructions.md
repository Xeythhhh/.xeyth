# Copilot Instructions

This project uses the AI Framework from the `.xeyth` submodule. See [.xeyth/Ai/Framework/copilot-instructions.md](.xeyth/Ai/Framework/copilot-instructions.md) for roles, workflows, and conventions.

## Model Requirements

**CRITICAL**: Use the correct model for each role:
- **Strategic Agents** (Orchestrator/Planner/Reviewer): **Claude Sonnet 4.5 only**
- **Implementation Agent**: **GPT-5.1-Codex-Max only**

---

## Instruction Files

This project uses the following instruction files for agent workflows:

- **Delegation Format**: [Ai/Framework/Delegation.instructions.md](../Ai/Framework/Delegation.instructions.md)

See workspace settings (`chat.instructionsFiles.locations`) for auto-loading configuration.

---

<!-- Add your project-specific rules, quality gates, and conventions below -->

## Project Context

**Project**: AI Framework (`.xeyth`)  
**Purpose**: Lightweight, reusable AI instruction framework distributed as a Git submodule  
**Primary Environment**: Git submodule, VS Code workspace

## Quality Gates

- **Builds**: `dotnet build` and `nuke` (0 errors/warnings)
- **Tests**: `dotnet test` - xUnit (unit), Verify (snapshots), integration, Playwright (E2E)
- **Lint**: Markdown linting via VS Code (markdownlint), C# analyzers
- **Platform**: Cross-platform (Windows, macOS, Linux) - .NET 10

## Project-Specific Conventions

### Technology Stack

- **Language**: C# with latest language features (.NET 10)
- **Build System**: Nuke build automation + dotnet CLI
- **Architecture**: Vertical slice architecture
- **Testing**: 
  - xUnit for unit tests
  - Verify for snapshot tests
  - Integration tests
  - Playwright for browser/E2E tests
  - NetArchTest for architecture validation
- **Code Quality**: Analyzers, linters, modern enterprise patterns

### Context Files

This project maintains its own context file system (`.task`, `.plan`, `.report`, `.convention`, `.inventory`, `.template`). All context files are markdown and tracked in version control.

### Archiving

Completed work moves to slice-level `archive/` directories with optional date suffixes: `archive/{Name}.YYYY-MM-DD.{Type}`

### Vertical Slices

Framework is organized in vertical slices:
- `Ai/Framework/` - Core prompts and conventions
- `Ai/Planning/` - Templates and planning tools
- `Ai/Maintenance/` - Framework maintenance tasks
- `Git/` - Git conventions, workflows, and collaboration

### Integration Model

This framework is generic and reusable. Host repositories reference it via their own `.github/copilot-instructions.md` and add project-specific rules.
