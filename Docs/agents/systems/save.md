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
- `TryLoad` has two consumers now: `MainMenuUI` (Continue button + which level it loads)
  and `AI.Employee.EmployeeRegistry.Awake()` (`AliveEmployeeCount` feeds next level's
  starting roster size — see `Docs/agents/systems/ai.md`). The latter resolves
  `ISaveService` via `LifetimeScope.Find<GameLifetimeScope>()` rather than constructor
  injection, since it has to run in `Awake`, before `MissionScope`'s own container is
  guaranteed built. It's still just a single-slot read; no multi-slot/continue-screen UI
  beyond the Continue button exists.
- If you need to save more state, extend `SaveData` and gather it in
  `LevelStartSaveTrigger.Start()` (or a new trigger, if the data doesn't belong there) —
  don't reach into `SaveService` from elsewhere in the codebase.
- Test scene: any scene using `MissionScope` (e.g. `TestHouse.unity`) triggers a save on
  start; check `Application.persistentDataPath/save.json` after entering play mode.
