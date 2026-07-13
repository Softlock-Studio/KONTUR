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
- `FightState` pulls infection via `IInfectionDirector` (currently
  `StubInfectionDirector`, injected as a `MonoBehaviour` serialized field).

## Employees
Stubs only: `IEmployee { Vector3 Position; bool IsAlive; }` + `EmployeeStub`.
The real worker system (tasks, room assignment) is not built yet — design it
against `IEmployee`, extend the interface rather than casting to concretes.

## Rules of thumb
- New states: one class per file in `States/`, constructor-injected deps
  (agent, config, blackboard), registered + wired in
  `BabooshkaController.BuildStateMachine` — keep transition logic there, not
  inside states.
- Anything the UI/monitor will need from the entity goes through `IBabooshka`.
- Test scene: `Assets/_Project/Scenes/TestScenes/TestAI/`.
