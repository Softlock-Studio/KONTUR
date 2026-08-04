using System;
using UnityEngine;

namespace Game.Audio
{
    // Generic over TSoundType so each entity (Employee, Babooshka, ...) keeps its own sound-type
    // vocabulary (enum) while sharing one data shape — see LoopingSoundEmitter<TSoundType>.
    [Serializable]
    public sealed class SoundDefinition<TSoundType> where TSoundType : struct, Enum
    {
        [Tooltip("Тип звука из перечисления, специфичного для этой сущности (сотрудник, бабушка и т.д.).")]
        public TSoundType Type;
        [Tooltip("Звуковой файл (SfxCue), который проигрывается для этого типа.")]
        public SfxCue Cue;

        [Tooltip("На каком канале AudioEmitter проигрывается звук — звуки на разных каналах могут " +
                 "звучать одновременно (например, шаги поверх атакующего рыка). Обрывает ли звук на " +
                 "том же канале другой звук — решает не сам канал, а флаг MustFinishPlaying ниже. " +
                 "Ставьте Movement для звуков передвижения, Action для атак/важных разовых звуков, " +
                 "General оставляйте для всего остального.")]
        public AudioChannel Channel = AudioChannel.General;

        [Tooltip("Если включено, этот звук нельзя прервать другим звуком на том же канале, пока он " +
                 "не доиграет сам (новые проигрывания на этом канале ждут своей очереди). Если " +
                 "выключено, звук может быть прерван следующим звуком на том же канале раньше " +
                 "времени (например, облизывание стены должно обрываться, как только действие закончилось).")]
        public bool MustFinishPlaying = true;

        [Tooltip("Громкость звука с точки зрения слуха ИИ (Low/Medium/High). Учитывается только " +
                 "если издающий звук объект передаёт события в HearingSensor через callback onEmitted.")]
        public SoundLoudness Loudness = SoundLoudness.Medium;

        [Tooltip("Случайная задержка между повторами, пока этот тип звука активен. Минимальное значение диапазона повтора — поставьте одинаковые Min и Max для фиксированного интервала.")]
        public float MinIntervalSeconds = 1f;
        [Tooltip("Случайная задержка между повторами, пока этот тип звука активен. Максимальное значение диапазона повтора — поставьте одинаковые Min и Max для фиксированного интервала.")]
        public float MaxIntervalSeconds = 1f;
    }
}
