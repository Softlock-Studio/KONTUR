---
name: unity-tests
description: Run Unity EditMode/PlayMode tests in batch mode — the authoritative check. Slow (1-3+ min). Use before PRs, after adding new files, or when runtime behavior changed.
---

# Unity batch-mode tests

Requires `UNITY_EDITOR_PATH` (set per machine in `.claude/settings.local.json`,
see `Docs/agents/setup.md`). **The Unity editor must NOT have this project open**
— batch mode fails on a locked project; ask the user to close it first.

EditMode (fast, no scene loading — prefer for pure logic):
```powershell
& "$env:UNITY_EDITOR_PATH" -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Logs/editmode-results.xml -logFile Logs/unity-tests.log
```
PlayMode: same with `-testPlatform PlayMode -testResults Logs/playmode-results.xml`.

Exit codes: 0 = all passed · 2 = test failures (read the results XML for
failed cases) · anything else = run didn't start; search `Logs/unity-tests.log`
for `error CS` (compile errors) or "already open in another instance".

## First tests in the project
There are no tests yet. To add the first EditMode tests, create
`Assets/_Project/Scripts/Tests/EditMode/` with `Game.Tests.EditMode.asmdef`:
```json
{
  "name": "Game.Tests.EditMode",
  "references": ["UnityEngine.TestRunner", "UnityEditor.TestRunner"],
  "includePlatforms": ["Editor"],
  "optionalUnityReferences": ["TestAssemblies"]
}
```
Note: without a main `Game` asmdef, Assembly-CSharp types are NOT referencable
from a test asmdef. Until asmdefs are introduced project-wide, flag this in your
handoff instead of fighting it — introducing asmdefs is a team decision.
New .asmdef/.cs files need `.meta`s → run `pwsh Tools/check-metas.ps1` before commit.
