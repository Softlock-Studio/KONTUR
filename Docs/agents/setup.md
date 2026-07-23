# Per-machine setup (one-time, ~5 min)

Everything agents need that can't be committed. Do this once per dev machine.

## 1. .NET SDK
`dotnet --version` should print 8.x or newer. If not: https://dotnet.microsoft.com/download

## 2. Generate C# projects
Open the project in Unity once, then open it in your IDE (Rider / Visual Studio), or
Unity → Edit → Preferences → External Tools → **Regenerate project files**.
This creates `Assembly-CSharp.csproj` etc. at the repo root (gitignored,
machine-local). Agents use these for fast compile checks.
Regenerate after pulling changes that add/remove `.cs` files.

## 3. Unity editor path for batch-mode tests
Set `UNITY_EDITOR_PATH` in your shell/user environment so batch-mode test commands
can locate Unity. Example for Windows PowerShell:

```powershell
setx UNITY_EDITOR_PATH "C:\Program Files\Unity\Hub\Editor\6000.0.78f1\Editor\Unity.exe"
```

Start a new shell after `setx`, then confirm with:

```powershell
echo $env:UNITY_EDITOR_PATH
```

If you still use Claude Code locally, you may alternatively inject the same value
through `.claude/settings.local.json` (gitignored):

```json
{
  "env": {
    "UNITY_EDITOR_PATH": "C:\\Program Files\\Unity\\Hub\\Editor\\6000.0.78f1\\Editor\\Unity.exe"
  }
}
```

Adjust to your Unity Hub install path.

## 4. Batch-mode test commands
EditMode:

```powershell
& "$env:UNITY_EDITOR_PATH" -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Logs/editmode-results.xml -logFile Logs/unity-tests.log
```

PlayMode:

```powershell
& "$env:UNITY_EDITOR_PATH" -batchmode -projectPath . -runTests -testPlatform PlayMode -testResults Logs/playmode-results.xml -logFile Logs/unity-tests.log
```

## 5. Sanity check
- `dotnet build Assembly-CSharp.csproj --nologo` → succeeds
- `pwsh Tools/check-metas.ps1` → "OK"
