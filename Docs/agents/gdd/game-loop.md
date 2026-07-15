# Game Loop

## Infection corridor
Goal is **not** "keep infection below a ceiling." Goal is to keep infection in a
**corridor [floor; ceiling]** that shifts upward from night to night.

## Big cycle
Maintain infection level across **several nights**. Each night of the cycle must
end within the acceptable infection corridor.

## Night start phase
1. Observe current state of the building.
2. Assign employees to tasks.
3. Watch cameras.
4. New events appear.
5. Reassign tasks as needed.

## Night end phase
1. Receive results.
2. Receive resources.
3. **Between-levels screen** — displays a text note / shift report; player presses Continue to launch the next night.
4. Begin next night.
