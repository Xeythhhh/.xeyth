# Task Priority System

**Purpose**: Define task priority scoring for efficient orchestration and backlog management

## Priority Metadata

Tasks include a `Priority:` field with values: `Critical`, `High`, `Medium`, `Low`

## Priority Scoring Algorithm

```
Score = ImpactScore + UrgencyScore - ComplexityPenalty + DependencyBonus

Where:
- ImpactScore: 10 (Critical) | 7 (High) | 4 (Medium) | 1 (Low)
- UrgencyScore: 5 (Blocker) | 3 (Near-term) | 1 (Future)
- ComplexityPenalty: 3 (Large) | 2 (Medium) | 0 (Small)
- DependencyBonus: 5 (Unblocks others) | 0 (Independent)
```

## Priority Guidelines

### Critical (10+ points)
- Blocks all other work
- Security/data loss risk
- Production incidents
- Examples: Build broken, git repository corrupted, critical bug

### High (7-9 points)
- Blocks multiple tasks
- Framework foundations (affects all downstream work)
- High-value user-facing features
- Examples: Core validation system, contract metadata, primary workflows

### Medium (4-6 points)
- Affects specific workflows
- Quality improvements
- Documentation
- Examples: Lint fixes, footer standardization, renderer improvements

### Low (1-3 points)
- Nice-to-have enhancements
- Cosmetic improvements
- Low-impact features
- Examples: Research tasks, legacy migration (non-blocking)

## Task Selection Rules

1. **Always take highest priority** available task
2. **Prefer unblocking tasks** (high DependencyBonus)
3. **Balance complexity** - mix Large tasks with Small wins
4. **Maintain backlog** between 5-10 ready tasks
5. **Create tasks** when backlog < 5
6. **Defer new planning** when backlog > 10

## Metadata Extraction Pattern

From task files:
```
Status: {Not Started|In Progress|Blocked|Complete}
Priority: {Critical|High|Medium|Low}
Effort: {Small|Medium|Large}
Owner: {Role}
```

## Active Task Cache

To optimize orchestration, maintain in-memory cache of:
- Task file path
- Priority score
- Status
- Blocking dependencies
- Estimated effort

Cache invalidation: When task files are modified or new tasks created.

## Implementation

Future automation tool: `xeyth-planning list --priority-order` to query tasks by priority score.
