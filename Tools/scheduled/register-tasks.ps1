# Registers the two scheduled agents in Windows Task Scheduler (per-user, no
# admin needed). Re-run any time - it overwrites existing registrations.
# Prereqs on this machine: claude CLI logged in, gh CLI logged in (reviewer).

$ErrorActionPreference = 'Stop'
$pwshPath = (Get-Command pwsh).Source
$settings = New-ScheduledTaskSettingsSet -StartWhenAvailable

$sync = New-ScheduledTaskAction -Execute $pwshPath `
    -Argument "-NoProfile -ExecutionPolicy Bypass -File `"$PSScriptRoot\gdd-sync.ps1`""
Register-ScheduledTask -TaskName 'KONTUR GDD Sync' -Action $sync `
    -Trigger (New-ScheduledTaskTrigger -Daily -At 12:00) -Settings $settings `
    -Description 'Daily: sync Docs/agents/gdd extracts with the designer''s Google Doc (branch chore/gdd-sync)' `
    -Force | Out-Null

$review = New-ScheduledTaskAction -Execute $pwshPath `
    -Argument "-NoProfile -ExecutionPolicy Bypass -File `"$PSScriptRoot\code-review.ps1`""
Register-ScheduledTask -TaskName 'KONTUR Code Review' -Action $review `
    -Trigger @((New-ScheduledTaskTrigger -Daily -At 01:00), (New-ScheduledTaskTrigger -Daily -At 12:00)) `
    -Settings $settings `
    -Description 'Twice daily: review new commits on development, file GitHub issues (branch agent/code-review)' `
    -Force | Out-Null

"Registered: 'KONTUR GDD Sync' (daily 12:00) and 'KONTUR Code Review' (daily 01:00 + 12:00)."
