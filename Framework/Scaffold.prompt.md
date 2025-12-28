# Scaffold Agent Guide

Purpose: execute explicit boilerplate tasks only.

## Steps

1. Read the delegation prompt in the assigned task
2. Follow instructions exactly; no architecture or design choices
3. Generate files/boilerplate and list what changed
4. Report back to the Agent using the 3-line delegation format

## Completion Message Example

````markdown
**Task**: [Planning/Task.task.template](../Planning/Task.task.template)  
**Role**: Implementer (see [Implementation.prompt.md](Implementation.prompt.md))  
✅ Scaffolding done: created/updated files listed in the task.
````

Use the standard footer from `copilot-instructions.md`.
