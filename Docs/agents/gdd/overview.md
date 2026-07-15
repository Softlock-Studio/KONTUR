# GDD extract: Overview

## Concept
The player commands a K.O.N.T.U.R. operations group containing an outbreak of
fungus **O-41** in a residential apartment block. The player sits in an
observation room and never acts in the world directly: they watch through a
camera system and assign tasks to employees via the UI. Meanwhile the infection
spreads through the house, **random resident events** fire, and the infected
grandmother (**Babooshka**, an experiment subject) roams the building — the main
threat to employees. Each night's goal: keep infection under control and keep
the team alive.

## Core goal: the corridor
The goal is NOT "keep infection below a cap". The player must steer house
infection inside a **corridor [floor; ceiling]** that shifts **upward** from
night to night. A cycle = several nights; every shift must end inside the
corridor.

## Night flow
1. Start: observe house state, assign employees to tasks.
2. During: watch cameras; new events appear; reassign tasks.
3. End: results, resource income, next night.

## Nights
- One night shift ≈ **7 real minutes** (starting value); visible timer.
- Every new night raises baseline infection.

## Lose conditions (preliminary)
- The whole team dies.
- Infection hits a critical level.

## UI — main screen elements
- Selected-camera window (the game's only 3D view)
- Floor map (separate screen) with a task-assignment context menu
- Employee list
- House infection indicator (average over zones)
- Resource counters (shared warehouse pool)
