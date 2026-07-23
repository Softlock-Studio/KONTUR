using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Game.Audio
{
    [CreateAssetMenu(menuName = "KONTUR/Audio/Audio Config", fileName = "AudioConfig")]
    public sealed class AudioConfig : ScriptableObject
    {
        [Header("Mixer")]
        public AudioMixer Mixer;
        public AudioMixerGroup MusicGroup;

        [Tooltip("Menu/HUD sounds — played clean, not filtered. Routed by PlayUiSfx.")]
        public AudioMixerGroup UiSfxGroup;

        [Tooltip("In-world sounds (footsteps, growls, wall-lick, ...) — the player only ever " +
                 "hears these through a security camera feed, not \"in the room\", so this group " +
                 "is where a TV/speaker coloring filter (Lowpass/Distortion) belongs, applied on " +
                 "the Mixer asset itself. Routed by PlaySfxAtPoint and AudioEmitter.")]
        [FormerlySerializedAs("SfxGroup")]
        public AudioMixerGroup WorldSfxGroup;

        [Header("Mixer exposed parameter names")]
        [Tooltip("Must match the names exposed on the Mixer asset via \"Expose ... to script\".")]
        public string MasterVolumeParam = "MasterVolume";
        public string MusicVolumeParam = "MusicVolume";
        public string SfxVolumeParam = "SfxVolume";

        [Header("Defaults")]
        [Range(0f, 1f)] public float DefaultMasterVolume = 1f;
        [Range(0f, 1f)] public float DefaultMusicVolume = 0.8f;
        [Range(0f, 1f)] public float DefaultSfxVolume = 1f;
        public float DefaultMusicFadeSeconds = 1.5f;

        [Header("Pooling")]
        public int SfxPoolSize = 8;
    }
}
