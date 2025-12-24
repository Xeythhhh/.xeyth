# Convention File Format

**Purpose**: Define the structure and format for `.convention` files in the .xeyth framework

## Overview

Convention files (`.convention`) capture formal rules, patterns, and standards that apply across the framework or specific slices. They provide a structured, discoverable way to document conventions that agents and developers must follow.

## File Format

Convention files use **YAML frontmatter** followed by **Markdown content**.

### Basic Structure

```markdown
---
type: Architecture Pattern | Coding Standard | Process | Naming | File Format | Documentation
scope: Framework-wide | Slice-specific | Project-specific
applies_to: All files | Specific file types | Specific roles | {custom}
status: Active | Deprecated | Proposed | Under Review
---

# {Convention Name}

## Convention Summary

{One-paragraph summary of what this convention defines}

## Rules

{Specific, actionable rules that must be followed}

## Examples

{Code examples, patterns, or demonstrations}

## Validation

{How to verify compliance with this convention}

## See Also

{Links to related conventions, documentation, or tools}
```

## YAML Frontmatter

### Required Fields

**type** (Required):
- **Architecture Pattern**: Structural design decisions (e.g., Structured Concept Pattern, Vertical Slices)
- **Coding Standard**: Code style and quality rules (e.g., naming, formatting)
- **Process**: Workflow and procedural conventions (e.g., commit format, PR process)
- **Naming**: File, directory, or identifier naming rules
- **File Format**: Specifications for file structures and schemas
- **Documentation**: Documentation style and organization rules

**scope** (Required):
- **Framework-wide**: Applies to all .xeyth framework files
- **Slice-specific**: Applies only within a specific slice
- **Project-specific**: Applies to host projects using .xeyth

**applies_to** (Required):
- **All files**: Convention applies universally
- **Specific file types**: `*.task`, `*.plan`, `*.cs`, etc.
- **Specific roles**: `Orchestrator`, `Implementer`, etc.
- **Custom**: Any specific scope (tools, documentation, etc.)

**status** (Required):
- **Active**: Currently enforced
- **Deprecated**: Being phased out (include migration path)
- **Proposed**: Under review, not yet enforced
- **Under Review**: Being evaluated for changes

### Optional Fields

**version**: Convention version number (e.g., `1.0`, `2.1`)

**deprecated_by**: Path to replacement convention (if status: Deprecated)

**enforcement**: How convention is enforced
- `Manual` - Developers/agents verify
- `Linter` - Automated linting tool
- `CI/CD` - Pipeline validation
- `Pre-commit Hook` - Git hook validation

**related_tools**: Tools that work with this convention
- CLI tools (e.g., `xeyth-conventions`)
- Linters
- Analyzers

## Markdown Content Sections

### Convention Summary (Required)

One paragraph explaining:
- What the convention governs
- Why it exists
- Who it applies to

### Rules (Required)

Specific, actionable rules using:
- **✅ MUST**: Mandatory requirements
- **❌ MUST NOT**: Prohibited patterns
- **✅ SHOULD**: Recommended practices
- **⚠️ MAY**: Optional but suggested

Example:
```markdown
## Rules

- ✅ MUST use PascalCase for slice directory names
- ❌ MUST NOT create slices with fewer than 3 files
- ✅ SHOULD include Format.md for structured concepts
- ⚠️ MAY create CLI module for automation
```

### Examples (Required)

Demonstrate:
- ✅ Good patterns (what to do)
- ❌ Bad patterns (what to avoid)
- Real-world usage from the framework

### Validation (Required)

Explain how to check compliance:
- Manual verification steps
- Automated tools/commands
- CI/CD checks
- Quality gates

### See Also (Optional but Recommended)

Link to:
- Related conventions
- Documentation
- Templates
- Tools
- Examples in codebase

## Naming Conventions

### File Naming

Pattern: `{ConventionName}.convention`

**Examples**:
- `StructuredConceptPattern.convention` - Architecture pattern
- `CommitMessageFormat.convention` - Process convention
- `TaskFileStructure.convention` - File format convention
- `NamingConventions.convention` - Naming rules

