# GDD extract: Infection & resources

## Zone model
- The house is split into zones (rooms, corridor). Each zone has ONE infection
  parameter, **0–100%**, no sub-scales.
- House indicator = **average over zones**, always visible on screen.
- Infection spreads gradually between zones during the night; spread is
  **faster in darkness**.
- After every night the overall level rises (see overview.md: corridor goal).

## Full-house scan
One button: measures all zones at once and lifts fog of war everywhere.
Costs **generator fuel**. The fresh-data effect persists for a while, then zone
data starts going stale again.

## Growth factors (Δ infection/hour — values TBD in GDD)
- Darkness
- No camera observation
- No treatment

## Reduction actions (Δ infection/hour — values TBD in GDD)
- Vinegar treatment · iodine-resorcinol treatment · KONTUR-agent treatment
- Laying carpets · light · camera observation

## Resources
Shared warehouse: an integer counter per resource type, shown on screen.
It's a common pool — employees do not physically carry stock.

| Resource | Effect |
|---|---|
| Vinegar 9% | cheap, common; weak one-shot reduction |
| Iodine-resorcinol-A | rare; strong, long effect; **also repels Babooshka** |
| KONTUR agent | long-lasting growth-rate reduction in a room (lore: −89%) |
| Carpets | prophylactic; weaker than KONTUR agent, cheap; must be re-laid every night; **prevents Vestnik** |
| Light bulbs | consumable for the "change bulb" task; removes the Darkness factor in a room; improves camera visibility |
| Generator fuel | one shared tank per night; powers ACC/cameras/lighting and the full scan; at 0 the generator shuts down |

Open questions in the GDD (TBD): generator consumption rate, effect durations,
whether resources ever spawn randomly (leaning: no, income between nights).

## Lore grounding (O-41 physics — keep mechanics consistent with this)
- Grows inside concrete; spread by infected people touching/licking walls;
  tell-tale sign: tap water turns black.
- Grows faster in darkness (stairwell lights are kept on deliberately).
- Reagent potency, weak → strong: vinegar < carpets < KONTUR agent <
  iodine-resorcinol-A (used on critical infrastructure; "metro smell" causes
  revulsion in the infected).
- Canonical measurement metrics exist (BAS, SD, PA; field device "Baz M") —
  use for UI flavor copy only; gameplay uses the single 0–100% per zone.
