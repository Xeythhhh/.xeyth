# Copilot Cloud Agent Configuration Guide

**Last Updated**: 2025-12-26  
**Version**: 1.0  
**Audience**: Developers and maintainers of the AI Framework

---

## Quick Start

This guide provides step-by-step instructions for configuring GitHub Copilot Cloud agents to work with the AI Framework.

### Prerequisites

- GitHub Copilot Business or Enterprise subscription
- VS Code with GitHub Copilot extension
- Access to repository settings (for MCP configuration)
- GitHub Personal Access Token (for MCP servers)

---

## 1. Custom Agents Setup

Custom agents define specialized AI "teammates" for the framework's Strategic and Implementation roles.

### Files Created

- `.github/agents/strategic-agent.agent.md` - Orchestrator, Planner, Reviewer roles
- `.github/agents/implementation-agent.agent.md` - Implementer role

### Configuration

Both agents are configured with:
- `target: github-copilot` - Available in all Copilot environments
- `tools: ["*"]` - Access to all available tools
- `infer: true` - Auto-select when contextually relevant
- Model requirements specified in metadata

### Usage

Agents are automatically loaded when:
- Opening the repository in VS Code with Copilot
- Working with Copilot on GitHub.com
- Using Copilot CLI in the repository

Agents auto-activate based on context. You can also explicitly mention them:
- "@strategic-agent please review this PR"
- "@implementation-agent implement this feature"

---

## 2. Agent Skills Setup

Skills provide reusable task templates for common workflows.

### Files Created

- `.github/skills/task-management/SKILL.md` - Task file lifecycle management

### Usage

Skills auto-load when contextually relevant. For example:
- Creating a new `.task` file → task-management skill loads
- Updating Progress Log → task-management skill provides guidance
- Archiving completed tasks → task-management skill shows conventions

---

## 3. MCP Server Configuration

MCP (Model Context Protocol) enables external API integrations for Copilot agents.

### Repository-Level Configuration

**Option 1: GitHub Repository Settings** (Recommended for team use)

1. Navigate to **Repository Settings → Copilot → Coding agent**
2. Add MCP server configuration:

```json
{
  "mcpServers": {
    "github": {
      "command": "npx github-mcp-server",
      "args": ["--token", "${COPILOT_MCP_GITHUB_TOKEN}"]
    }
  }
}
```

3. Add secret `COPILOT_MCP_GITHUB_TOKEN` in repository settings:
   - Settings → Secrets and variables → Codespaces → New repository secret
   - Name: `COPILOT_MCP_GITHUB_TOKEN`
   - Value: Your GitHub PAT with repo and workflow permissions

### Workspace-Level Configuration

**Option 2: VS Code Workspace** (For individual developers)

Configuration file: `.vscode/mcp.json` (already created)

To use:
1. Open repository in VS Code
2. When Copilot prompts, enter your GitHub PAT
3. PAT is stored securely in VS Code keychain

### Available MCP Servers

**GitHub MCP Server** (`github-mcp-server`):
- Query issues and PRs
- Create/update issues
- Manage GitHub Actions workflows
- Search code and files
- Cross-repo operations

**Install via npm**:
```bash
npm install -g github-mcp-server
```

### Security Notes

- Use `COPILOT_MCP_` prefix for all MCP secrets
- Prefer OAuth 2.0 for production use
- Only install trusted MCP servers
- Review MCP server permissions before installation

---

## 4. Environment Setup Workflow

The setup workflow preinstalls global tools in the agent environment.

### Files Created

- `.github/workflows/copilot-setup-steps.yml` - Tool installation workflow

### What It Installs

- **GitHub CLI** (`gh`) - For PR management and GitHub API access
- **Copilot CLI** - For agent-to-agent communication
- **.NET SDK** - For building and testing framework code
- **.NET Global Tools** - Framework-specific tools (when available)

### Important Limitations

⚠️ **CRITICAL**: This workflow runs only during setup phase, NOT during agent runtime.

- Tools installed here are available during initial environment setup
- Each agent task runs in a **fresh, ephemeral environment**
- No persistent state across agent invocations
- Tools must be reinstalled for each new agent task

**Workaround**: Use MCP servers for runtime tool access instead of relying on preinstalled tools.

### Running the Workflow

**Manual trigger** (for testing):
```bash
gh workflow run copilot-setup-steps.yml
```

The workflow is primarily for documentation purposes, showing what tools would be available in an ideal setup.

---

## 5. Environment Variables and Secrets

### What's Available

