#!/usr/bin/env pwsh

if (Get-Command xeyth-git -ErrorAction SilentlyContinue) {
    exit (xeyth-git commit-msg @args)
}

Write-Warning "xeyth-git not installed; skipping commit message validation"
exit 0
