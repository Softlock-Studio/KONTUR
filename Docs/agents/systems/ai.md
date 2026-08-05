# System brief: AI (Babooshka + Employees)

Namespace `Game.AI.Babooshka`, `Game.AI.Employee` · folder `Assets/_Project/Scripts/AI/`.

## Babooshka (the entity)
`BabooshkaController` (MonoBehaviour, requires NavMeshAgent) owns everything:
- Builds a UnityHFSM `StateMachine` in `Awake` with states **Wander → Chase →
  Fight/Search → Wander**. Transitions read `BabooshkaBlackboard`:
  - Wander→Chase: `Target != null` · Wander→Search: heard sound within
    `HearingReactionWindow` · Chase→Fight: target within `AttackRange` ·
    Chase→Search: target lost · Search→Chase: target reacquired ·
    Search→Wander: `TransitionAfter(InvestigateTimeout)` · Fight→Wander: `IsResolved`.
- `SightSensor` / `HearingSensor` are `Bind(blackboard, config)`-ed components that
  write Target / LastHeardSound into the blackboard. Sight is ticked from
  `Update`; hearing is event-driven.
- All tuning lives in `BabooshkaConfig` (ScriptableObject); instance at
  `Assets/_Project/Configs/AI/BabooshkaConfig.asset`. Add new tunables there,
  never as magic numbers in states.
- `FightState` pulls infection via `Game.House.IInfectionDirector`, injected as a
  serialized `MonoBehaviour` field on `BabooshkaController` (`infectionDirectorSource`,
  cast to the interface in `Awake`). In `TestHouse` this is wired to `ZoneRegistry`,
  which implements `IInfectionDirector.GetInfectionLevel()` as the average infection
  across all `Zone`s. `StubInfectionDirector` still exists for scenes without a house
  (e.g. `TestAI`) — swap the serialized reference, don't change code. (The interface
  lives in `Game.House`, not here — House is the source of truth for infection, AI is
  just a consumer; it was originally written under `Game.AI.Babooshka` before `Game.House`
  existed and got moved once that stopped making sense.)
- Fight outcome: `BabooshkaConfig.ResolveDeathChance(infection)` rolls whether the
  employee dies; survivors get `ApplyAttackOutcome(survived: true)` (sets
  `FleeRequested` on the employee blackboard) and a mercy window
  (`SparedTarget`/`SparedUntilTime`) so Babooshka won't immediately re-target them.
- `WanderState` (the `Wander` state) roams `patrolPoints` at random (random point,
  random stand-still duration per `WanderStandStillMinSeconds/MaxSeconds`), and with
  `ApartmentVisitChance` probability instead detours into a random `RoomType.Apartment`
  zone via `Game.House.IZoneDirectory` (same `infectionDirectorSource`, optionally cast —
  `null` in scenes without a zone directory, e.g. `TestAI`, where it just falls back to
  `patrolPoints`). After standing still in an apartment, rolls `WallLickChance` once; on
  success calls `IWanderZone.TriggerInfectionOutbreak()` on that zone (a no-op if an
  outbreak there is already active).
- Aggression is configurable: `BabooshkaConfig.AggressionChance01` (0 = never engages, 1 =
  always, same as before this field existed) gates whether a freshly-sighted employee
  actually becomes `blackboard.Target` — see `SightSensor.ApplyAggressionGate`. The roll
  happens once per new sighting and sticks (via `blackboard.IgnoredSightTarget`) for as
  long as that employee stays continuously visible, so a "no" doesn't flicker into a "yes"
  a few frames later; losing sight of everyone resets it for the next encounter.

## Employees
Real FSM now, not stubs: `EmployeeController` (`MonoBehaviour`, requires
`NavMeshAgent`) builds a UnityHFSM machine in `Awake` with states **Idle →
MovingTo → PerformingTask → Idle**, plus **ReturningToBase → Idle** and an
any-state **→ Fleeing → Idle** override driven by `EmployeeBlackboard.FleeRequested`.
- `IEmployee` is the cross-system contract: `Position`, `IsAlive`,
  `CurrentStateName`, `AssignTask(IEmployeeTask)`, `Move`, `Stop`, `ReturnToBase`,
  `ApplyAttackOutcome(bool survived)`. House code depends only on this interface.
- `IEmployeeTask` (`TargetPosition`, `Duration`, `OnStarted/OnCompleted/OnCancelled`)
  is how callers hand the employee work without it knowing what the task *is*.
  `Game.House.ZoneTask` is the real implementation (applies a zone's
  `IZoneActivityEffect` on completion); `DebugEmployeeTask` is a no-op version used
  by `EmployeeDebugController` for ad-hoc move/assign testing via mouse + hotkeys
  (LMB select/move, RMB assign, X/B/F/G = Stop/ReturnToBase/simulate-survive/die).
- `AssignTask`/`Move`/`Stop`/`ReturnToBase` all no-op while `!IsAlive` or mid-flee
  (`CanAcceptCommand`) — check that gate before adding new commands.
- Tuning lives in `EmployeeConfig` (ScriptableObject), same pattern as Babooshka.
- Task queue / multi-room scheduling (assigning across many zones automatically) is
  NOT built — assignment today is one-off, per-zone (`Zone.TryAssign`), driven by a
  human click in the debug controller.
- Roster size is a pool, not dynamic spawning: `EmployeeRegistry.Awake()` finds every
  `EmployeeController` placed in the scene, then immediately enables the first `count`
  of them and disables the rest — `count` = last level's `SaveData.AliveEmployeeCount`
  (0 if no save exists yet, i.e. this is the very first level) +
  `HouseConfig.EmployeeReinforcements`, clamped to how many are actually placed. Which
  specific placed instances stay active is arbitrary (they're interchangeable besides
  `CallsignNumber`). This all happens in `Awake` — deliberately not deferred to a
  VContainer `IStartable` — because `EmployeeListView` (a plain `MonoBehaviour`) reads
  `Employees` in its own native Unity `Start()`, which runs *before* any VContainer
  `IStartable.Start()` in the same frame (confirmed via
  `PlayerLoopHelper`: `VContainerStartup` is inserted right after
  `EarlyUpdate.ScriptRunDelayedStartupFrame`, where native `Start()` calls happen) — an
  `IStartable`-based activator would sometimes lose that race and leave the Employee
  List UI bound to an empty roster. Awake has no such race (Unity finishes every
  object's Awake before calling Start on any of them), so `HouseConfig` is wired to
  `EmployeeRegistry` via a plain `[SerializeField]` (same asset as `MissionScope`'s)
  rather than DI-injected — resolving from `MissionScope`'s own container isn't safe
  that early either, only `GameLifetimeScope`'s (already built since MainMenu) is. To
  change the max roster for a level, place more/fewer `Employee.prefab` instances in
  the scene by hand — see `Docs/agents/systems/save.md`.

## Cross-references
- `Game.House.IInfectionDirector` and `Game.House.IZoneDirectory`/`IWanderZone` are
  House-owned contracts AI consumes — see `Docs/agents/systems/house.md`. Don't add
  new AI-owned contracts for House concepts; if it's about zones/infection, it belongs
  in `Game.House`, even though it's Babooshka calling it.

## Rules of thumb
- New states: one class per file in `States/`, constructor-injected deps
  (agent, config, blackboard), registered + wired in
  `BabooshkaController.BuildStateMachine` — keep transition logic there, not
  inside states.
- Anything the UI/monitor will need from the entity goes through `IBabooshka`.
- Test scene: `Assets/_Project/Scenes/TestScenes/TestAI/`.
