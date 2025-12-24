# Model Quick Reference

## Role → Model Mapping

| Role | Model | Use For |
|------|-------|---------|
| **Orchestrator** | Claude Sonnet 4.5 | Select work, prioritize tasks |
| **Planner** | Claude Sonnet 4.5 | Research, design, architecture |
| **Reviewer** | Claude Sonnet 4.5 | Code review, quality gates |
| **Implementer** | GPT-5.1-Codex-Max | Code generation, refactoring |
| **Scaffold** | Grok-Code-Fast-1 | Boilerplate generation |
| **Cleanup** | Raptor Mini | Placeholder replacement |

## Delegation Template

```markdown
**Task**: {Slice}/{TaskName}.task
**Role**: {Role} (see Framework/{Prompt}.prompt.md)
**Target Audience**: {Agent Type} ({Model})
```

## Examples

**Strategic Work**:

```markdown
**Target Audience**: Strategic Agent (Claude Sonnet 4.5)
```

**Implementation Work**:

```markdown
**Target Audience**: Implementation Agent (GPT-5.1-Codex-Max)
```

## ⚠️ Critical

- **Never mix models** - Each role has one correct model
- **Always specify explicitly** - No `<ModelName>` placeholders in production
- **Strategic = Claude Sonnet 4.5** - Planning, architecture, review
- **Implementation = GPT-5.1-Codex-Max** - Code generation, execution

See [ModelRequirements.md](ModelRequirements.md) for detailed rationale.
