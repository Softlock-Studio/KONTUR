# Infection System

## Zone model
- Building divided into zones (rooms, corridor).
- Each zone has **one infection parameter** (0–100%), no sub-scales.
- **House indicator** = average across all zones; displayed somewhere on screen.

## Full house scan
- Single button: lifts fog of war across all zones at once.
- Costs **generator fuel**.
- Effect persists for a duration, then data begin going stale again.
- Duration: **TBD**. Fuel cost per scan: **TBD**.

## Spread
- Infection spreads gradually through the building during the night.
- **Spreads faster in darkness.** Darkness can be created by the "lightbulb outage"
  random event (see [residents.md](residents.md)).
- **In light, spread is very slow** (qualitative design intent; exact rate TBD).
- After each night, overall infection level rises.

## Growth factors

| Factor | Δ infection/hour |
| ------ | ---------------- |
| Darkness | TBD |
| No observation | TBD |
| No treatment | TBD |

## Treatment actions (reduce infection)

| Action | Δ infection/hour |
| ------ | ---------------- |
| Vinegar treatment | TBD |
| Iodine-resorcinol treatment | TBD |
| KONTUR treatment | TBD |
| Lay carpets | TBD |
| Light (bulb replaced) | TBD |
| Camera observation | TBD |
