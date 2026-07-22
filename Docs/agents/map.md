# Systems Map

Status reflects `Assets/_Project/Scripts/` as of 2026-07-15 (commit `54daaf8`). "Built"
means real gameplay code exists (may still be debug-driven, no Canvas UI yet). "Planned"
means design-only, nothing under `Scripts/` yet.

| System | Status | Code | GDD Extract |
| ------ | ------ | ---- | ----------- |
| Babooshka AI (Patrol/Chase/Search/Fight FSM, sight+hearing sensors) | Built | `Scripts/AI/Babooshka/` | [babooshka.md](gdd/babooshka.md) |
| Employee AI (Idle/MovingTo/PerformingTask/ReturningToBase/Fleeing FSM) | Built | `Scripts/AI/Employee/` | [employees.md](gdd/employees.md) |
| Employee task queue / room assignment | Built (debug-driven click UI, no HUD) | `Scripts/AI/Employee/EmployeeDebugController.cs`, `Scripts/House/ZoneTask.cs` | [employees.md](gdd/employees.md) |
| House model + zone infection/light (0-100% per zone, house average) | Built | `Scripts/House/` (`HouseModel`, `Zone`, `ZoneRegistry`) | [infection.md](gdd/infection.md) |
| Infection spread (darkness modifier; treatment activity) | Built (placeholder tuning, not GDD-sourced) | `Scripts/House/Zone.cs`, `ZoneConfig.cs`, `ReduceInfectionEffect.cs` | [infection.md](gdd/infection.md) |
| Zone event system (LightOff/InfectionOutbreak/Emergency; instant vs condition, expiry → failed task) | Built | `Scripts/House/ZoneEventDefinition.cs`, `ZoneEventKind.cs`, `ZoneEventType.cs`, `Zone.cs` | [residents.md](gdd/residents.md) |
| House presenter / view boundary (Presenter+`IHouseView`, debug console view) | Built (Canvas view not implemented — `DebugHouseConsoleView` is the only `IHouseView`) | `Scripts/House/Presentation/` | [ui.md](gdd/ui.md) |
| DI wiring (VContainer game + mission scopes) | Built | `Scripts/Bootstrap/GameLifetimeScope.cs`, `Scripts/Mission/MissionScope.cs` | — |
| Audio (music/SFX/master volume, mixer-driven, `AudioEmitter` hooks on Employee/Babooshka) | Built (no clip/mixer assets authored yet — framework only) | `Scripts/Audio/` | — |
| UI layout (Canvas scene) | Scene only, no C# yet | `Assets/_Project/Scenes/TestScenes/TestUI.unity` | [ui.md](gdd/ui.md) |
| Full house scan (button, generator cost, staleness) | Planned | — | [infection.md](gdd/infection.md) |
| Fog of war (per-zone staleness timer) | Planned | — | [floor-map.md](gdd/floor-map.md) |
| Generator (shared fuel pool; powers ACS/cameras/lighting/scan) | Planned | — | [resources.md](gdd/resources.md) |
| Resources (vinegar, iodine-resorcinol-A, lightbulbs, fuel) | Planned | — | [resources.md](gdd/resources.md) |
| Camera system (switch, visual tracking, ACS stabilization) | Planned | — | [cameras.md](gdd/cameras.md) |
| Floor map / mini-map (GPS employees; fog-of-war overlay) | Planned | — | [floor-map.md](gdd/floor-map.md) |
| Grandmother feed schedule / chains | Planned (roam+chase FSM exists, see Babooshka AI above) | — | [grandmother.md](gdd/grandmother.md) |
| Night timer (~7 real min per night) | Planned | — | [nights.md](gdd/nights.md) |
| Night cycle (multi-night infection corridor [floor; ceiling]) | Planned | — | [game-loop.md](gdd/game-loop.md) |
| Resident events (random events, help requests, emergencies) | Partially built — event *scaffolding* (spawn/expiry/resolve) is generic and live; specific resident-event content is not authored | `Scripts/House/ZoneEventType.cs`, `ResolveZoneEventEffect.cs` | [residents.md](gdd/residents.md) |
| Defeat conditions (full team death; critical infection; infection below floor ×2) | Planned | — | [defeat.md](gdd/defeat.md) |
| Object stabilization (camera-as-stabilizer rule) | Planned | — | [lore-stabilization.md](gdd/lore-stabilization.md) |

## Test scenes
- `Assets/_Project/Scenes/TestScenes/TestAI/` — Babooshka + Employee FSMs.
- `Assets/_Project/Scenes/TestScenes/TestHouse.unity` — House model/zones/events, DI scopes, debug console + OnGUI views.
- `Assets/_Project/Scenes/TestScenes/TestUI.unity` — Canvas layout only, not wired to any system yet.

## Per-system briefs
`Docs/agents/systems/ai.md` (Babooshka + Employee), `Docs/agents/systems/house.md`
(Zone/HouseModel/events), `Docs/agents/systems/audio.md` (music/SFX/volume). Read the
relevant one before editing that system.
