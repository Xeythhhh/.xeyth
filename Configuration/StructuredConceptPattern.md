# Structured Concept Pattern

**Purpose**: Define the architecture pattern for structured concepts in the .xeyth framework

**Convention**: See [StructuredConceptPattern.convention](../Conventions/StructuredConceptPattern.convention) for formal rules and validation criteria.

## Overview

The .xeyth framework uses a **Structured Concept Pattern** for any file-based concept that has:
1. Specific format/schema (.xeyth, .task, .plan, .proposal, .convention, etc.)
2. Multiple instances across the workspace
3. Need for documentation, templates, and tooling

Each structured concept gets its own slice with standardized components.

## Pattern Structure

For each structured concept, create a slice with:

```
{ConceptName}/
  {ConceptName}.{ext}              # Example instance (optional)
  {ConceptName}Format.md           # Format documentation
  {ConceptName}.{ext}.template     # Template file
  Proposed/                        # For proposals (if applicable)
  archive/                         # Archived instances
  CLI/                             # CLI module for concept (optional)
    {ConceptName}Module.cs         # API for working with concept
    {ConceptName}Parser.cs         # Parse concept files
    {ConceptName}Validator.cs      # Validate concept instances
```

## Slice Components

### 1. Format Documentation (`{ConceptName}Format.md`)

**Purpose**: Complete specification of the concept format

**Contents**:
- Overview and purpose
- File format specification (YAML, Markdown sections, etc.)
- Required vs optional fields
- Validation rules
- Usage examples
- Best practices
- CLI tool integration
- See Also links

**Example**: [ConfigurationFormat.md](ConfigurationFormat.md)

### 2. Template File (`{ConceptName}.{ext}.template`)

**Purpose**: Boilerplate for creating new instances

**Contents**:
- Complete structure with placeholders
- Inline comments explaining each section
- Common patterns and examples
- Placeholders in `{CurlyBraces}` format

**Example**: [Configuration.xeyth.template](Configuration.xeyth.template)

### 3. CLI Module (Optional)

**Purpose**: Simple API for working with concept instances

**Components**:
- **Parser**: Read and parse concept files
- **Validator**: Validate structure and content
- **Creator**: Generate new instances from template
- **Lister**: Find all instances in workspace
- **Updater**: Modify existing instances

**Integration**: Modules loaded by central CLI tools (xeyth-planning, xeyth-config, etc.) or standalone tools

## Existing Structured Concepts

### Configuration (`.xeyth` files)

**Location**: `Configuration/`

**Components**:
- ✅ `Configuration/Configuration.xeyth` - Framework configuration instance
- ✅ `Configuration/ConfigurationFormat.md` - Format specification
- ✅ `Configuration/Configuration.xeyth.template` - Template
- ⏳ `Configuration/CLI/ConfigurationModule.cs` - CLI module (planned)

**CLI Tool**: `xeyth-config` (planned)

**API**:
```csharp
var config = ConfigurationModule.Load("Configuration/Configuration.xeyth");
var maxBacklog = config.Orchestrator.Backlog.Maximum;
ConfigurationModule.Validate(config);
```

### Planning (`.task`, `.plan`, `.report`, `.proposal` files)

**Location**: `Planning/`

**Components**:
- ✅ `Planning/Task.task.template` - Task template
- ✅ `Planning/Plan.plan.template` - Plan template
- ✅ `Planning/Proposal.template` - Proposal template
- ✅ `Planning/ProgressReport.report.template` - Progress report template
- ✅ `Planning/BlockerReport.report.template` - Blocker report template
- ✅ `Planning/ContextFiles.md` - Context file documentation
- ✅ `Planning/ProposalSystem.md` - Proposal system documentation
- ✅ `Planning/TaskPrioritySystem.md` - Task priority documentation
- ✅ `Planning/Proposed/` - Pending proposals directory
- ⏳ `Planning/CLI/TaskModule.cs` - Task parsing/validation (planned)
- ⏳ `Planning/CLI/ProposalModule.cs` - Proposal parsing/validation (planned)

**CLI Tool**: `xeyth-planning` (in progress - [Automation/PlanningCliTool.task](../Automation/PlanningCliTool.task))

**API**:
```csharp
var tasks = TaskModule.FindAll("**/*.task");
var pending = ProposalModule.FindPending();
var task = TaskModule.Parse("Git/AiSliceMigration.task");
TaskModule.Validate(task);
```

### Git (`.convention`, `.gitmessage` files)

**Location**: `Git/`

**Components**:
- ✅ `Git/Commit.convention` - Commit message convention
- ✅ `Git/README.md` - Git workflow documentation
- ✅ `.gitmessage` - Commit message template (root)
- ⏳ `Git/CLI/ConventionModule.cs` - Convention validation (planned)

**CLI Tool**: Git hooks (commit-msg) + `xeyth-git` (planned)

**API**:
```csharp
var conventions = ConventionModule.FindAll("**/*.convention");
var commitMsg = ConventionModule.ValidateCommitMessage(message);
```

### Contracts (`.metadata`, YAML schema files)

**Location**: `Contracts/`