| Variable         | Setup Phase | Runtime | Notes                           |
|------------------|-------------|---------|--------------------------------|
| `GITHUB_TOKEN`   | ✅ Yes      | ❌ NO   | Security restriction            |
| `PATH`           | ✅ Yes      | ⚠️ Partial | Standard paths only         |
| Custom vars      | ✅ Yes      | ❌ NO   | Cannot pass to agent            |
| `COPILOT_MCP_*`  | ✅ Yes      | ✅ Yes  | For MCP server auth only        |

### Critical Restrictions

For security reasons, Copilot Cloud agents do NOT have access to:
- Repository secrets (including `GITHUB_TOKEN`)
- Custom environment variables
- Persistent environment state

### Recommended Approaches

**Instead of environment variables, use**:
1. **Git-tracked configuration files** - Commit config to repository
2. **MCP servers** - For external API access with `COPILOT_MCP_*` secrets
3. **Repository settings** - For sensitive values (accessed via MCP)
4. **PR workflow** - State persists via commits and pull requests

---

## 6. Agent Invocation Patterns

### Current Pattern: Code Block Delegation

Agents delegate to each other using 4-backtick code blocks:

````markdown
**Task**: [Framework/TaskName.task](Framework/TaskName.task)
**Role**: Implementer (see [Framework/Implementation.prompt.md](Framework/Implementation.prompt.md))

{1-2 sentence context}
````

**Why**: The `runSubagent` API exists but is not directly accessible in current Copilot setup.

### Future Pattern: Programmatic Invocation

When `runSubagent` API becomes available:

```javascript
// Agent (Orchestrator) could invoke Agent directly
await runSubagent({
  prompt: "Full delegation prompt from task file",
  description: "TaskName - Implementation"
});
```

**Benefits**:
- Automatic agent spawning (no manual copy/paste)
- Parallel task execution
- Improved workflow automation
- Real-time monitoring of agent progress

**Framework is prepared**: See `Framework/Strategic.prompt.md` for automation workflow ready for when tools become available.

---

## 7. Workspace Persistence

### Ephemeral Environment Model

Each Copilot Cloud agent task:
1. Spins up fresh environment (GitHub Actions runner)
2. Clones repository
3. Executes work
4. Creates branch and PR
5. Tears down environment

**No persistent state** between invocations.

### State Management Strategy

**State persists via**:
- Git commits on `copilot/*` branches
- Pull request descriptions and comments
- Task files and progress reports (Git-tracked)
- Issue assignments and labels

**Does NOT persist**:
- Environment variables
- Installed packages (unless committed)
- File system changes outside repo
- In-memory state

### Implications for Framework

- All configuration must be in Git-tracked files
- Use `.task`, `.report`, `.convention` files for state
- PRs are the communication channel between agents
- No "session memory" - each invocation is fresh start

---

## 8. Testing the Configuration

### Verify Custom Agents

1. Open repository in VS Code with Copilot
2. Open Copilot Chat
3. Type: "@strategic-agent what are your responsibilities?"
4. Agent should respond with role information

### Verify MCP Integration

1. Configure MCP server (see Section 3)
2. In Copilot Chat, ask: "List open issues in this repository"
3. If MCP is working, Copilot will query GitHub API via MCP server

### Verify Agent Skills

1. Create a new task file: `Test/Example.task`
2. In Copilot Chat, ask: "How do I structure a task file?"
3. task-management skill should auto-load and provide guidance

---

## 9. Troubleshooting

### Custom Agents Not Loading

**Problem**: Agents don't appear in Copilot
**Solution**:
- Verify files are in `.github/agents/` directory
- Check YAML frontmatter syntax (must be valid)
- Ensure `target: github-copilot` is set
- Reload VS Code window

### MCP Server Connection Failed

**Problem**: MCP server cannot connect
**Solution**:
- Verify `COPILOT_MCP_*` secret is set correctly
- Check PAT has required permissions (repo, workflow)
- Test MCP server independently: `npx github-mcp-server`
- Review MCP server logs in Output panel

### Environment Variables Not Available

**Problem**: Agent can't access GITHUB_TOKEN
**Solution**:
- This is expected behavior (security restriction)
- Use MCP servers for GitHub API access instead
- Embed configuration in Git-tracked files
- Don't rely on environment variables during runtime

### Agent Invocation Not Working

**Problem**: runSubagent API not available
**Solution**:
- This is expected in current setup
- Use code block delegation pattern (documented in `Framework/Delegation.instructions.md`)
- Monitor GitHub changelog for API availability updates
- Framework is prepared for future automation

