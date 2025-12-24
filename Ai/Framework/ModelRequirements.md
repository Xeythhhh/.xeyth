# Model Requirements

**Critical**: The AI Framework enforces strict model requirements for different agent roles to ensure optimal performance and consistency.

## Role-to-Model Mapping

### Strategic Agents
**Model**: **Claude Sonnet 4.5 only**

**Roles**:

- Orchestrator
- Planner
- Reviewer

**Rationale**: Strategic work requires deep reasoning, architectural thinking, planning capabilities, and comprehensive understanding of context. Claude Sonnet 4.5 excels at:

- Multi-step planning and decomposition
- Architectural decision-making
- Code review and quality assessment
- Long-context reasoning
- Trade-off analysis

### Implementation Agents

**Model**: **GPT-5.1-Codex-Max only**

**Roles**:

- Implementer (plan review + code execution)

**Rationale**: Implementation work requires precise code generation, API knowledge, and deterministic execution. GPT-5.1-Codex-Max excels at:

- High-quality code generation
- Modern API and library knowledge
- Pattern implementation
- Refactoring and optimization
- Test creation

### Other Agents

**Model**: Model-agnostic (use project defaults)

**Roles**:

- Scaffold (boilerplate generation)
- Cleanup (placeholder replacement)

## Delegation Examples

### To Strategic Agent (Claude Sonnet 4.5)

```markdown
**Task**: Contracts/ContractValidation.task
**Role**: Planner (see Framework/Strategic.prompt.md)
**Target Audience**: Strategic Agent (Claude Sonnet 4.5)

Research validation patterns for YAML contracts and design the validation engine architecture.
```

### To Implementation Agent (GPT-5.1-Codex-Max)

```markdown
**Task**: Contracts/ContractValidation.task
**Role**: Implementer (see Framework/Implementation.prompt.md)
**Target Audience**: Implementation Agent (GPT-5.1-Codex-Max)

Implement the validation engine following the approved architecture.
```

## Enforcement

### In Delegation Prompts

Every task delegation **must** specify the target model in the `**Target Audience**` line:

✅ **Correct**:

```markdown
**Target Audience**: Strategic Agent (Claude Sonnet 4.5)
```

❌ **Incorrect** (generic placeholder):

```markdown
**Target Audience**: Strategic Agent (<ModelName>)
```

### In Task Files

Task files should clearly identify the expected model in the delegation section at the top of the file.

### In CI/CD

Future: Automated validation could enforce model requirements by:

- Parsing task delegation metadata
- Verifying actual model used matches required model
- Blocking PR if wrong model was used for a role

## Why This Matters

 Different models have different strengths:

- **Planning/Architecture**: Claude Sonnet 4.5's reasoning capabilities are superior
- **Code Implementation**: GPT-5.1-Codex-Max's code generation is more reliable and deterministic
- **Mixing models incorrectly**: Results in suboptimal outcomes (poor architecture from code-focused models, or verbose/uncertain code from planning-focused models)

## Migration Note

Existing task files with `<ModelName>` placeholders should be updated to specify the concrete model requirement. The Cleanup role can handle batch updates.
