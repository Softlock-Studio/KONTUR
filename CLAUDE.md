@AGENTS.md

## Claude Code specifics
- Shared skills (in `.claude/skills/`): `compile-check`, `unity-tests`, `new-system`,
  `scene-edit`. Use them instead of improvising the equivalent.
- Subagents: `scout` (cheap read-only task scoping — use it before non-trivial
  implementation), `unity-reviewer` (pre-PR diff review).
- Per-machine setup (Unity path, csproj generation): `Docs/agents/setup.md`.
  `UNITY_EDITOR_PATH` comes from `.claude/settings.local.json` — never commit it.
