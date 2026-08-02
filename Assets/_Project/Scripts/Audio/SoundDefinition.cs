using System;
using UnityEngine;

namespace Game.Audio
{
    // Generic over TSoundType so each entity (Employee, Babooshka, ...) keeps its own sound-type
    // vocabulary (enum) while sharing one data shape — see LoopingSoundEmitter<TSoundType>.
    [Serializable]
    public sealed class SoundDefinition<TSoundType> where TSoundType : struct, Enum
    {
        public TSoundType Type;
        public SfxCue Cue;

        // Only consulted if the owning LoopingSoundEmitter was constructed with an onEmitted
        // callback (e.g. Employee wires this to Babooshka's HearingSensor.NotifySound).
        public SoundLoudness Loudness = SoundLoudness.Medium;

        [Tooltip("Randomized delay between repeats while this sound type stays active. Set both to the same value for a fixed cadence.")]
        public float MinIntervalSeconds = 1f;
        public float MaxIntervalSeconds = 1f;
    }
}
