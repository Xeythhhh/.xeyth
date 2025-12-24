# .xeyth - AI Instruction Framework

**Lightweight, reusable AI instruction framework for agent-driven development.**

## Overview

.xeyth is a Git submodule-based framework that provides structured roles, workflows, and conventions for AI-assisted software development. It enables Strategic and Implementation agents to collaborate on planning, executing, and reviewing work through a systematic approach.

## Features

- **Role-Based Workflows**: Strategic Agent (Orchestrator/Planner/Reviewer) and Implementation Agent with clear responsibilities
- **Task Management**: Structured `.task` files with delegation prompts, deliverables, and progress tracking
- **Planning System**: `.plan` files for multi-task coordination and architecture decisions
- **Proposal System**: Capture optimization ideas during implementation without disrupting focus
- **Context Files**: Markdown-based conventions (`.task`, `.plan`, `.report`, `.convention`, `.proposal`, `.template`)
- **Continuous Flow**: Agents operate autonomously, delegating between roles and creating new tasks
- **Quality Gates**: Built-in verification (builds, tests, lints) and progress tracking

## Getting Started

### As a Submodule

Add `.xeyth` to your repository:

```bash
git submodule add https://github.com/{user}/.xeyth.git .xeyth
git submodule update --init --recursive
```

Initialize the framework:

```bash
# See .xeyth/Ai/Maintenance/InitializeAiFramework.prompt.md for complete setup
```

Create `.github/copilot-instructions.md` in your repository:

```markdown
# Copilot Instructions

This project uses the AI Framework from the `.xeyth` submodule. See [.xeyth/Ai/Framework/copilot-instructions.md](.xeyth/Ai/Framework/copilot-instructions.md) for roles, workflows, and conventions.

---

<!-- Add your project-specific rules below -->
```

Configure workspace settings:

```json
{
  "settings": {
    "files.associations": {
      "*.task": "markdown",
      "*.plan": "markdown",
      "*.report": "markdown",
      "*.template": "markdown",
      "*.convention": "markdown",
      "*.proposal": "markdown",
      "*.inventory": "markdown"
    },
    "chat.instructionsFiles.locations": [
      ".github",
      ".xeyth/Ai/Framework"
    ]
  }
}
```

### Standalone Usage

Clone the repository:

```bash
git clone --recursive https://github.com/{user}/.xeyth.git
cd .xeyth
```

Open workspace:

```bash
code .xeyth.code-workspace
```

## Project Structure

```
.xeyth/
  Ai/
    Framework/           # Core prompts, conventions (Strategic, Implementation, Flow)
    Maintenance/         # Framework maintenance tasks
    Configuration.xeyth  # Framework configuration (backlog, archiving, models)
  Planning/              # Templates (Task, Plan, Report, Proposal)
  Automation/            # CLI tools (xeyth-verify, xeyth-contracts, xeyth-planning)
  Contracts/             # Contract validation system
  Git/                   # Git conventions, workflows, collaboration
  Research/              # Research documents and analysis
```

## Core Concepts

### Roles

- **Strategic Agent** (Claude Sonnet 4.5)
  - **Orchestrator**: Select work, maintain backlog, create tasks
  - **Planner**: Research, design, architecture decisions
  - **Reviewer**: Verify deliverables, approve/request changes

- **Implementation Agent** (GPT-5.1-Codex-Max)
  - **Implementer**: Plan review + code execution

### Workflows

1. **Orchestrator** selects highest-priority task from backlog
2. **Planner** researches and creates implementation plan
3. **Implementation Agent** reviews plan, implements deliverables
4. **Reviewer** verifies work, approves or requests changes
5. **Orchestrator** selects next task (continuous flow)

### Task System

Tasks are markdown files (`.task`) with:
- Delegation prompt (role, objective, deliverables)
- Task details (status, priority, effort, architecture decisions)
- Progress log (decisions, verification, reports)

Example: `Git/GitHubRepositorySetup.task`

### Proposal System

Capture ideas during implementation without context switching:
- Create `.proposal` file when discovering optimization
- Orchestrator reviews pending proposals in batch
- Accept (create task), Defer, or Reject with rationale

## CLI Tools

- **xeyth-verify**: Verify snapshot test differences
- **xeyth-contracts**: Validate YAML contracts
- **xeyth-planning**: Manage proposals and backlog (planned)

Install as dotnet tools:

```bash
dotnet tool install -g xeyth-verify
dotnet tool install -g xeyth-contracts
```

## Configuration

Optional `.xeyth/Configuration/Configuration.xeyth`:

```yaml
orchestrator:
  backlog:
    minimum: 5
    maximum: 20
    createThreshold: 7

testing:
  verify:
    diffTool: VisualStudioCodeInsiders

conventions:
  archiving:
    dateFormat: "yyyy-MM-dd"
    directoryName: "archive"

models:
  strategic: "Claude Sonnet 4.5"
  implementation: "GPT-5.1-Codex-Max"
```

## Documentation

- **Framework**: [Ai/Framework/copilot-instructions.md](Ai/Framework/copilot-instructions.md)
- **Strategic Agent**: [Ai/Framework/Strategic.prompt.md](Ai/Framework/Strategic.prompt.md)
- **Implementation Agent**: [Ai/Framework/Implementation.prompt.md](Ai/Framework/Implementation.prompt.md)
- **Flow System**: [Ai/Framework/Flow.prompt.md](Ai/Framework/Flow.prompt.md)
- **Initialization**: [Ai/Maintenance/InitializeAiFramework.prompt.md](Ai/Maintenance/InitializeAiFramework.prompt.md)
- **Task Template**: [Planning/Task.task.template](Planning/Task.task.template)
- **Proposal System**: [Planning/ProposalSystem.md](Planning/ProposalSystem.md)
- **Git Conventions**: [Conventions/CommitMessageFormat.convention](Conventions/CommitMessageFormat.convention)

## Technology Stack

- **.NET 10**: Cross-platform runtime
- **C#**: Modern language features
- **Nuke**: Build automation
- **xUnit**: Unit testing
- **Verify**: Snapshot testing
- **Spectre.Console**: CLI visual components
- **Playwright**: Browser/E2E testing

## Quality Gates

All contributions must pass:
- `dotnet build` (0 errors, 0 warnings)
- `nuke Compile Test` (all tests passing)
- Markdown linting (markdownlint)
- Conventional Commits format

## Contributing

See [Git/Collaboration.plan](Git/Collaboration.plan) for:
- PR workflow (GitHub Flow)
- Commit message standards (`.gitmessage`)
- Code review guidelines
- Cloud agent integration

## License

[To be determined]

## Contact

[To be determined]

---

**Status**: Active Development  
**Visibility**: Private (→ Public later)  
**Version**: Pre-release
