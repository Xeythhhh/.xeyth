#!/usr/bin/env pwsh

if (Get-Command xeyth-git -ErrorAction SilentlyContinue) {
    xeyth-git commit-msg @args
    exit $LASTEXITCODE
}

Write-Warning "xeyth-git not installed; skipping commit message validation"
exit 0
