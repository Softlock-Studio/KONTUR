# KONTUR — Agent Instructions

SCP-adjacent supervisor sim, Unity 6 (6000.0.78f1), URP. The player commands a
K.O.N.T.U.R. ops team containing a fungal outbreak (O-41) in an apartment block,
across night shifts, entirely through a UI: assign employees to rooms/tasks, watch
cameras, keep zone infection inside a moving target corridor, keep the team away
from the infected grandmother ("Babooshka"). The only 3D view is the selected
security-camera feed; the 3D world exists for that feed and the AI simulation.
Design source of truth: `Docs/agents/gdd/` (extracts) → full GDD linked in its README.

## Stack
- uGUI + Input System · DI: VContainer (installed, not yet wired) · FSM: UnityHFSM
- NavMesh (com.unity.ai.navigation) · Unity Test Framework

## Where things live
- ALL project code/assets go under `Assets/_Project/` (Scripts, Art, Configs, Scenes,
  Settings). Never add files at `Assets/` root or in `Assets/Plugins/`.
- Codebase map: `Docs/agents/map.md` — read it INSTEAD of exploring the tree.
- Per-system briefs: `Docs/agents/systems/<system>.md` — read the relevant one before
  editing that system. Game design extracts: `Docs/agents/gdd/`.

## Hard rules (Unity foot-guns)
1. **.meta files.** Moving/renaming anything under `Assets/` must move/rename its
   `.meta` too. Never delete a `.meta` whose asset still exists, never write `.meta`
   content by hand. New files you create get their `.meta` when someone opens Unity —
   before committing, run `pwsh Tools/check-metas.ps1`; if metas are missing, say so
   in the handoff instead of faking them.
2. **Never hand-edit `.unity` / `.prefab` / `.asset` YAML.** For scene, prefab, or
   asset-instance changes, write an editor script a human runs in Unity
   (see skill `scene-edit`) or list manual steps in the handoff.
3. **Renaming a `[SerializeField]` field breaks scene wiring.** Add
   `[FormerlySerializedAs("oldName")]` and keep it until the rename has shipped.
4. Don't touch `ProjectSettings/` or `Packages/manifest.json` unless the task
   explicitly says so.

## Code conventions
- Namespace = folder: `Game.<System>[.<Sub>]` mirrors `Assets/_Project/Scripts/`
  (e.g. `Game.AI.Babooshka`). One top-level class per file, filename = class name.
- Cross-system contracts are interfaces (`IEmployee`, `IInfectionDirector`); depend
  on another system's interfaces, never its concrete classes.
- Tunable data = `ScriptableObject` config classes; `.asset` instances live under
  `Assets/_Project/Configs/` and are created by humans in the editor.
- No singletons, no `FindObjectOfType`. Use serialized references within a
  prefab/scene; use VContainer for cross-system services once scopes exist.
- No per-frame allocations (LINQ, `new`, string concat) in `Update`/`OnLogic`/`Tick`
  paths; cache lookups in `Awake`.
- Logging: `Debug.Log($"[{name}] ...", this)` in MonoBehaviours.

## Verify before you claim done
1. Fast, after every change: build the generated csproj — see skill `compile-check`.
2. Full, before PR or for new files/runtime behavior: batch-mode tests — see skill
   `unity-tests`.
3. `pwsh Tools/check-metas.ps1` if you added/moved/deleted files under `Assets/`.
Report honestly which checks you ran and their results.

## Handoff (end of every task)
Finish with a short handoff block: what changed, what a human must do in the Unity
editor (asset creation, scene wiring, running an editor tool, regenerating metas),
and which checks you ran. Assume the reader has not seen your work.

## Git
- Never commit directly to `main`. Work on feature branches; ask which branch to
  target if not told.
- Commit messages: short imperative summary line.
