# Per-machine setup (one-time, ~5 min)

Everything agents need that can't be committed. Do this once per dev machine.

## 1. .NET SDK
`dotnet --version` should print 8.x or newer. If not: https://dotnet.microsoft.com/download

## 2. Generate C# projects
Open the project in Unity once, then open it in your IDE (Rider / Visual Studio), or
Unity → Edit → Preferences → External Tools → **Regenerate project files**.
This creates `Assembly-CSharp.csproj` etc. at the repo root (gitignored,
machine-local). The `compile-check` skill builds these.
Regenerate after pulling changes that add/remove `.cs` files.

## 3. Unity editor path for batch-mode tests
Create `.claude/settings.local.json` (gitignored) with:

```json
{
  "env": {
    "UNITY_EDITOR_PATH": "C:\\Program Files\\Unity\\Hub\\Editor\\6000.0.78f1\\Editor\\Unity.exe"
  }
}
```

Adjust to your Unity Hub install path. The `unity-tests` skill uses this.

## 4. Sanity check
- `dotnet build Assembly-CSharp.csproj --nologo` → succeeds
- `pwsh Tools/check-metas.ps1` → "OK"
