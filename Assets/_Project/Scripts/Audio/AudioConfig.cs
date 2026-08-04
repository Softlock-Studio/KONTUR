using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Game.Audio
{
    [CreateAssetMenu(menuName = "KONTUR/Audio/Audio Config", fileName = "AudioConfig")]
    public sealed class AudioConfig : ScriptableObject
    {
        [Header("Mixer")]
        [Tooltip("Основной AudioMixer проекта.")]
        public AudioMixer Mixer;
        [Tooltip("Группа микшера для музыки.")]
        public AudioMixerGroup MusicGroup;

        [Tooltip("Звуки меню/HUD — играются чисто, без фильтров. Используется через PlayUiSfx.")]
        public AudioMixerGroup UiSfxGroup;

        [Tooltip("Внутриигровые звуки (шаги, рычание, облизывание стены и т.д.) — игрок слышит их " +
                 "только через камеру наблюдения, а не «в комнате», поэтому именно на эту группу " +
                 "вешается фильтр «как из телевизора/колонки» (Lowpass/Distortion) на самом ассете " +
                 "Mixer. Используется через PlaySfxAtPoint и AudioEmitter.")]
        [FormerlySerializedAs("SfxGroup")]
        public AudioMixerGroup WorldSfxGroup;

        [Header("Mixer exposed parameter names")]
        [Tooltip("Имя параметра громкости мастер-канала. Должно точно совпадать с именем, открытым на ассете Mixer через «Expose ... to script».")]
        public string MasterVolumeParam = "MasterVolume";
        [Tooltip("Имя параметра громкости музыки. Должно точно совпадать с именем, открытым на ассете Mixer через «Expose ... to script».")]
        public string MusicVolumeParam = "MusicVolume";
        [Tooltip("Имя параметра громкости звуковых эффектов. Должно точно совпадать с именем, открытым на ассете Mixer через «Expose ... to script».")]
        public string SfxVolumeParam = "SfxVolume";

        [Header("Defaults")]
        [Tooltip("Громкость мастер-канала по умолчанию (0–1), пока нет сохранённых пользовательских настроек.")]
        [Range(0f, 1f)] public float DefaultMasterVolume = 1f;
        [Tooltip("Громкость музыки по умолчанию (0–1), пока нет сохранённых пользовательских настроек.")]
        [Range(0f, 1f)] public float DefaultMusicVolume = 0.8f;
        [Tooltip("Громкость звуковых эффектов по умолчанию (0–1), пока нет сохранённых пользовательских настроек.")]
        [Range(0f, 1f)] public float DefaultSfxVolume = 1f;
        [Tooltip("Длительность плавного перехода (fade) между музыкальными треками, в секундах.")]
        public float DefaultMusicFadeSeconds = 1.5f;

        [Header("Pooling")]
        [Tooltip("Количество источников звука (AudioSource) в пуле для одновременного проигрывания SFX.")]
        public int SfxPoolSize = 8;

        [Header("World SFX 3D falloff (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("Минимальная дистанция (AudioSource.minDistance) для внутриигровых звуков (прикреплённых эмиттеров и PlaySfxAtPoint) — подобрана под масштаб комнат дома, а не под завышенные значения Unity по умолчанию (1/500).")]
        public float WorldSfxMinDistance = 1f;
        [Tooltip("Максимальная дистанция (AudioSource.maxDistance), на которой внутриигровой звук ещё слышен — подобрана под масштаб комнат дома, а не под завышенные значения Unity по умолчанию (1/500).")]
        public float WorldSfxMaxDistance = 15f;

        [Tooltip("Где находится единственный постоянный мировой AudioListener, когда не выбрана ни одна камера — достаточно далеко за пределами WorldSfxMaxDistance, чтобы любой мировой звук становился неслышным, при этом UI/музыка (spatialBlend 0, не зависят от позиции слушателя) остаются слышны.")]
        public Vector3 WorldListenerParkPosition = new(0f, -1000f, 0f);
    }
}
