---
name: compile-check
description: Fast C# compile check via the IDE-generated csproj. Use after editing .cs files, before claiming any code change is done. Seconds, not minutes; does not need Unity running.
---

# Fast compile check

Unity generates `Assembly-CSharp.csproj` (plus one csproj per asmdef) at the repo
root when the project is opened in an IDE. They are gitignored and machine-local.

1. Check the repo root for `Assembly-CSharp.csproj`.
   - Missing → tell the user to generate projects (open the project in
     Rider/VS, or Unity → Edit → Preferences → External Tools → Regenerate
     project files; see `Docs/agents/setup.md`). Never write a csproj by hand.
2. Build: `dotnet build Assembly-CSharp.csproj --nologo -v q`
   (plus `Assembly-CSharp-Editor.csproj` / asmdef csprojs if you touched editor
   code or asmdef-covered code).
3. If dotnet fails on project FORMAT (MSBuild namespace/targets errors, not
   `error CS`), fall back to VS MSBuild: `msbuild Assembly-CSharp.csproj -nologo -v:q`.
4. Fix every `error CS`; report warnings you didn't introduce without fixing them.

## Known blind spots — say so in your handoff when they apply
- The csproj is a snapshot: **files added/deleted since generation are not in
  it.** If you created new .cs files, a passing build does NOT cover them —
  either ask the user to regenerate projects and re-run, or run the `unity-tests`
  skill (batch mode recompiles everything).
- Catches C# errors only — not asset references, serialization, or missing
  components.