**Components**:
- ✅ `Contracts.Core/` - Contract validation library
- ✅ `Contracts.Cli/` - xeyth-contracts CLI tool
- ⏳ `Contracts/ContractFormat.md` - Format documentation (planned)
- ⏳ `Contracts/Contract.metadata.template` - Template (planned)

**CLI Tool**: `xeyth-contracts` (implemented)

**API**:
```csharp
var contracts = ContractModule.Discover("**/*.metadata");
var result = ContractModule.Validate(contract);
```

## Pattern Benefits

### 1. Discoverability

Users know where to find:
- Documentation: `{Concept}/{Concept}Format.md`
- Template: `{Concept}/{Concept}.{ext}.template`
- Examples: `{Concept}/` or subdirectories

### 2. Consistency

All concepts follow same structure:
- Format documentation in predictable location
- Templates use same placeholder convention
- CLI modules have consistent API

### 3. Tooling Integration

CLI modules provide simple APIs:
- Central tools load modules as needed
- Plugins/extensions can add new concepts
- Uniform parsing/validation across concepts

### 4. Self-Documenting

Each concept is self-contained:
- Format documentation explains structure
- Template shows complete example
- Examples demonstrate usage

### 5. Extensibility

New concepts follow same pattern:
- Create slice directory
- Add format documentation
- Create template
- Implement CLI module (optional)
- Register with central tools

## Creating New Structured Concepts

### Step 1: Identify Need

Concept qualifies if:
- Multiple instances will exist
- Specific format/schema required
- Users need templates and documentation
- Tooling would add value (parsing, validation, creation)

### Step 2: Create Slice

```bash
mkdir {ConceptName}
```

### Step 3: Document Format

Create `{ConceptName}/{ConceptName}Format.md`:
- Overview and purpose
- Format specification
- Required vs optional fields
- Validation rules
- Examples
- CLI tool usage

### Step 4: Create Template

Create `{ConceptName}/{ConceptName}.{ext}.template`:
- Complete structure
- Placeholder format: `{PlaceholderName}`
- Inline comments
- Common patterns

### Step 5: Implement CLI Module (Optional)

Create `{ConceptName}/CLI/{ConceptName}Module.cs`:
- Parser: Load and parse files
- Validator: Check structure/content
- Creator: Generate from template
- Lister: Find instances
- Updater: Modify instances

### Step 6: Integrate with Tools

Register module with central CLI tool:
```csharp
// In xeyth-planning or relevant tool
var modules = new[]
{
    new TaskModule(),
    new ProposalModule(),
    new {ConceptName}Module()
};
```

## Anti-Patterns

❌ **Don't**:
- Create slice for single-use files
- Mix multiple unrelated concepts in one slice
- Skip documentation ("code is self-documenting")
- Create CLI module without clear use case
- Duplicate documentation across slices

✅ **Do**:
- Group related concept types together (tasks, plans, reports in Planning/)
- Document format thoroughly with examples
- Create templates for consistency
- Implement CLI modules when automation adds value
- Link between related concepts

## Example: Creating a `.review` Concept

### 1. Identify Need

**Purpose**: Structured code review feedback files

**Format**: Markdown with review metadata

**Instances**: Multiple reviews per repository

**Tooling**: Parse reviews, aggregate feedback, track approvals

### 2. Create Slice

```bash
mkdir Reviews
```

### 3. Document Format

Create `Reviews/ReviewFormat.md`:
```markdown
# Review File Format

**Purpose**: Structured code review feedback

## Format

\```markdown
# Review: {Title}

**Reviewer**: {Name}
**Date**: YYYY-MM-DD
**Status**: Approved | Changes Requested | Comment

## Summary
{Overall assessment}

## Comments
- File: {path}
  Line: {number}
  Severity: Critical | Major | Minor
  Comment: {feedback}
\```
```

### 4. Create Template

Create `Reviews/Review.template`:
```markdown
# Review: {Title}

**Reviewer**: {ReviewerName}
**Date**: {YYYY-MM-DD}
**Status**: Approved | Changes Requested | Comment

## Summary
{Overall assessment of the changes}

## Comments
- File: {FilePath}
  Line: {LineNumber}
  Severity: Critical | Major | Minor
  Comment: {Feedback text}
```

### 5. Implement Module

Create `Reviews/CLI/ReviewModule.cs`:
```csharp
public class ReviewModule
{
    public static Review Parse(string path) { /* ... */ }
    public static IEnumerable<Review> FindAll(string pattern) { /* ... */ }
    public static bool Validate(Review review) { /* ... */ }
    public static void Create(string path, ReviewTemplate template) { /* ... */ }
}
```

### 6. Integrate

Add to xeyth-planning or create xeyth-review tool:
```bash
xeyth-planning list-reviews --pending
xeyth-planning show-review MyReview
xeyth-planning create-review --template Reviews/Review.template
```

## See Also

- [Planning/ContextFiles.md](../Planning/ContextFiles.md) - Context file system overview
- [Planning/ProposalSystem.md](../Planning/ProposalSystem.md) - Proposal concept example
- [ConfigurationFormat.md](ConfigurationFormat.md) - Configuration concept example
- [Automation/PlanningCliTool.task](../Automation/PlanningCliTool.task) - CLI tool for proposals/tasks

