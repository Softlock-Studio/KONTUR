# Systems Map

Status reflects `Assets/_Project/Scripts/` as of 2026-07-15 (commit `54daaf8`). "Built"
means real gameplay code exists (may still be debug-driven, no Canvas UI yet). "Planned"
means design-only, nothing under `Scripts/` yet.

| System | Status | Code | GDD Extract |
| ------ | ------ | ---- | ----------- |
| Babooshka AI (Patrol/Chase/Search/Fight FSM, sight+hearing sensors) | Built | `Scripts/AI/Babooshka/` | [babooshka.md](gdd/babooshka.md) |
| Employee AI (Idle/MovingTo/PerformingTask/ReturningToBase/Fleeing FSM) | Built | `Scripts/AI/Employee/` | [employees.md](gdd/employees.md) |
| Employee task queue / room assignment | Built — debug click UI (`EmployeeDebugController`) still exists alongside real Canvas UI (select from Employee List, click a zone on the minimap → context menu of valid actions); auto-scheduling across zones is not built | `Scripts/AI/Employee/EmployeeDebugController.cs`, `Scripts/AI/Employee/EmployeeRegistry.cs`, `Scripts/UI/House/`, `Scripts/House/ZoneTask.cs` | [employees.md](gdd/employees.md) |
| House model + zone infection/light (0-100% per zone, house average) | Built | `Scripts/House/` (`HouseModel`, `Zone`, `ZoneRegistry`) | [infection.md](gdd/infection.md) |
| Infection spread (darkness modifier; treatment activity) | Built (placeholder tuning, not GDD-sourced) | `Scripts/House/Zone.cs`, `ZoneConfig.cs`, `ReduceInfectionEffect.cs` | [infection.md](gdd/infection.md) |
| Zone event system (LightOff/InfectionOutbreak/Emergency; instant vs condition, expiry → failed task) | Built | `Scripts/House/ZoneEventDefinition.cs`, `ZoneEventKind.cs`, `ZoneEventType.cs`, `Zone.cs` | [residents.md](gdd/residents.md) |
| House presenter / view boundary (Presenter+`IHouseView`, real Canvas view) | Built — `HouseCanvasView` is the registered `IHouseView` (`DebugHouseConsoleView` still exists, unregistered); per-zone display (room list) still has no UI, only house-aggregate infection | `Scripts/House/Presentation/` | [ui.md](gdd/ui.md) |
| DI wiring (VContainer game + mission scopes) | Built | `Scripts/Bootstrap/GameLifetimeScope.cs`, `Scripts/Mission/MissionScope.cs` | — |
| Input (centralized mouse/keyboard polling via new Input System — LMB/RMB/Esc + generic key queries) | Built | `Scripts/Input/` | — |
| Audio (music/SFX/master volume, mixer-driven, `AudioEmitter` hooks on Employee/Babooshka) | Built (no clip/mixer assets authored yet — framework only) | `Scripts/Audio/` | — |
| UI layout (Display Canvas: employee list, camera feed, infection, inventory, orders, timer, settings) | Built — wired to House/Mission/Audio/Camera backends; several new UI subtrees (Settings Panel sliders, Floor toggle buttons, Zone action menu, camera Map Icon, Resource Item) still need to be added in-editor, see `Docs/agents/systems/ui.md` | `Scripts/UI/House/`, `Scripts/UI/Settings/` | [ui.md](gdd/ui.md) |
| Full house scan (button, generator cost, staleness) | Planned | — | [infection.md](gdd/infection.md) |
| Fog of war (per-zone staleness timer) | Planned | — | [floor-map.md](gdd/floor-map.md) |
| Generator (shared fuel pool; powers ACS/cameras/lighting/scan) | Planned | — | [resources.md](gdd/resources.md) |
| Resources (vinegar, iodine-resorcinol-A, lightbulbs, fuel) | Planned | — | [resources.md](gdd/resources.md) |
| Camera system (switch by clicking a camera's Map Icon; `CamerasModel`/`CamerasPresenter`/`ICamerasView`; ACS/visual tracking not built) | Built (partial) | `Scripts/CameraSystem/` | [cameras.md](gdd/cameras.md) |
| Floor map / mini-map (top-down `Map Camera` feed, click-to-select-camera, click-a-zone-for-task-context-menu; GPS/fog-of-war overlay, floor system) | Built (partial) — temporary floor "system" just repositions the Map Camera between two Y presets, no real multi-floor support | `Scripts/UI/House/MapClickController.cs`, `FloorToggleView.cs`, `ZoneActionMenuView.cs` | [floor-map.md](gdd/floor-map.md) |
| Grandmother feed schedule / chains | Planned (roam+chase FSM exists, see Babooshka AI above) | — | [grandmother.md](gdd/grandmother.md) |
| Night timer (~7 real min per night) | Built (partial) — countdown + `LevelEnded` event (timeout=victory, defeat detected first=loss), carrying max infection reached and employees-killed count; no pause/between-levels screen yet | `Scripts/Mission/MissionManager.cs`, `MissionTimer.cs`, `LevelEndResult.cs` | [nights.md](gdd/nights.md) |
| Night cycle (multi-night infection corridor [floor; ceiling]) | Planned | — | [game-loop.md](gdd/game-loop.md) |
| Resident events (random events, help requests, emergencies) | Partially built — event *scaffolding* (spawn/expiry/resolve) is generic and live; specific resident-event content is not authored | `Scripts/House/ZoneEventType.cs`, `ResolveZoneEventEffect.cs` | [residents.md](gdd/residents.md) |
| Defeat conditions (full team death; critical infection; infection corridor) | Built — full team death and infection maxed (100%) end the mission instantly (`MissionManager.IsDefeated`); infection outside `HouseConfig.InfectionFloor01`/`InfectionCeiling01` at night-end is also a defeat (`MissionManager.IsInfectionWithinCorridor`), checked as a snapshot when the timer runs out rather than counting mid-night breaches (deviates from the GDD's "floor breached twice" draft — floor/ceiling are static per-level values, no night-to-night escalation) | `Scripts/Mission/MissionManager.cs`, `Scripts/House/HouseConfig.cs` | [defeat.md](gdd/defeat.md) |
| Object stabilization (camera-as-stabilizer rule) | Planned | — | [lore-stabilization.md](gdd/lore-stabilization.md) |
| Save (single-slot autosave on level start: resource counts, alive employee count) | Built (partial) — write-only checkpoint; load/continue flow not wired up | `Scripts/Save/` | — |

## Test scenes
- `Assets/_Project/Scenes/TestScenes/TestAI/` — Babooshka + Employee FSMs.
- `Assets/_Project/Scenes/TestScenes/TestHouse.unity` — House model/zones/events, DI scopes,
  `Display Canvas` UI (real `IHouseView`, camera/employee/timer/settings UI), debug console +
  OnGUI views still present alongside it.
- `Assets/_Project/Scenes/TestScenes/TestUI.unity` — earlier Canvas layout scratch scene, largely
  superseded by `Display Canvas` in `TestHouse.unity`.

## Per-system briefs
`Docs/agents/systems/ai.md` (Babooshka + Employee), `Docs/agents/systems/house.md`
(Zone/HouseModel/events), `Docs/agents/systems/audio.md` (music/SFX/volume),
`Docs/agents/systems/input.md` (mouse/keyboard polling), `Docs/agents/systems/ui.md` (Display
Canvas wiring: employee list, map/camera clicks, zone action menu, timer, settings),
`Docs/agents/systems/save.md` (autosave checkpoint on level start). Read the relevant one
before editing that system.
