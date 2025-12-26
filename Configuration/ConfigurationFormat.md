# Configuration File Format

**Purpose**: Define the `.xeyth` configuration file format for framework customization

## Overview

The `.xeyth` file is a YAML-based configuration format that allows projects to customize AI Framework behavior without modifying core framework files. Each configuration aspect controls specific framework features (orchestrator backlog, testing tools, archiving conventions, model requirements).

## File Location

- **Framework configuration**: `Configuration/Configuration.xeyth` (framework defaults)
- **Project overrides**: `{ProjectRoot}/.xeyth/Configuration.xeyth` (when framework used as submodule)

## Format Specification

### YAML Structure

```yaml
# .xeyth Framework Configuration
# This file is optional; defaults apply when absent.

orchestrator:
  backlog:
    minimum: 5          # Create new tasks when backlog drops below this
    maximum: 20         # Pause task creation when backlog exceeds this
    createThreshold: 7  # Target backlog size to maintain

testing:
  verify:
    diffTool: VisualStudioCodeInsiders  # Preferred diff viewer for Verify snapshots

conventions:
  archiving:
    dateFormat: "yyyy-MM-dd"  # Date format for archive filenames
    directoryName: "archive"  # Archive directory name

models:
  strategic: "Claude Sonnet 4.5"         # Strategic Agent model requirement
  implementation: "GPT-5.1-Codex-Max"    # Implementation Agent model requirement
```

## Configuration Sections

### orchestrator

Controls Orchestrator agent behavior (task selection, backlog management).

**Properties**:
- `backlog.minimum` (integer): Create new tasks when backlog drops below this threshold
  - Default: 5
  - Typical range: 3-10
  - Lower values: More proactive task creation
  - Higher values: Focus on execution before creating more work

- `backlog.maximum` (integer): Pause task creation when backlog exceeds this threshold
  - Default: 20
  - Typical range: 10-50
  - Lower values: Stay focused, avoid overwhelming backlog
  - Higher values: Accommodate PR workflows with cloud agents

- `backlog.createThreshold` (integer): Target backlog size to maintain
  - Default: 7
  - Typical range: 5-15
  - Orchestrator aims to keep backlog around this size

**Example**:
```yaml
orchestrator:
  backlog:
    minimum: 7
    maximum: 25
    createThreshold: 10
```

### testing

Controls testing tool configuration (test runners, snapshot viewers).

**Properties**:
- `verify.diffTool` (string): Preferred diff viewer for Verify snapshot tests
  - Default: VisualStudioCode
  - Options: VisualStudioCode, VisualStudioCodeInsiders, Rider, VisualStudio, etc.
  - See [Verify documentation](https://github.com/VerifyTests/Verify) for full list

**Example**:
```yaml
testing:
  verify:
    diffTool: Rider
```

### conventions

Controls framework conventions (archiving, naming, formatting).

**Properties**:
- `archiving.dateFormat` (string): Date format for archive filenames
  - Default: "yyyy-MM-dd"
  - Format: .NET date format string
  - Example filename: `TaskName.2024-12-24.task`

- `archiving.directoryName` (string): Archive directory name
  - Default: "archive"
  - Common alternatives: "archived", "completed", "history"
  - Location: `{Slice}/{directoryName}/`

**Example**:
```yaml
conventions:
  archiving:
    dateFormat: "yyyyMMdd"
    directoryName: "completed"
```

### models

Controls model requirements for agent roles (Strategic vs Implementation).

**Properties**:
- `strategic` (string): Model requirement for Strategic Agent roles
  - Default: "Claude Sonnet 4.5"
  - Roles: Orchestrator, Planner, Reviewer
  - Optimized for: Planning, architecture, reasoning

- `implementation` (string): Model requirement for Implementation Agent
  - Default: "GPT-5.1-Codex-Max"
  - Role: Implementer (plan review + code execution)
  - Optimized for: Code generation, API knowledge, deterministic execution

**Example**:
```yaml
models:
  strategic: "Claude Sonnet 4.5"
  implementation: "GPT-5.1-Codex-Max"
```

## Loading Behavior

1. **Framework defaults**: Built-in defaults in framework code
2. **Framework config**: `Configuration/Configuration.xeyth` (if exists)
3. **Project overrides**: `{ProjectRoot}/.xeyth/Configuration.xeyth` (if exists)

**Precedence**: Project overrides > Framework config > Built-in defaults

## Validation Rules

- **File format**: Valid YAML syntax
- **Section names**: Must match specification (orchestrator, testing, conventions, models)
- **Property types**: Integer for counts, string for names/formats
- **Required properties**: None (all optional, defaults apply)
- **Unknown properties**: Ignored (allows forward compatibility)

## CLI Tool Integration

The `xeyth-config` tool (or module) provides configuration management:

```bash
# Show current effective configuration
xeyth-config show

# Validate configuration file
xeyth-config validate Configuration/Configuration.xeyth

# Get specific value
xeyth-config get orchestrator.backlog.maximum

# Set value (updates file)
xeyth-config set orchestrator.backlog.maximum 30

# Reset to defaults
xeyth-config reset
```

## Usage Examples

### High-Volume PR Workflow

For projects with many PRs and cloud agents:

```yaml
orchestrator:
  backlog:
    minimum: 10
    maximum: 50
    createThreshold: 20
```

### Focus Mode

For solo developers wanting minimal backlog:

```yaml
orchestrator:
  backlog:
    minimum: 3
    maximum: 8
    createThreshold: 5
```

### Custom Archive Naming

For ISO 8601 date format:

```yaml
conventions:
  archiving:
    dateFormat: "yyyy-MM-dd"
    directoryName: "archive"
```

### Different Diff Tool

For Rider IDE users:

```yaml
testing:
  verify:
    diffTool: Rider
```

## Best Practices

1. **Start with defaults**: Don't create configuration file unless you need to override
2. **Document changes**: Add comments explaining why you changed defaults
3. **Test after changes**: Verify framework behavior matches expectations
4. **Version control**: Commit configuration file with rationale in commit message
5. **Project-specific**: Use project overrides for team-specific preferences

## File Associations

Configure in workspace settings (`.code-workspace`):

```json
{
  "settings": {
    "files.associations": {
      "*.xeyth": "yaml"
    }
  }
}
```

Enables YAML syntax highlighting, validation, and IntelliSense for `.xeyth` files.

## Schema (Future)

JSON Schema for `.xeyth` files (planned):

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "title": ".xeyth Configuration",
  "type": "object",
  "properties": {
    "orchestrator": { "type": "object" },
    "testing": { "type": "object" },
    "conventions": { "type": "object" },
    "models": { "type": "object" }
  }
}
```

Would enable IDE validation and autocomplete.

## Extension Points

Future configuration sections:

- `reporting`: Report format, verbosity, output paths
- `delegation`: Delegation templates, model selection rules
- `quality`: Code quality gates, linting rules, coverage thresholds
- `planning`: Priority scoring weights, effort estimates
- `proposals`: Auto-accept criteria, review frequency

## See Also

- [Configuration.xeyth.template](Configuration.xeyth.template) - Template file
- [Framework/copilot-instructions.md](../Framework/copilot-instructions.md) - References configuration
- xeyth-config CLI tool - Configuration management (planned)

