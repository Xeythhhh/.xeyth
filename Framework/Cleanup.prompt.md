# Cleanup Agent Guide

Goal: replace placeholders with project specifics, remove template-only guidance, and clean stray/misplaced files.

## Placeholders to Replace

- `<PROJECT_NAME>`, `<CODEBASE_ROOT>`, `<TECH_STACK>`
- `<BUILD_CMD>`, `<TEST_CMD>`, `<LINT_CMD>`, `<DOC_LINT_CMD>`
- `<PRIMARY_ENV>`, `<SECONDARY_ENV>`
- `<OWNER>`, `<REPO_URL>`

## Steps

1. Read host repo context (README, scripts, pipeline) to infer real commands and environments
2. Update all prompts and templates in this package with the real values
3. Trim any sections not relevant to the host repo; keep files concise
4. Remove or relocate stray files (e.g., deprecated prompts, misplaced templates); ensure content sits under `.xeyth/Ai/`
5. Add a short note in the host README pointing to this package, cleanup date, and workspace file-association changes
6. Commit changes in the host repo (not in the submodule) if using vendor overrides

## Output

- Provide a brief summary of replacements made
- List any unresolved placeholders and why
- Note any file moves/deletions performed
- Suggest next fixes if something is unclear

Use the standard footer from `copilot-instructions.md` in responses.
