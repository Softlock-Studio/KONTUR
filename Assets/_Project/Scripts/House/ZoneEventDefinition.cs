using System;
using UnityEngine;

namespace Game.House
{
    [Serializable]
    public sealed class ZoneEventDefinition
    {
        public ZoneEventType Type;
        public ZoneEventKind Kind = ZoneEventKind.Instant;
        public float CheckIntervalSeconds = 60f;

        [Range(0f, 1f)]
        public float SpawnChance = 0.1f;

        [Tooltip("Ignored when Kind is Condition — a condition event is a boolean, not a counter.")]
        public int MaxConcurrent = 1;

        [Tooltip("<= 0 means this event never expires on its own. > 0 starts a countdown the " +
                 "moment this event becomes active (Instant or Condition alike); if not resolved " +
                 "before it runs out, it counts as a failed task.")]
        public float ExpirySeconds = 0f;
    }
}
