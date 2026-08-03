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

        [Tooltip("Which AudioEmitter channel this plays on — sounds on different channels can " +
                 "overlap (e.g. footsteps under an attack bark). Whether same-channel sounds cut " +
                 "each other off is decided by MustFinishPlaying below, not by the channel alone. " +
                 "Set Movement for locomotion sounds, Action for attacks/important one-offs, leave " +
                 "General for everything else.")]
        public AudioChannel Channel = AudioChannel.General;

        [Tooltip("If true, this sound can't be cut off by another sound on the same channel until " +
                 "it finishes playing on its own (new same-channel plays are held off instead). If " +
                 "false, this sound can be cut off early by the next same-channel sound that wants " +
                 "to play (e.g. wall-lick should stop as soon as she's done with that action).")]
        public bool MustFinishPlaying = true;

        // Only consulted if the owning LoopingSoundEmitter was constructed with an onEmitted
        // callback (e.g. Employee wires this to Babooshka's HearingSensor.NotifySound).
        public SoundLoudness Loudness = SoundLoudness.Medium;

        [Tooltip("Randomized delay between repeats while this sound type stays active. Set both to the same value for a fixed cadence.")]
        public float MinIntervalSeconds = 1f;
        public float MaxIntervalSeconds = 1f;
    }
}
