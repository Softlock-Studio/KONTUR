# Scheduled code reviewer. Maintains a worktree on branch agent/code-review as a
# "last reviewed" watermark: merges origin/development into it, has Claude review
# the newly merged range, files GitHub issues for findings. Rolls the merge back
# if the review run fails so nothing is silently skipped.
# Usage: code-review.ps1 [-DryRun]   (DryRun = plumbing only, no model call)

param([switch]$DryRun)
$ErrorActionPreference = 'Stop'

$repo   = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$wt     = Join-Path (Split-Path $repo -Parent) 'KONTUR-review'
$branch = 'agent/code-review'
$model  = 'claude-opus-4-8'

$logDir = Join-Path $repo 'Logs\code-review'
New-Item -ItemType Directory -Force $logDir | Out-Null
$transcript = Join-Path $logDir "run-$(Get-Date -Format 'yyyyMMdd-HHmm').log"
function Log($msg) { "$(Get-Date -Format 'HH:mm:ss') $msg" | Tee-Object -FilePath $transcript -Append }

Set-Location $repo
git fetch origin --quiet

if (-not (Test-Path $wt)) {
    if (git branch --list $branch) {
        git worktree add $wt $branch | Out-Null
    } else {
        git worktree add $wt -b $branch origin/development | Out-Null
        git -C $wt push -u origin $branch *>> $transcript
        Log "initialized $branch at origin/development - nothing to review yet"
        exit 0
    }
}

Set-Location $wt
$base = git rev-parse HEAD
$head = git rev-parse origin/development
$count = [int](git rev-list --count "$base..$head")

if ($count -eq 0) { Log 'no new commits on origin/development'; exit 0 }
Log "reviewing $count new commit(s): $base..$head"

if ($DryRun) { Log 'DRY RUN - stopping before merge and model call'; exit 0 }

git merge --no-edit origin/development *>> $transcript
if ($LASTEXITCODE -ne 0) { git merge --abort; Log 'merge failed - aborted'; exit 1 }

$template = Get-Content (Join-Path $PSScriptRoot 'code-review-prompt.md') -Raw
$prompt = $template.Replace('{{RANGE}}', "$base..$head")

$prompt | & claude -p --model $model `
    --allowedTools 'Read,Grep,Glob,Bash(git:*),Bash(gh:*),Bash(date:*)' 2>&1 |
    Tee-Object -FilePath $transcript -Append

if ($LASTEXITCODE -ne 0) {
    git reset --hard $base *>> $transcript
    Log "claude exited $LASTEXITCODE - watermark rolled back, range will be re-reviewed next run"
    exit 1
}

# Watermark branch only ever fast-forwards along development - plain push.
git push -u origin $branch *>> $transcript
if ($LASTEXITCODE -ne 0) { Log 'push failed - watermark advanced locally only' }
Log 'review complete'
