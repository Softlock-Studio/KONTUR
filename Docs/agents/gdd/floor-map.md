# Floor Map (Mini-Map)

- Separate screen showing the floor plan.
- Displays:
  - Employees
  - Cameras
  - Grandmother position (if known)
  - Infection outbreaks
  - Quick-time event markers (if implemented)
- Selected room is highlighted.
- Clicking a squad on the map → context menu for task assignment.

## Fog of war
- Map shows only what was last observed; data "go stale" if a camera has not been
  checked recently.
- Staleness is particularly relevant for entity tracking.
- Squads (employees) are displayed via GPS — always current position.
