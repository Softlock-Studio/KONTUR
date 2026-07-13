---
name: scene-edit
description: Make scene/prefab/asset changes via a one-shot editor script that a human runs inside Unity. Use whenever a task needs .unity/.prefab/.asset modifications — never edit that YAML directly.
---

# Scene changes via editor tool

Never hand-edit `.unity` / `.prefab` / `.asset` files. Instead ship an editor
script the team runs from a menu — reviewable, repeatable, no GUID corruption.

1. Create `Assets/_Project/Scripts/Editor/AgentTools/<Task>Tool.cs`
   (`Editor/` is a Unity special folder → editor-only assembly; safe to keep
   out of builds automatically).
2. Pattern:
```csharp
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.EditorTools
{
    public static class AddInfectionZonesTool
    {
        [MenuItem("Tools/AgentTools/Add Infection Zones To MainScene")]
        public static void Run()
        {
            var scene = EditorSceneManager.OpenScene("Assets/_Project/Scenes/MainScene.unity");
            // mutate: new GameObject(...), AddComponent, wire refs via SerializedObject
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[AgentTools] Add Infection Zones: done");
        }
    }
}
```
3. Make it **idempotent** — check for existing objects before creating; running
   twice must not duplicate anything.
4. Wire object references via `SerializedObject`/`SerializedProperty` (not
   direct field writes) so Undo and prefab overrides behave.
5. Handoff must state: the exact menu path, what the tool changes, and how the
   human verifies the result.
6. One-shot tools get deleted in a follow-up commit once confirmed run; keep
   the tool only if it's genuinely reusable.
