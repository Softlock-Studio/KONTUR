# System brief: Audio (music, SFX, volume)

Namespace `Game.Audio` · folder `Assets/_Project/Scripts/Audio/`. Game-wide, registered in
`GameLifetimeScope` (persists across missions, same bucket as `Game.Localization`), not in
`MissionScope`.

## Layers
- **`IAudioService`** — the only contract other systems should depend on. Music
  (`PlayMusic`/`StopMusic`, crossfades over `AudioConfig.DefaultMusicFadeSeconds`), one-shot SFX
  (`PlayUiSfx` for 2D UI/menu sounds, `PlaySfxAtPoint` for pooled 3D one-shots), volume
  (`MasterVolume`/`MusicVolume`/`SfxVolume`, `SetXVolume`, `VolumeChanged` event for an options
  menu to listen to), and `CreateAttachedSource(Transform)` for components that need a sound
  that follows a moving object.
- **`AudioService`** (`IAudioService, IStartable, ITickable, IDisposable`, `RegisterEntryPoint`)
  — the only implementation. Owns a procedurally-created, `DontDestroyOnLoad` root `GameObject`
  with two music `AudioSource`s (A/B, for crossfade) and a pool of `AudioConfig.SfxPoolSize`
  SFX sources (round-robin, steals the oldest if all are busy — never allocates per call; each
  pooled source's `outputAudioMixerGroup` is (re)assigned per-call, since the same source gets
  reused for both `PlayUiSfx` and `PlaySfxAtPoint`).
  Volume is applied through a real Unity **AudioMixer** (`AudioConfig.Mixer`), not code-side
  scalars — `SetXVolume` converts linear 0-1 to dB (`Log10(v)*20`, floor -80dB) and calls
  `Mixer.SetFloat` on the exposed parameter named in `AudioConfig.MasterVolumeParam` /
  `MusicVolumeParam` / `SfxVolumeParam`. Volumes persist via `PlayerPrefs`
  (`Audio.MasterVolume/MusicVolume/SfxVolume`), same pattern as
  `LocalizationService`'s language pref.
- **`SfxCue`** / **`MusicCue`** (ScriptableObjects) — the only place clips are referenced.
  `SfxCue` supports a clip array (`GetClip()` picks one at random) and a pitch range
  (`GetPitch()`); `MusicCue` is a single looping clip. Never reference an `AudioClip` directly
  from gameplay code — always through a cue asset, same as `ZoneConfig`/`EmployeeConfig` hold
  tunables instead of magic numbers.
- **`AudioConfig`** — the system's tunable config (mixer + group refs, exposed-param names,
  default volumes/fade time, pool size). One instance, registered in `GameLifetimeScope`.

## No player avatar — sound is heard "through the camera", not "in the room"
Per the GDD (`Docs/agents/gdd/concept.md`), the player never has a body in the 3D world — they
sit in the observation room and see it only through whichever security camera is selected. Two
consequences for audio, one already handled, one not yet ours to fix:
- **Positioning**: Unity's 3D spatialization (`spatialBlend = 1`, used by `PlaySfxAtPoint` and
  `AudioEmitter`) is always relative to the scene's single `AudioListener`. Since there's no
  player object to put it on, the **Camera system** (currently Planned, no code yet — see
  `Docs/agents/systems/ai.md`/map) must move/reparent the one `AudioListener` onto whichever
  camera is currently selected when the player switches feeds. Nothing in `Game.Audio` needs to
  change for this to work — it's a property of wherever the listener sits, not of how sounds are
  played.
- **Coloring**: to sell "coming through a camera/TV speaker" rather than "standing in the room",
  in-world sound needs a filter (Lowpass/Distortion) that UI sound must NOT have — a button click
  is the player's own interface, not something transmitted through a camera mic. This is why
  `AudioConfig` splits `UiSfxGroup` (clean) from `WorldSfxGroup` (filtered) instead of one shared
  `SfxGroup` — see mixer layout below. `PlayUiSfx` always routes to `UiSfxGroup`;
  `PlaySfxAtPoint` and `CreateAttachedSource` (so all `AudioEmitter`s) always route to
  `WorldSfxGroup`. Music is intentionally not filtered — it's non-diegetic score, not something
  the camera "hears".
