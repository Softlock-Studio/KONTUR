# System brief: UI (Display Canvas)

Namespace `Game.UI.House` / `Game.UI.Settings` · folder `Scripts/UI/House/`, `Scripts/UI/Settings/`.
Wires the already-laid-out `Assets/_Project/Prefabs/UI/Display Canvas.prefab` (instanced in
`TestHouse.unity`/`Level1.unity`, under `MissionScope`) to the House/Employee/Camera/Mission/Audio
backends. See `Docs/agents/systems/house.md`, `ai.md`, `audio.md` for those systems themselves.

## DI access pattern
New HUD scripts do **not** use `[Inject]`/Auto Inject Game Objects — they use the `MainMenuUI`
precedent instead: `LifetimeScope.Find<MissionScope>(gameObject.scene).Container.Resolve<T>()`
(mission-scoped: `EmployeeRegistry`, `IHousePresenter`, `MissionManager`) or
`LifetimeScope.Find<GameLifetimeScope>()` (game-wide: `IAudioService`). This sidesteps the "forgot
to add to Auto Inject Game Objects" foot-gun that bit `AudioEmitter`/`BackgroundMusicTrigger`
earlier — see `audio.md`.

Always pass `gameObject.scene` to `Find<MissionScope>` (never the parameterless overload):
`SceneController.LevelLoad` (`Scripts/Loader/SceneLoader/SceneController.cs`) loads the next level
additively before unloading the previous one — Unity refuses to unload the only loaded scene, so
load-then-unload is required — meaning two `MissionScope`s briefly coexist. The parameterless
`Find<MissionScope>()` is `FindAnyObjectByType` under the hood and can silently grab the outgoing
level's (soon destroyed) scope instead of the incoming one. `GameLifetimeScope` doesn't need this —
it's a single persistent root, never duplicated. This bit `EmployeeListView`/`HouseCanvasView`/
`MissionTimerView`/`ResourceGridPresenter`/`ZoneMapLabelsPresenter` in production (Missing
ReferenceExceptions and stale employee/infection state right after a level transition) before all
five were switched to the scene-aware overload.

## Employee list & task assignment
- **`EmployeeRegistry`** (`Scripts/AI/Employee/`) — mirrors `ZoneRegistry`:
  `FindObjectsByType<EmployeeController>` in `Awake`, exposes `IReadOnlyList<IEmployee>`.
  `RegisterComponentInHierarchy` in `MissionScope`.
- **`EmployeeSlotView`** — one of the 5 fixed `Employee Slot` pool instances (no overflow handling
  past 5). Toggles the "Unavailable" placeholder vs the nested `Employee Card`; `IPointerClickHandler`
  raises `Clicked`. "Goal" is approximated from `IEmployee.CurrentStateName` (no real
  goal/destination data exists on `IEmployee` yet); "Destination" has no backing data at all.
- **`EmployeeListPresenter`** — binds the 5 slots, tracks the selected `IEmployee`
  (`SelectionChanged` event), exposes `HousePresenter` for sibling views to call.
- **`EmployeeActionButtonsView`** — Move/Stop/Return buttons. Move does **not** take a destination
  itself — it arms `MapClickController` (`ArmPlainMove`) for one plain move on the next map click.
  Stop/Return call `IHousePresenter` immediately. All three disabled with no selection.
- **`MapClickController`** (on `Map`) — one raycast per click, converted from the RawImage click
  via `RawImageWorldRay` (shared UV→world-ray helper). A `GameCamera` hit switches the selected
  camera feed (`CamerasView.HandleClick`); a `Zone` hit either performs the armed plain move or
  opens **`ZoneActionMenuView`** — the real GDD flow ("click employee, click zone → context menu"),
  not a raw destination pick. The menu's options are derived live from `Zone` state: "Treatment" if
  `Infection > 0`, "Change lightbulb" if `!HasLight`, "Resolve Emergency" if
  `HasActiveEvent(ZoneEventType.Emergency)` (the only event type that routes through
  `ActivityType.ResidentEvent` — check `Zone.TryBuildActivity` if this ever changes), always
  "Move here" and "Cancel".
- **`FloorToggleView`** — temporary multi-floor "system": moves the orthographic `Map Camera`'s Y
  between two serialized presets instead of any real floor toggle (none exists) — see rationale
  in the plan/commit history: disabling floor GameObjects would freeze that floor's `Zone.Update()`
  simulation, which is very likely not what's wanted.

## Everything else
- **`HouseCanvasView`** (`Scripts/House/Presentation/`) — the real `IHouseView`, replacing
  `DebugHouseConsoleView` in `MissionScope`. Infection → Infection Slider/Label directly;
  resources/orders delegate to `ResourceGridPresenter`/`OrdersToastView`. `RenderZones`/
  `UpdateZone`/`SetSelectedZone` are no-ops — there is still no per-zone display surface (room
  list), only the house-aggregate infection slider.
- **`ResourceGridPresenter`/`ResourceItemView`** — spawns one item per `ResourceType` under
  Inventory Grid on first sight, updates count thereafter (needs a "Resource Item" prefab — not
  created yet, see handoff).
- **`OrdersToastView`** — single latest-message toast on Order Label (not a scrolling log),
  auto-clears after a few seconds.
- **`MissionTimerView`** — Time/Night labels from `MissionManager.GetTimer`/`IsEndDay`; only
  touches the label when the displayed whole-second value changes.
- **`SettingsPanelView`** — Master/Music/Sfx sliders bound to `IAudioService`; Settings Button
  toggles the panel (panel itself doesn't exist yet, see handoff).

## Known gaps (intentionally not solved here)
- No per-zone display UI (room list) — only the house-aggregate infection slider.
- `Employee Card`'s "Destination" has no backing data; "Goal" is an approximation.
- Employee list pool is fixed at 5 — no overflow UI if the roster grows past that.
- Floor "system" is a camera-height hack, not real multi-floor support.
- Several new UI subtrees referenced by this code don't exist in `Display Canvas.prefab` yet —
  see the handoff for the exact list; the scene-edit skill's `AgentTools` menu only covers
  `Employee Slot.prefab` wiring so far (`Tools/AgentTools/Wire Employee Slot`).

## Rules of thumb
- New HUD script needing a backend service → `LifetimeScope.Find<...>().Container.Resolve<...>()`
  in `Start()`, not `[Inject]`, unless you're registering it as the one canonical
  `RegisterComponentInHierarchy` implementation of an interface (`IHouseView`, `ICamerasView`).
- Don't bypass `IHousePresenter`/`IAudioService` — UI should never touch `Zone`/`HouseModel`/
  mixer internals directly.
