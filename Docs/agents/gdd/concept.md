# Concept

Player controls the K.O.N.T.U.R. operational group conducting containment of fungal infection O-41 in a residential building.
Player is in an observation room and does not participate directly in operations.
Player coordinates staff via camera system and management interface: assigns tasks, redistributes assignments, reacts to threats.
Simultaneously: infection spreads, random resident events occur, and the infected grandmother (main threat) roams the building.
Goal each night: keep infection under control and preserve the team.

## Game loop

**Macro cycle:** maintain infection level within a corridor [floor; ceiling] across multiple nights; the corridor shifts upward each night.

**Night start:**
1. Observe current house state
2. Assign staff to tasks
3. Monitor via cameras
4. React to new events / reassign tasks

**Night end:**
- Receive results
- Receive resources
- Begin next night

## Defeat conditions (preliminary)

- Death of the entire team
- Critical infection level
