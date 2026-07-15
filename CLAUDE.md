Canonical repo rules live in `AGENTS.md`.

This file remains only as a legacy Claude entry point. The Codex-facing entry point
is `CODEX.md`.

## Shared setup
- Per-machine setup (Unity path, csproj generation): `Docs/agents/setup.md`.
- `UNITY_EDITOR_PATH` should exist in the process environment. Claude users may
  still inject it through `.claude/settings.local.json` if they keep that tooling.

## Legacy Claude-only helpers
- Shared skills live in `.claude/skills/`.
- Subagents live in `.claude/agents/`.