**Rules**:
- Use PascalCase for multi-word names
- Be descriptive and specific
- Avoid redundancy (don't use "Convention" in the name unless discussing conventions themselves)

### Directory Structure

**Framework-wide conventions**: `Conventions/`

**Slice-specific conventions**: `{Slice}/Conventions/` or co-located with related files

**Example**:
```
Conventions/
  StructuredConceptPattern.convention
  NamingConventions.convention
  MarkdownFormatting.convention

Git/
  CommitMessageFormat.convention
  BranchingStrategy.convention
```

## Usage Examples

### Example 1: Architecture Pattern

```markdown
---
type: Architecture Pattern
scope: Framework-wide
applies_to: All slices with structured concepts
status: Active
enforcement: Manual
version: 1.0
---

# Structured Concept Pattern

## Convention Summary

All file-based structured concepts (file types with specific formats,
multiple instances, need for documentation/templates/tooling) MUST
follow the Structured Concept Pattern architecture.

## Rules

- ✅ MUST create Format.md documentation
- ✅ MUST create .template file
- ✅ SHOULD create CLI module for automation
- ❌ MUST NOT create structured concept for single-use files

## Examples

✅ **Good**: Configuration slice
- ConfigurationFormat.md
- Configuration.xeyth.template
- Configuration.xeyth (instance)

❌ **Bad**: Ad-hoc markdown file without schema

## Validation

- [ ] Format.md exists and documents schema
- [ ] Template exists with placeholders
- [ ] At least 3 instances (current or planned)
- [ ] CLI module justified (if included)

## See Also

- [StructuredConceptPattern.md](../Configuration/StructuredConceptPattern.md)
- [Planning/ContextFiles.md](../Planning/ContextFiles.md)
```

### Example 2: Naming Convention

```markdown
---
type: Naming
scope: Framework-wide
applies_to: All files and directories
status: Active
enforcement: Manual
---

# File and Directory Naming

## Convention Summary

Defines naming conventions for files, directories, and identifiers
across the .xeyth framework to ensure consistency and discoverability.

## Rules

### Directories

- ✅ MUST use PascalCase for slice names (`Planning`, `Automation`)
- ✅ MUST use singular form (`Convention` not `Conventions`)
- ❌ MUST NOT use abbreviations unless widely understood

### Files

- ✅ MUST use PascalCase for primary content files
- ✅ MUST use descriptive names indicating purpose
- ✅ MUST use appropriate extensions (`.task`, `.plan`, `.convention`)

### Context Files

- ✅ MUST match pattern: `{Name}.{Type}` or `{Name}.{Type}.{Subtype}`
- Examples: `Task.task`, `Progress.report`, `Task.DiagnosticBlocker.report`

## Examples

✅ **Good**:
- `Planning/Task.task.template`
- `Conventions/NamingConventions.convention`
- `Git/CommitMessageFormat.convention`

❌ **Bad**:
- `planning/task_template.md` (lowercase, underscores)
- `Convs/naming.conv` (abbreviations, non-standard extension)

## Validation

Review file and directory names for:
- Case consistency (PascalCase)
- Descriptive clarity
- Proper extensions
- No abbreviations

## See Also

- [Planning/ContextFiles.md](../Planning/ContextFiles.md)
- [Git/README.md](../Git/README.md)
```

### Example 3: Process Convention

```markdown
---
type: Process
scope: Framework-wide
applies_to: All commits
status: Active
enforcement: Pre-commit Hook
related_tools: [git-hooks, xeyth-git]
---

# Commit Message Format

## Convention Summary

All commits MUST follow Conventional Commits format with specific
types, scopes, and formatting rules enforced by git hooks.

## Rules

### Format

- ✅ MUST use format: `<type>(<scope>): <subject>`
- ✅ MUST keep subject ≤ 50 characters
- ✅ MUST use imperative mood ("add" not "added")
- ✅ MUST start subject with lowercase
- ❌ MUST NOT end subject with period

### Types

- `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`,
  `chore`, `ci`, `build`, `revert`

### Body

- ✅ SHOULD wrap at 72 characters
- ✅ SHOULD explain what and why, not how

## Examples

✅ **Good**:
```
feat(planning): add task priority scoring system

Implements priority calculation based on impact, urgency,
dependency, and complexity factors.

Refs: Planning/TaskPrioritySystem.md
```

❌ **Bad**:
```
Added new feature for tasks.
```

## Validation

Pre-commit hook validates:
- Type is valid
- Subject length ≤ 50 characters
- Subject format (imperative, lowercase, no period)
- Body line length ≤ 72 characters

Bypass (not recommended): `git commit --no-verify`

## See Also

- [Git/README.md](../Git/README.md)
- [.gitmessage](../.gitmessage)
```

## CLI Tool Integration

The `xeyth-conventions` tool (planned) will support:

```bash
# List all conventions
xeyth-conventions list

# Show specific convention
xeyth-conventions show StructuredConceptPattern

# Validate convention file format
xeyth-conventions validate Conventions/NamingConventions.convention

# Check compliance with convention
xeyth-conventions check-compliance StructuredConceptPattern --path .

# Create new convention from template
xeyth-conventions create --type "Architecture Pattern" --name MyPattern
```

## Validation Rules

Convention files MUST:
- ✅ Have valid YAML frontmatter with all required fields
- ✅ Include Convention Summary section
- ✅ Include Rules section with actionable items
- ✅ Include Examples section
- ✅ Include Validation section
- ✅ Use markdown formatting

Convention files SHOULD:
- ✅ Link to related conventions
- ✅ Provide good and bad examples
- ✅ Specify enforcement mechanism
- ✅ Include version number if applicable

## Best Practices

1. **Be Specific**: Rules should be actionable, not vague
2. **Provide Context**: Explain why the convention exists
3. **Include Examples**: Show both good and bad patterns
4. **Define Validation**: Make compliance verifiable
5. **Link Related Content**: Connect to documentation and tools
6. **Version Changes**: Increment version for breaking changes
7. **Deprecation Path**: Document migration for deprecated conventions

## See Also

- [StructuredConceptPattern.convention](StructuredConceptPattern.convention)
- [Convention.convention.template](Convention.convention.template)
- [Planning/ProposalSystem.md](../Planning/ProposalSystem.md)
