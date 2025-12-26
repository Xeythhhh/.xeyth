#!/usr/bin/env pwsh

if (Get-Command xeyth-git -ErrorAction SilentlyContinue) {
    exit (xeyth-git prepare-commit-msg @args)
}

Write-Warning "xeyth-git not installed; skipping commit message templating"
exit 0
