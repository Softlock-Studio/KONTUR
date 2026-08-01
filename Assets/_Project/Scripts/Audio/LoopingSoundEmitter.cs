using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    // Periodically re-triggers whichever sound type a caller feeds it — both the audible SFX
    // (AudioEmitter) and an optional gameplay hearing ping (onEmitted) — while that type stays
    // active, so a multi-second action (walking, cleaning, laughing, ...) keeps being noticeable
    // instead of pinging once and going silent. Generic over TSoundType so every entity keeps its
    // own sound-type enum/vocabulary; this class only knows the shared mechanism.
    //
    // Each type gets its own independent timer (not one shared timer for the whole emitter) —
    // needed because a single entity can have more than one sound category active at once (e.g.
    // Babooshka's footsteps and laughter both ticking during Wander), each with its own cadence.
    public sealed class LoopingSoundEmitter<TSoundType> where TSoundType : struct, Enum
    {
        private readonly Transform origin;
        private readonly AudioEmitter audioEmitter;
        private readonly IReadOnlyList<SoundDefinition<TSoundType>> definitions;
        private readonly Action<Vector3, SoundLoudness> onEmitted;

        private readonly Dictionary<TSoundType, float> timers = new();

        public LoopingSoundEmitter(Transform origin, AudioEmitter audioEmitter,
            IReadOnlyList<SoundDefinition<TSoundType>> definitions, Action<Vector3, SoundLoudness> onEmitted = null)
        {
            this.origin = origin;
            this.audioEmitter = audioEmitter;
            this.definitions = definitions;
            this.onEmitted = onEmitted;
        }

        // Call from a state's OnEnter (for whichever type it's about to Tick) so switching into
        // it always pulses immediately instead of waiting out wherever that type's timer happened
        // to be left at.
        public void ResetTimer(TSoundType type) => timers[type] = 0f;

        public void Tick(TSoundType type, float deltaTime)
        {
            if (!TryGetDefinition(type, out SoundDefinition<TSoundType> definition)) return;

            float timer = timers.TryGetValue(type, out float t) ? t : 0f;
            timer -= deltaTime;
            if (timer > 0f)
            {
                timers[type] = timer;
                return;
            }

            timers[type] = UnityEngine.Random.Range(definition.MinIntervalSeconds, definition.MaxIntervalSeconds);

            audioEmitter?.Play(definition.Cue);
            onEmitted?.Invoke(origin.position, definition.Loudness);
        }

        private bool TryGetDefinition(TSoundType type, out SoundDefinition<TSoundType> found)
        {
            for (int i = 0; i < definitions.Count; i++)
            {
                if (!EqualityComparer<TSoundType>.Default.Equals(definitions[i].Type, type)) continue;
                found = definitions[i];
                return true;
            }

            found = null;
            return false;
        }
    }
}
