# System brief: Save

Namespace `Game.Save` · folder `Assets/_Project/Scripts/Save/`.

## What it does
Single-slot autosave checkpoint, written at the start of every level. Not a "continue
game" screen — that's still Planned (see `Docs/agents/map.md`); this is the persistence
groundwork for it.

- `ISaveService` — contract: `Save(SaveData)`, `TryLoad(out SaveData)`. Implemented by
  `SaveService` (`Lifetime.Singleton`, registered in `GameLifetimeScope`, alongside
  `ResourceInventory` — same "persists across missions" tier). Writes/reads one JSON file
  at `Application.persistentDataPath/save.json` via `JsonUtility`; each `Save` overwrites
  it, there is no multi-slot support.
- `SaveData` — plain `[Serializable]` DTO: `LevelType`, `AliveEmployeeCount`,
  `List<ResourceCountEntry>` (list, not `Dictionary`, because `JsonUtility` can't
  serialize dictionaries). Add new fields here as more state needs saving.
- `LevelStartSaveTrigger` — `IStartable`, `Lifetime.Scoped`, registered in `MissionScope`
  next to `MissionManager`. On `Start()` reads `ResourceInventory.GetAllCounts()`,
  counts `IEmployee.IsAlive` across `EmployeeRegistry.Employees`, and
  `SceneController.GetCurrentLevelType()`, then calls `ISaveService.Save`. Deliberately
  kept out of `MissionManager` — that class owns win/lose/timer, not persistence.

## Rules of thumb
- Nothing calls `TryLoad` yet — it's a stub for the future load/continue flow. Don't wire
  it into gameplay without a reason; adding a "continue" screen is a separate task.
- If you need to save more state, extend `SaveData` and gather it in
  `LevelStartSaveTrigger.Start()` (or a new trigger, if the data doesn't belong there) —
  don't reach into `SaveService` from elsewhere in the codebase.
- Test scene: any scene using `MissionScope` (e.g. `TestHouse.unity`) triggers a save on
  start; check `Application.persistentDataPath/save.json` after entering play mode.
