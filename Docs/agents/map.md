# Codebase map

Read this instead of exploring the tree. **Update this file whenever you add, move,
or remove a system** — one line per entry, keep it scannable.

```
Assets/_Project/
  Scripts/
    AI/
      Babooshka/        the anomalous entity. UnityHFSM FSM: Patrol/Chase/Search/Fight
        Interfaces/     IBabooshka (Position, CurrentStateName)
        Sensors/        SightSensor, HearingSensor -> write into BabooshkaBlackboard
        States/         one class per FSM state
        BabooshkaController.cs   MonoBehaviour owner: builds FSM, ticks sensors
        BabooshkaBlackboard.cs   shared mutable state (Target, last seen/heard)
        BabooshkaConfig.cs       ScriptableObject tuning
        IInfectionDirector.cs    infection level source (StubInfectionDirector for now)
      Employee/         worker side, stubs only: IEmployee (Position, IsAlive), EmployeeStub
  Art/                  Models (+ TEMP_Materials), Sprites
  Configs/              SO instances, e.g. AI/BabooshkaConfig.asset
  Scenes/               MainMenu.unity, MainScene.unity, TestScenes/TestAI/
  Settings/             URP renderer + quality assets
Assets/Plugins/         TextMesh Pro only — never add code here
Docs/agents/            this map, system briefs, GDD extracts, templates
Tools/                  check-metas.ps1 (pre-commit .meta sanity check)
```

Planned systems (per GDD, not yet created): `Core/` (night loop, timer,
VContainer scopes), `Infection/` (zone model, spread, treatments),
`Tasks/` (assignment, task execution), `UI/` (map screen, employee list,
context menus, indicators), `Cameras/` (feed switching, fog of war),
`Resources/` (warehouse, generator fuel), `Residents/` (random events).

## Cross-system contracts
| Interface | Path | Consumers |
|---|---|---|
| `IEmployee` | Scripts/AI/Employee/IEmployee.cs | Babooshka sensors/blackboard |
| `IInfectionDirector` | Scripts/AI/Babooshka/IInfectionDirector.cs | FightState |
| `IBabooshka` | Scripts/AI/Babooshka/Interfaces/IBabooshka.cs | (future UI/monitor feed) |

No asmdefs yet — everything compiles into Assembly-CSharp. No VContainer
LifetimeScope exists yet; first system to need a service should create
`Scripts/Core/` and flag it in the PR.
