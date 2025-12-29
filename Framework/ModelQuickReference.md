# Model Quick Reference

## Role → Model Mapping

| Role | Model | Use For |
|------|-------|---------|
| **Orchestrator** | Model-agnostic | Select work, prioritize tasks |
| **Planner** | Model-agnostic | Research, design, architecture |
| **Reviewer** | Model-agnostic | Code review, quality gates |
| **Implementer** | Model-agnostic | Code generation, refactoring |
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
**Target Audience**: Strategic Agent
```

**Implementation Work**:

```markdown
**Target Audience**: Implementation Agent
```

## ⚠️ Critical

- **Never mix models** - Each role has one correct model
- **Always specify explicitly** - No `<ModelName>` placeholders in production
- **Strategic** - Planning, architecture, review
- **Implementation** - Code generation, execution

See [ModelRequirements.md](ModelRequirements.md) for detailed rationale.
