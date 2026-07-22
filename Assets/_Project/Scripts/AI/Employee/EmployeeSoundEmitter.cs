using UnityEngine;
using Game.AI.Babooshka;

namespace Game.AI.Employee
{
    // Periodically re-triggers whichever sound the currently active state feeds it — both the
    // audible SFX (AudioEmitter) and the gameplay hearing ping (HearingSensor) — every
    // EmployeeConfig.SoundEmitIntervalSeconds, so a multi-second action (walking, cleaning, ...)
    // stays noticeable instead of pinging once and going silent. Call ResetTimer() from a state's
    // OnEnter so switching actions always pulses immediately rather than waiting out a stale timer.
    public sealed class EmployeeSoundEmitter
    {
        private readonly Transform origin;
        private readonly EmployeeConfig config;
        private readonly Game.Audio.AudioEmitter audioEmitter;
        private readonly HearingSensor[] hearingSensorsToNotify;

        private float timer;

        public EmployeeSoundEmitter(Transform origin, EmployeeConfig config, Game.Audio.AudioEmitter audioEmitter,
            HearingSensor[] hearingSensorsToNotify)
        {
            this.origin = origin;
            this.config = config;
            this.audioEmitter = audioEmitter;
            this.hearingSensorsToNotify = hearingSensorsToNotify;
        }

        public void ResetTimer() => timer = 0f;

        public void Tick(EmployeeSoundType type, float deltaTime)
        {
            timer -= deltaTime;
            if (timer > 0f) return;

            timer = config.SoundEmitIntervalSeconds;
            Emit(type);
        }

        private void Emit(EmployeeSoundType type)
        {
            if (!config.TryGetSound(type, out EmployeeSoundDefinition sound)) return;

            audioEmitter?.Play(sound.Cue);

            if (hearingSensorsToNotify == null) return;
            foreach (HearingSensor sensor in hearingSensorsToNotify)
                sensor.NotifySound(origin.position, sound.Loudness);
        }
    }
}
