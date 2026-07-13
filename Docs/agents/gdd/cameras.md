# GDD extract: Cameras & house map

## Camera system
- The player switches between cameras around the house; the selected feed
  renders in the main window — the game's only 3D view.
- Cameras are used to: track employees, spot Babooshka, find new tasks (dirty
  zone, broken camera, ...), and spot threat markers (e.g. a **Vestnik** in
  frame = indicator of an undetected infection hotbed).
- Cameras can be broken; "fix camera" is an employee task.

## House map (mini-map)
- Separate screen with the floor plan.
- Shows: employees, cameras, Babooshka's position (**only if known**),
  infection hotbeds, quick-time events (if added).
- The selected room is highlighted. Clicking a group opens the task context
  menu (same one as camera-side assignment).
- **Fog of war / stale data:** the map shows only what was last observed —
  data ages if a camera hasn't been checked. Applies to monsters/threats.
  **Exception: squads/employees display via GPS** (always current).

## Lore grounding: stabilization by observation
- Human presence holds an Object stable. **ACC** (autonomous surveillance
  system) cameras can perform the stabilizing function instead of a human.
- Precedent: perimeter cameras froze an Object in one state; when the
  recording failed, the space started changing and an entity emerged.
- Mechanical implication: camera observation is an infection-stabilizing
  factor (see infection.md), and a dead camera is dangerous beyond lost
  vision.
- Flavor: ACC records at a lowered framerate when nothing moves (saves
  battery/film).
