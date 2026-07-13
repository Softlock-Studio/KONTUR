# GDD extracts for agents

English, per-system, facts-only extracts of the game design document. A
small-context agent working on one system loads ONLY its extract.

- Full GDD (Russian, canonical, live document):
  https://docs.google.com/document/d/1T0SfcPm2yhoArpBzUWWpfS4JBkCw-C-TdD8U6NgN4LE
- Universe wiki (lore): https://konturproject.fandom.com/ru/wiki/КОНТУР_Вики

| Extract | Covers |
|---|---|
| `overview.md` | concept, game loop, nights, lose conditions, UI layout |
| `infection.md` | zone infection model, growth/reduction factors, resources, fungus lore |
| `tasks.md` | employee assignment UX, task list, residents |
| `babooshka.md` | main threat: behavior + grounded stabilization rules |
| `cameras.md` | camera system, minimap, fog of war, observation-stabilization |
| `entities.md` | spore syndrome, future entity roster, protocols |

Rules:
- Facts and constraints, not vision prose. Numbers the GDD leaves blank are
  marked **TBD** — never invent values; flag them in your handoff instead.
- The GDD is the source of truth; extracts are a cache. On conflict, the GDD
  wins — fix the extract in the same PR (or flag the mismatch if you can't
  read the GDD).
