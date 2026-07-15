# System brief: House (zones, infection, events)

Namespace `Game.House[.Model|.Presentation]` · folder `Assets/_Project/Scripts/House/`.
Test scene: `Assets/_Project/Scenes/TestScenes/TestHouse.unity`.

## Layers
- **`Zone`** (`MonoBehaviour`) — the source of truth for one room: infection %
  (0-100, clamped), light on/off, occupant slots (`standingPoints`), active
  activities, active events. Tuned per-zone via a shared `ZoneConfig` asset
  (infection growth rates, activity durations/effects, `Events` list) — config
  values are placeholders, not GDD-sourced yet.
- **`ZoneRegistry`** (`MonoBehaviour`) — finds all `Zone`s in the scene via
  `FindObjectsByType` in `Awake`, exposes them as `IReadOnlyList<Zone>`, and
  implements `IInfectionDirector` (house-average infection, consumed by Babooshka —
  see `Docs/agents/systems/ai.md`). Also has an `OnGUI` debug readout.
- **`HouseModel`** (plain `IDisposable`, registered `Lifetime.Scoped` in
  `MissionScope`) — wraps `ZoneRegistry` behind immutable `ZoneSnapshot`s keyed by
  `ZoneId`, re-fires `ZoneChanged`/`TaskFailed` events by subscribing to each
  `Zone`'s events in `Initialize()`. This is the read model; nothing outside House
  should touch `Zone` directly except through here.
- **`HousePresenter`** (`IStartable, ITickable, IDisposable`, `RegisterEntryPoint`)
  — pushes `ZoneViewState`s to `IHouseView` on zone changes and on a
  0.5s poll (`InfectionSampleIntervalSeconds`) for continuously-growing infection.
  Exposes the only mutation entry points: `RequestAssignTask`,
  `RequestStopEmployee`, `RequestMoveEmployee`, `RequestReturnToBaseEmployee`,
  `SelectZone`/`ClearSelection`.
- **`IHouseView`** — the only implementation today is `DebugHouseConsoleView`
  (logs everything to console). **Temporary** — swap for the real Canvas view by
  registering it `.As<IHouseView>()` in `MissionScope` instead. No UI code exists
  under `Scripts/UI/` yet even though `TestUI.unity` has a Canvas layout.

## Activities vs. events — two different concepts
- **Activity** (`ActivityType`: Treatment, LightbulbChange, ResidentEvent) — work
  an employee is doing, created by `Zone.TryAssign` → `ZoneTask` → runs on the
  employee's FSM (see `Docs/agents/systems/ai.md`) → `IZoneActivityEffect.Apply(zone)`
  on completion (`ReduceInfectionEffect`, `RestoreLightEffect`,
  `ResolveZoneEventEffect`).
- **Event** (`ZoneEventType`: Emergency, LightOff, InfectionOutbreak) — something
  happening *in* the zone, spawned ambiently by `Zone.TickEvents` per
  `ZoneEventDefinition` (`config.Events`), independent of any employee. Two kinds:
  - `Instant` — counts concurrent occurrences (`MaxConcurrent`), each with its own
    optional expiry countdown; e.g. multiple `Emergency`s could stack.
  - `Condition` — a boolean tied to zone state (`LightOff` ⇔ `!HasLight`,
    `InfectionOutbreak` ⇔ an internal flag) rather than a counter; spawning it just
    flips the flag (`SetLight(false)` / `TriggerInfectionOutbreak()`).
  - If `ExpirySeconds > 0` and the event isn't resolved via `ResolveEvent`/an
    activity before the timer runs out, it counts as a **failed task**
    (`HouseModel.FailedTaskCount`, surfaced via `TaskFailed`/`view.ShowTaskFailed`).
  - `Zone.TriggerInfectionOutbreak()` is public and ungated by `config.Events` on
    purpose — it's the hook for a future grandmother behavior to force an outbreak
    in a chosen zone, independent of that zone's own ambient roster.

## Rules of thumb
- Don't add new mutation paths that bypass `HousePresenter` — UI and debug tools
  should call it, not `Zone`/`HouseModel` directly, so there's one place that knows
  about the view.
- New activity types: add to `ActivityType`, branch in `Zone.TryBuildActivity`,
  write an `IZoneActivityEffect`. New event types: add to `ZoneEventType`, extend
  `Zone.Spawn`/`IsConditionActive` if it needs custom activation logic (most
  `Instant` types need no extra code, just a `ZoneEventDefinition` entry in the
  `ZoneConfig` asset).
- `ZoneConfig` assets are shared across multiple `Zone` instances — never store
  runtime/per-zone-instance state on `ZoneConfig` or `ZoneEventDefinition`; it goes
  on `Zone` itself (see the comment on `eventCheckTimers` in `Zone.cs`).
- Employee task queue / auto-scheduling across zones is not built — see
  `Docs/agents/systems/ai.md`.
