using System.Runtime.CompilerServices;

namespace Contracts.Core.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        // Configure Verify to use DiffPlex for inline diffs
        // And prefer VS Code Insiders for external diff viewer
        VerifyDiffPlex.Initialize();
        
        // NOTE: To use VS Code Insiders as diff viewer:
        // 1. Set environment variable: DiffEngine_ToolOrder=VisualStudioCodeInsiders
        // 2. Or create .verify/DiffEngine.json in repo root with: {"ToolOrder": ["VisualStudioCodeInsiders"]}
    }
}
