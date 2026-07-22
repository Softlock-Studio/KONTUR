using System;
using Game.Audio;

namespace Game.AI.Employee
{
    [Serializable]
    public sealed class EmployeeSoundDefinition
    {
        public EmployeeSoundType Type;
        public SfxCue Cue;
        public SoundLoudness Loudness = SoundLoudness.Medium;
    }
}
