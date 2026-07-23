---
name: new-system
description: Scaffold a new gameplay system (folder, namespace, interfaces, config class, docs entries). Use when creating a system like Infection, Tasks, Cameras, UI, Core.
---

# New system scaffold

1. Read `Docs/agents/map.md` (planned-systems list and existing contracts) and
   the relevant `Docs/agents/gdd/*.md` extract.
2. Create `Assets/_Project/Scripts/<System>/`, namespace `Game.<System>`:
   - `Interfaces/I<Thing>.cs` — the contracts other systems will consume.
     Design these first; keep them minimal.
   - `<System>Config.cs : ScriptableObject` with
     `[CreateAssetMenu(menuName = "KONTUR/<System>/<System>Config")]` — all
     tunables live here, no magic numbers (GDD marks many values TBD: expose
     them as fields with sensible defaults and a `// TBD per GDD` comment).
   - Implementation classes, one per file.
3. Cross-system references: consume other systems ONLY via their interfaces.
   If you need another system's internals, stop and flag it in the handoff.
4. Do NOT create `.asset` instances by hand — list "create the config asset via
   the CreateAssetMenu and wire it" as a human handoff step (or a `scene-edit`
   editor tool).
5. Update the docs in the same change:
   - `Docs/agents/map.md` — add the folder lines and any new contracts to the table.
   - `Docs/agents/systems/<system>.md` — new brief; copy the structure of
     `systems/ai.md` (what it is, key classes, rules of thumb). Keep it ≤50 lines.
6. Verify: `compile-check` skill, then `pwsh Tools/check-metas.ps1` (new files
   won't have metas until Unity opens — flag that in the handoff).
