#!/usr/bin/env pwsh

if (Get-Command xeyth-git -ErrorAction SilentlyContinue) {
    xeyth-git prepare-commit-msg @args
    exit $LASTEXITCODE
}

Write-Warning "xeyth-git not installed; skipping commit message templating"
exit 0
