# System brief: AI (Babooshka + Employees)

Namespace `Game.AI.Babooshka`, `Game.AI.Employee` · folder `Assets/_Project/Scripts/AI/`.

## Babooshka (the entity)
`BabooshkaController` (MonoBehaviour, requires NavMeshAgent) owns everything:
- Builds a UnityHFSM `StateMachine` in `Awake` with states **Patrol → Chase →
  Fight/Search → Patrol**. Transitions read `BabooshkaBlackboard`:
  - Patrol→Chase: `Target != null` · Patrol→Search: heard sound within
    `HearingReactionWindow` · Chase→Fight: target within `AttackRange` ·
    Chase→Search: target lost · Search→Chase: target reacquired ·
    Search→Patrol: `TransitionAfter(InvestigateTimeout)` · Fight→Patrol: `IsResolved`.
- `SightSensor` / `HearingSensor` are `Bind(blackboard, config)`-ed components that
  write Target / LastHeardSound into the blackboard. Sight is ticked from
  `Update`; hearing is event-driven.
- All tuning lives in `BabooshkaConfig` (ScriptableObject); instance at
  `Assets/_Project/Configs/AI/BabooshkaConfig.asset`. Add new tunables there,
  never as magic numbers in states.
- `FightState` pulls infection via `IInfectionDirector`, injected as a serialized
  `MonoBehaviour` field on `BabooshkaController` (`infectionDirectorSource`, cast to
  the interface in `Awake`). In `TestHouse` this is wired to `ZoneRegistry`, which
  implements `IInfectionDirector.GetInfectionLevel()` as the average infection across
  all `Zone`s. `StubInfectionDirector` still exists for scenes without a house (e.g.
  `TestAI`) — swap the serialized reference, don't change code.
- Fight outcome: `BabooshkaConfig.ResolveDeathChance(infection)` rolls whether the
  employee dies; survivors get `ApplyAttackOutcome(survived: true)` (sets
  `FleeRequested` on the employee blackboard) and a mercy window
  (`SparedTarget`/`SparedUntilTime`) so Babooshka won't immediately re-target them.

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

## Rules of thumb
- New states: one class per file in `States/`, constructor-injected deps
  (agent, config, blackboard), registered + wired in
  `BabooshkaController.BuildStateMachine` — keep transition logic there, not
  inside states.
- Anything the UI/monitor will need from the entity goes through `IBabooshka`.
- Test scene: `Assets/_Project/Scenes/TestScenes/TestAI/`.
