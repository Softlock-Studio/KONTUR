# Scheduled GDD sync. Downloads the designer's Google Doc (anonymous markdown
# export), then has Claude reconcile Docs/agents/gdd/ extracts with it in a
# dedicated worktree on branch chore/gdd-sync. The branch is reset to
# origin/development every run and receives at most one fresh sync commit -
# review and merge it into development when it appears.
# Usage: gdd-sync.ps1 [-DryRun]   (DryRun = plumbing only, no model call)

param([switch]$DryRun)
$ErrorActionPreference = 'Stop'

$repo   = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$wt     = Join-Path (Split-Path $repo -Parent) 'KONTUR-gdd-sync'
$branch = 'chore/gdd-sync'
$model  = 'claude-sonnet-4-6'
$docId  = '1T0SfcPm2yhoArpBzUWWpfS4JBkCw-C-TdD8U6NgN4LE'

$logDir = Join-Path $repo 'Logs\gdd-sync'
New-Item -ItemType Directory -Force $logDir | Out-Null
$transcript = Join-Path $logDir "run-$(Get-Date -Format 'yyyyMMdd-HHmm').log"
function Log($msg) { "$(Get-Date -Format 'HH:mm:ss') $msg" | Tee-Object -FilePath $transcript -Append }

Set-Location $repo
git fetch origin --quiet

if (-not (Test-Path $wt)) {
    if (git branch --list $branch) { git worktree add $wt $branch | Out-Null }
    else { git worktree add $wt -b $branch origin/development | Out-Null }
}

Set-Location $wt
# Each run re-derives the full drift from the live doc, so unmerged commits from
# earlier runs are safe to discard: always start clean from development.
git reset --hard origin/development *>> $transcript

$md = (Invoke-WebRequest -Uri "https://docs.google.com/document/d/$docId/export?format=md" -UseBasicParsing).Content
# Strip embedded base64 images - they dwarf the text and models can't use them.
$md = $md -replace '\[image\d+\]:\s*<data:[^>]*>', ''
$md = $md -replace '\(data:image/[^)]*\)', '(image omitted)'
$docDir = Join-Path $wt 'Logs\gdd-sync'
New-Item -ItemType Directory -Force $docDir | Out-Null
Set-Content -Path (Join-Path $docDir 'gdd-latest.md') -Value $md -Encoding utf8
Log "downloaded GDD export ($($md.Length) chars after image strip)"

if ($DryRun) { Log 'DRY RUN - stopping before model call'; exit 0 }

Get-Content (Join-Path $PSScriptRoot 'gdd-sync-prompt.md') -Raw |
    & claude -p --model $model `
    --allowedTools 'Read,Grep,Glob,Write,Edit,Bash(git:*),Bash(date:*)' 2>&1 |
    Tee-Object -FilePath $transcript -Append

if ($LASTEXITCODE -ne 0) { Log "claude exited $LASTEXITCODE - see transcript"; exit 1 }

$new = [int](git rev-list --count "origin/development..$branch")
if ($new -eq 0) { Log 'no drift - no commit'; exit 0 }

# Land the sync commit on development. It sits directly on top of
# origin/development, so this push is a fast-forward unless development moved
# while the model ran - in that case rebase once and retry.
git fetch origin --quiet
git push origin "${branch}:development" *>> $transcript
if ($LASTEXITCODE -ne 0) {
    git fetch origin --quiet
    git rebase origin/development *>> $transcript
    if ($LASTEXITCODE -ne 0) {
        git rebase --abort
        git push --force-with-lease -u origin $branch *>> $transcript
        Log "could not land on development (rebase conflict) - pushed to origin/$branch for manual merge"
        exit 1
    }
    git push origin "${branch}:development" *>> $transcript
    if ($LASTEXITCODE -ne 0) {
        git push --force-with-lease -u origin $branch *>> $transcript
        Log "development push rejected - pushed to origin/$branch for manual merge"
        exit 1
    }
}
Log 'sync commit landed on development and pushed'
