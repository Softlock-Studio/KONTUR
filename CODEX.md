# Codex Entry Point

Canonical repo rules live in `AGENTS.md`.

## Codex specifics
- Read `Docs/agents/map.md` before touching code; use it instead of tree-walking.
- Read the relevant `Docs/agents/systems/<system>.md` brief before editing a system.
- Fast verification after C# edits: `dotnet build Assembly-CSharp.csproj --nologo -v q`
  (plus the relevant generated editor/asmdef csproj if applicable).
- Full verification for new files or runtime behavior changes: run Unity in batch
  mode with `UNITY_EDITOR_PATH`; exact commands are in `Docs/agents/setup.md`.
- For scene, prefab, or `.asset` instance changes, write an editor script or leave
  precise Unity-editor handoff steps. Never hand-edit Unity YAML.
- `.claude/` contains legacy Claude helpers. Treat them as reference material unless
  a task explicitly asks to maintain that tooling.
