# Verify Snapshot Testing with VS Code Insiders

The `.xeyth` framework uses [Verify](https://github.com/VerifyTests/Verify) for snapshot testing and is configured to use **VS Code Insiders** as the diff viewer.

## Configuration

The framework includes:

- [.verify/DiffEngine.json](.verify/DiffEngine.json) - Configures VS Code Insiders as the preferred diff tool
- [ModuleInitializer.cs](src/tests/Contracts.Core.Tests/ModuleInitializer.cs) - Initializes Verify with DiffPlex support
- [Verify.DiffPlex](https://www.nuget.org/packages/Verify.DiffPlex) - Provides inline diff capabilities

## For Consuming Repositories

If you're using `.xeyth` as a submodule and want the same Verify/diff configuration:

### Quick Setup (recommended)

```bash
# Run from your repository root
dotnet run --project .xeyth/src/Automation.Verify -- setup --path . --tool VSCodeInsiders
```

If you install the global tool: `xeyth-verify setup --path . --tool VSCodeInsiders`

This creates `.verify/DiffEngine.json` in your repository with the correct configuration. The legacy PowerShell script now forwards to this tool.

### Manual Setup

1. **Create `.verify/DiffEngine.json` in your repository root:**

```json
{
  "ToolOrder": [
    "VisualStudioCodeInsiders",
    "VisualStudioCode"
  ]
}
```

2. **Add package references to your test projects:**

```xml
<PackageReference Include="Verify.Xunit" Version="31.9.0" />
<PackageReference Include="Verify.DiffPlex" Version="3.1.2" />
```

3. **Add ModuleInitializer.cs to your test project:**

```csharp
using System.Runtime.CompilerServices;

namespace YourNamespace.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyDiffPlex.Initialize();
    }
}
```

## Supported Diff Tools

The setup script supports:
- `VSCodeInsiders` (default)
- `VSCode`
- `VisualStudio`
- `Rider`

## How It Works

When a Verify test fails or needs approval:
1. Verify detects the difference between received and approved snapshots
2. DiffEngine reads `.verify/DiffEngine.json` for tool preferences
3. VS Code Insiders launches with a diff view
4. You can approve/reject changes directly in the editor

## Benefits

- **Consistent workflow** - Same diff viewer across all developers
- **Better diffs** - VS Code Insiders provides excellent diff visualization
- **Fast iterations** - Approve/reject snapshot changes quickly
- **Cross-platform** - Works on Windows, macOS, Linux

## Troubleshooting

If diffs don't open in VS Code Insiders:
- Ensure VS Code Insiders is installed and in PATH
- Check `.verify/DiffEngine.json` exists in repository root
- Verify the package references are correct in your test project
- Check Verify test output for DiffEngine logs
