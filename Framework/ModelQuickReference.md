# Model Quick Reference

**DEPRECATED**: Model-specific requirements have been removed from the AI Framework.

The framework is now model-agnostic. Use any appropriate AI model for each role based on your project's needs and available tools.

## Role Definitions

| Role | Responsibilities |
|------|------------------|
| **Orchestrator** | Task selection, PR management, backlog maintenance |
| **Planner** | Research, design, architecture decisions |
| **Reviewer** | Code review, quality verification, approval |
| **Implementer** | Code generation, implementation, testing |
| **Scaffold** | Boilerplate generation |
| **Cleanup** | Placeholder replacement |

## Delegation Template

```markdown
**Task**: {Slice}/{TaskName}.task
**Role**: {Role} (see Framework/{Prompt}.prompt.md)

{Context}
```

See [Delegation.instructions.md](Delegation.instructions.md) for complete delegation format rules.
