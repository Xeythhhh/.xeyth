# Contracts CLI

Command-line entry point for validating files against contract metadata.

## Validate

```bash
xeyth-contracts validate --path <file|dir> [--contract <name>] [--strict]
```

- `--path, -p`: File or directory to validate (defaults to current directory)
- `--contract, -c`: Filter by contract metadata file name (without `.metadata`)
- `--strict`: Treat warnings as errors for CI/CD
- Exit codes: 0 (success or warnings), 1 (errors or strict warnings), 2 (usage errors)

### Examples

- Validate the current directory: `xeyth-contracts validate`
- Validate a single file: `xeyth-contracts validate --path Planning/Task.task`
- Run in strict mode: `xeyth-contracts validate --path Planning --strict`
