# GDD extract: Babooshka (the grandmother)

The main threat. An infected woman, 62, medium-to-heavy spore syndrome
(infected via the Church of Sprouting cult). Canonical source: the 1998
"Complex experiment with a spore-infected subject" — the A-93 / Sporinol
trials. Code-side implementation of her AI: see `Docs/agents/systems/ai.md`.

## In-game behavior
- Roams the house freely for most of the night.
- At certain moments of the night she deliberately moves toward employees.
- On contact she **kills the employee**. Kill probability (%) derives from the
  overall house infection level.
- Design intent: **predictable at the rules level, tactically uncertain** in
  the moment. The player tracks her via cameras; her map position shows only
  when known (fog of war, stale data — see cameras.md).

## Grounded stabilization rules (from the 1998 experiment — mechanics material)
- **Medicated meat** served at fixed times (**9:00 and 18:00**) stabilizes her.
  Missed feeding → she goes looking for fresh meat herself.
- **Chains** before the night cycle reduce her activity. Not chained → she
  comes looking for you.
- **Contact at least once every 3 hours** lowers the chance of a sudden switch
  to aggression.
- **Iodine-resorcinol-A repels her** (revulsion in the infected).
- Detection by **pulse/stress** is diegetic in this universe.

Player tasks touching her (from the task table): lure/summon her, leave food.

## Explicitly rejected by the GDD
Arbitrary ritual rules from adjacent materials ("whisper into the wardrobe",
"prop the door with a book") are **not used**. Only fungally/pharmacologically
grounded rules — the list above. Do not invent ritual mechanics.