- **`AudioEmitter`** (`MonoBehaviour`) — attach to an entity prefab that needs a sound tracking
  its transform (footsteps, growls, ambient loops). Gets `IAudioService` via `[Inject]` (deferred
  to `Start()`, same reasoning as `LocalizedTextTMP.Construct`), asks the service for an attached
  `AudioSource` via `CreateAttachedSource`, then exposes `Play`/`PlayLoop`/`Stop`. For a one-off
  sound that doesn't need to follow anything, call `IAudioService.PlaySfxAtPoint` directly instead
  of adding an emitter.
- **`BackgroundMusicTrigger`** (`MonoBehaviour`) — drop on any scene GameObject, assign a
  `MusicCue`, and it calls `PlayMusic` on `Start()`. No registration in `MissionScope`/
  `GameLifetimeScope` — see injection note below for why.

## Injection for scene MonoBehaviours (`AudioEmitter`, `BackgroundMusicTrigger`)
These are plain scene/prefab components, not services other code resolves — so they are
**not** registered via `RegisterComponentInHierarchy` in a scope's `Configure()`.
`RegisterComponentInHierarchy<T>` searches the scene for exactly one `T` and **throws at
container-build time if none exists** — using it here would make the component mandatory and
crash the scope the moment someone removes the one instance from the scene (this was tried and
reverted for `BackgroundMusicTrigger`).

Instead, add the GameObject to the owning `LifetimeScope`'s **Auto Inject Game Objects** list in
the Inspector (`MissionScope` for `TestHouse`, or `GameLifetimeScope` for anything game-wide).
VContainer injects that object and every child recursively — so the prefab root is enough even
if the component sits on a child. If a GameObject with `AudioEmitter`/`BackgroundMusicTrigger` is
never added to that list, `[Inject]` never fires and `audioService` stays `null` — guard for that
in `Start()` rather than assuming it always runs (this bit both components initially).

## Current hooks (no-ops until cues are assigned — see handoff below)
- `EmployeeConfig.DeathCue` / `FleeCue`, played from `EmployeeController.ApplyAttackOutcome`.
- `BabooshkaConfig.WallLickCue`, played from `WanderState` right after a wall-lick triggers an
  infection outbreak.
- Both controllers take an optional `[SerializeField] AudioEmitter audioEmitter` — same tier as
  `EmployeeRagdoll`/`EmployeeAnimatorDriver`, not DI-resolved by the controller itself.

## Human setup required before any sound plays
1. Create `Assets/_Project/Configs/Audio/MasterMixer.mixer`: `Master` → child group `Music`,
   child group `Sfx` → its own children `UI` and `World`. Expose Volume to script on `Master`,
   `Music`, and `Sfx` (rename the exposed params to exactly `MasterVolume`, `MusicVolume`,
   `SfxVolume` — `UI`/`World` don't need their own exposed param, they inherit `Sfx`'s). On the
   `World` group only, add a Lowpass (and optionally Distortion) effect for the camera/TV
   coloring described above; leave `UI` clean.
2. Create an `AudioConfig` asset (`KONTUR/Audio/Audio Config`), assign the mixer, `Music` →
   `MusicGroup`, `UI` → `UiSfxGroup`, `World` → `WorldSfxGroup`; assign the asset to
   `GameLifetimeScope`.
3. Create `SfxCue`/`MusicCue` assets from real clips and assign them on the relevant `*Config`.
4. Add an `AudioEmitter`/`BackgroundMusicTrigger` component to a prefab or scene object, wire its
   fields, and add that GameObject to the scope's **Auto Inject Game Objects** list (see above) —
   otherwise it compiles and runs but never actually receives `IAudioService`.
5. When the Camera system exists: make it move the scene's `AudioListener` to the active camera
   on every switch (see above) — nothing else here depends on it, but nothing sounds "positioned"
   correctly until it does.

## Rules of thumb
- New sound = new cue asset + a field on the relevant system's `*Config`, never a hardcoded
  `AudioClip` reference in code.
- New attached/looping sound on an entity → add/reuse an `AudioEmitter` on its prefab. New
  one-shot world sound not tied to a moving object → `IAudioService.PlaySfxAtPoint`. Both are
  diegetic — they always go through `WorldSfxGroup`, never `UiSfxGroup`.
- Don't add a second way to change volume that bypasses the mixer — UI sliders should call
  `IAudioService.SetXVolume`, not touch `AudioSource.volume` or the mixer directly.
