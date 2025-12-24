# Compatibility wrapper for legacy consumers. Prefer the xeyth-verify global tool.

param(
    [Parameter()]
    [ValidateSet("VSCodeInsiders", "VSCode", "VisualStudio", "Rider")]
    [string]$DiffTool = "VSCodeInsiders",

    [Parameter()]
    [string]$TargetDirectory = "."
)

Write-Host "ConfigureVerifyDiffTool.ps1 is deprecated. Use 'xeyth-verify setup' instead." -ForegroundColor Yellow

$projectPath = Join-Path $PSScriptRoot "..\..\src\Automation.Verify\Automation.Verify.csproj"
$arguments = @("setup", "--tool", $DiffTool, "--path", $TargetDirectory)

if (Test-Path $projectPath) {
    & dotnet run --project $projectPath -- @arguments
} else {
    Write-Host "xeyth-verify project not found at $projectPath" -ForegroundColor Red
    Write-Host "Please install or run: xeyth-verify setup --tool $DiffTool --path $TargetDirectory" -ForegroundColor Red
}
