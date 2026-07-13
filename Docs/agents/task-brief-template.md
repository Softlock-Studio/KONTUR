# Task brief template

How to hand a task to an agent (or a teammate). The point: a <100k-context model
should never have to explore the repo to figure out what you meant. Scope the input;
don't rely on the model to scope itself. The `scout` subagent can draft one of these.

```markdown
## Goal
One or two sentences. What exists when this is done that doesn't exist now.

## Read first
- Docs/agents/systems/<relevant>.md
- Docs/agents/gdd/<relevant>.md          (only what this task needs)

## Files in scope
- Assets/_Project/Scripts/<...>          (edit)
- Assets/_Project/Scripts/<...>          (create)
Anything not listed: read if needed, don't modify.

## Contracts to respect
- I<Interface> (path) — consume as-is / may extend with <member>

## Out of scope
Explicitly: what NOT to do (e.g. "no scene changes", "don't implement saving").

## Definition of done
- [ ] compile-check passes
- [ ] unity-tests pass (if runtime behavior changed / files added)
- [ ] check-metas.ps1 clean or metas flagged in handoff
- [ ] map.md / system brief updated (if structure changed)
- [ ] Handoff block written (Unity-side steps for a human)
```

## Example (filled)

```markdown
## Goal
Employees emit a noise event when they run, so Babooshka's HearingSensor can react.

## Read first
- Docs/agents/systems/ai.md

## Files in scope
- Assets/_Project/Scripts/AI/Employee/IEmployee.cs        (extend)
- Assets/_Project/Scripts/AI/Employee/EmployeeStub.cs     (edit)
- Assets/_Project/Scripts/AI/Babooshka/Sensors/HearingSensor.cs (edit)

## Contracts to respect
- IEmployee — may add `event Action<Vector3> NoiseMade`
- BabooshkaBlackboard — write LastHeardSound/LastHeardTime only

## Out of scope
No FSM/state changes, no config changes, no scenes.

## Definition of done
- [ ] compile-check passes
- [ ] EditMode test: noise event updates blackboard
- [ ] Handoff: none expected (code only)
```