---

## 10. Best Practices

### Security

- ✅ Use `COPILOT_MCP_` prefix for all MCP secrets
- ✅ Prefer OAuth 2.0 for production MCP servers
- ✅ Only install trusted MCP servers from known publishers
- ✅ Review agent permissions regularly
- ❌ Never commit secrets to repository
- ❌ Don't try to expose GITHUB_TOKEN to agents

### Configuration Management

- ✅ Keep custom agents and skills in Git version control
- ✅ Document agent responsibilities in agent files
- ✅ Use metadata for agent organization
- ✅ Test configuration changes in draft PRs first
- ✅ Share MCP configuration across team via repository settings

### Workflow Optimization

- ✅ Use `infer: true` for automatic agent selection
- ✅ Create specialized agents for domain-specific tasks
- ✅ Define skills for repetitive workflows
- ✅ Leverage MCP for external integrations
- ❌ Don't rely on persistent environment state
- ❌ Don't expect tools installed in setup to be available at runtime

---

## 11. Future Enhancements

### When runSubagent API Becomes Available

The framework is prepared for automatic agent invocation:

**Orchestrator workflow** (from `Framework/Strategic.prompt.md`):
1. Commit and push orchestrator work to master
2. Update PR branches if behind master
3. Programmatically invoke Agents:
   ```javascript
   runSubagent(
     prompt: "{Full delegation prompt}",
     description: "{TaskName} - Implementation"
   )
   ```
4. Monitor active agents via PR status
5. Post refinement comments automatically
6. Approve/merge when ready

**No code changes needed** - workflow is already documented and agents are prepared.

### Advanced MCP Integrations

**Custom MCP servers** for:
- Task priority calculation
- Contract validation
- Documentation generation
- Architecture compliance checking
- Cross-repo dependency analysis

**Multi-agent orchestration**:
- Main agent spawns subagents for parallel work
- Subagents report back via MCP protocol
- Orchestrator coordinates and merges results

---

## 12. Quick Reference

### File Locations

| Type                | Location                                  |
|---------------------|-------------------------------------------|
| Custom agents       | `.github/agents/*.agent.md`               |
| Agent skills        | `.github/skills/*/SKILL.md`               |
| MCP config (VS Code)| `.vscode/mcp.json`                        |
| Setup workflow      | `.github/workflows/copilot-setup-steps.yml`|
| Instruction files   | `.github/copilot-instructions.md`, `Framework/copilot-instructions.md` |

### Key Documents

| Document            | Purpose                                   |
|---------------------|-------------------------------------------|
| `Strategic.prompt.md` | Orchestrator/Planner/Reviewer workflows |
| `Implementation.prompt.md` | Implementer workflow               |
| `Delegation.instructions.md` | Cross-model delegation format    |
| `Flow.prompt.md`    | Continuous flow automation                |
| `CopilotCloudAgentConfiguration.research.report` | Detailed research findings |

### Command Reference

```bash
# Test MCP server
npx github-mcp-server

# Install Copilot CLI
npm install -g @github/copilot

# Run setup workflow
gh workflow run copilot-setup-steps.yml

# List custom agents (manual)
ls -la .github/agents/

# List skills (manual)
ls -la .github/skills/
```

---

## 13. Support and Resources

### Internal Documentation

- Full research report: [CopilotCloudAgentConfiguration.research.report](CopilotCloudAgentConfiguration.research.report)
- Task file: [CopilotCloudAgentConfiguration.task](CopilotCloudAgentConfiguration.task)
- Strategic agent workflow: [Strategic.prompt.md](Strategic.prompt.md)
- Implementation agent workflow: [Implementation.prompt.md](Implementation.prompt.md)

### External Resources

- [GitHub Docs: Custom Agents](https://docs.github.com/en/copilot/reference/custom-agents-configuration)
- [GitHub Docs: MCP Integration](https://docs.github.com/enterprise-cloud@latest/copilot/using-github-copilot/coding-agent/extending-copilot-coding-agent-with-mcp)
- [VS Code: Agent Skills](https://code.visualstudio.com/docs/copilot/customization/agent-skills)
- [GitHub Blog: Agent Mode 101](https://github.blog/ai-and-ml/github-copilot/agent-mode-101-all-about-github-copilots-powerful-mode/)

---

**Configuration Version**: 1.0  
**Last Reviewed**: 2025-12-26  
**Next Review**: When runSubagent API becomes available or major Copilot updates released
