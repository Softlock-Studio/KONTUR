# Sanity check for Unity .meta files under Assets/.
# Reports: assets (files and folders) missing a .meta, and orphan .meta files.
# Respects .gitignore via git ls-files. Exit 0 = clean, 1 = issues found.

$ErrorActionPreference = 'Stop'

$files = @(git ls-files --cached --others --exclude-standard -- Assets) |
    Where-Object { $_ } | ForEach-Object { $_ -replace '\\', '/' }

if (-not $files) { Write-Host 'OK (no files under Assets/)'; exit 0 }

$set = [System.Collections.Generic.HashSet[string]]::new(
    [string[]]$files, [System.StringComparer]::OrdinalIgnoreCase)

$assets = $files | Where-Object { $_ -notlike '*.meta' }
$metas  = $files | Where-Object { $_ -like '*.meta' }

# Every directory between Assets/ and a tracked file needs a .meta too.
$dirs = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
foreach ($f in $assets + $metas) {
    $d = [System.IO.Path]::GetDirectoryName($f) -replace '\\', '/'
    while ($d -and $d -ne 'Assets') {
        [void]$dirs.Add($d)
        $d = [System.IO.Path]::GetDirectoryName($d) -replace '\\', '/'
    }
}

$missing = @()
foreach ($a in $assets) { if (-not $set.Contains("$a.meta")) { $missing += $a } }
foreach ($d in $dirs)   { if (-not $set.Contains("$d.meta")) { $missing += "$d/" } }

$orphans = @()
foreach ($m in $metas) {
    $base = $m.Substring(0, $m.Length - 5)
    if (-not $set.Contains($base) -and -not $dirs.Contains($base) -and -not (Test-Path $base)) {
        $orphans += $m
    }
}

if ($missing) {
    Write-Host "MISSING .meta ($($missing.Count)) - open the project in Unity once to generate, then commit them:"
    $missing | Sort-Object | ForEach-Object { Write-Host "  $_" }
}
if ($orphans) {
    Write-Host "ORPHAN .meta ($($orphans.Count)) - asset is gone; delete the .meta too:"
    $orphans | Sort-Object | ForEach-Object { Write-Host "  $_" }
}

if ($missing -or $orphans) { exit 1 }
Write-Host 'OK'
exit 0
