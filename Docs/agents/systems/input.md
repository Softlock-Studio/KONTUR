# System brief: Input (mouse/keyboard polling)

Namespace `Game.Input` · folder `Assets/_Project/Scripts/Input/`. Game-wide, registered in
`GameLifetimeScope` (persists across missions), same bucket as `Game.Audio`/`Game.Localization`.

## Why this exists
Before this system, gameplay/debug code read `Mouse.current`/`Keyboard.current` directly (new
Input System, device-polling style — the project never used the legacy `UnityEngine.Input` class
or `.inputactions` action maps). That meant every script duplicated null-checks on the device and
had no single place to change how input is read. `IInputService` centralizes it.

## Layers
- **`IInputService`** — the only contract other systems should depend on.
  - `MousePosition` / `MouseDelta` — current-frame mouse position/delta (screen space).
  - `IsLeftMouseButtonHeld` / `IsRightMouseButtonHeld` — held state.
  - `WasLeftMouseButtonPressedThisFrame` / `...ReleasedThisFrame` and the `Right` equivalents.
  - `WasEscapePressedThisFrame`.
  - `IsKeyHeld(Key)` / `WasKeyPressedThisFrame(Key)` / `WasKeyReleasedThisFrame(Key)` — generic
    keyboard access (Input System's `Key` enum, not `KeyCode`) for call sites that need a specific
    or rebindable key (debug hotkeys, WASD movement) beyond the LMB/RMB/Esc members above.
- **`InputService`** (`IInputService, ITickable`, `RegisterEntryPoint`) — the only implementation.
  Polls `Mouse.current`/`Keyboard.current` once per frame in `Tick()` and caches the results, so
  every property is already this-frame-accurate by the time consumers read it in their own
  `Update()`. No `IStartable`/`IDisposable` — nothing to set up or tear down.

## Consuming it
Plain scene `MonoBehaviour`s resolve it the same way they resolve `ILocalizationService`/
`IAudioService` — `LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IInputService>()` in
`Start()` (not `Awake()`, scope build order isn't guaranteed earlier), cached to a field. See
`EmployeeDebugController`, `EmployeeStub`, `SpectatorFlyCamera`, `LocalizationDebugController` for
the pattern.

## Not covered here
`MapClickController` (`Scripts/UI/House/MapClickController.cs`) reads LMB/RMB through uGUI's
`IPointerClickHandler`/`PointerEventData`, not device polling — that's Unity's EventSystem/
`InputSystemUIInputModule` layer, which already correctly filters clicks against UI raycasting.
It's intentionally left alone; routing it through `IInputService` would lose that filtering.

## Rules of thumb
- New input read = `IInputService`, never a fresh `Mouse.current`/`Keyboard.current` call.
- `WasXPressedThisFrame` for one-shot actions (clicks, hotkeys), `IsXHeld` for continuous state
  (fly-cam look/move, sprint modifiers) — same distinction Unity's own `InputControl` API makes.
- `WasEscapePressedThisFrame` is exposed but not yet wired to any UI (no panel currently closes on
  Esc) — wire it explicitly in the consuming view if/when that's needed, don't assume it fires.
