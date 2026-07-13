# Scheduled agents

Headless Claude Code jobs run by Windows Task Scheduler on a dev machine.
Opt in by running `register-tasks.ps1` once (per-user, no admin). Prereqs:
`claude` CLI logged in; `gh` CLI logged in (reviewer files GitHub issues).

| Task | When | Model | What it does |
|---|---|---|---|
| KONTUR GDD Sync | daily 12:00 | claude-sonnet-4-6 | Downloads the designer's Google Doc, reconciles `Docs/agents/gdd/` extracts with it in a worktree (`../KONTUR-gdd-sync`, branch `chore/gdd-sync`, always reset to development + at most one fresh commit, force-with-lease pushed to origin). Merge the commit into `development` when you're happy with it. |
| KONTUR Code Review | daily 01:00 + 12:00 | claude-opus-4-8 | Merges `origin/development` into branch `agent/code-review` (worktree `../KONTUR-review` — the branch is the "last reviewed" watermark, pushed to origin after each run), reviews the new range, files GitHub issues labeled `agent-review` (one per blocking finding + one suggestions digest). Failed runs roll the watermark back so nothing is skipped. |

Operations:
- Logs/transcripts: `Logs/gdd-sync/` and `Logs/code-review/` (gitignored).
- Test plumbing without spending tokens: `pwsh <script> -DryRun`.
- Run a real cycle now: `Start-ScheduledTask 'KONTUR Code Review'` (or run the script directly).
- Change model/times: `$model` at the top of each script; triggers in `register-tasks.ps1` (re-run it).
- Uninstall: `Unregister-ScheduledTask 'KONTUR GDD Sync','KONTUR Code Review'`.
