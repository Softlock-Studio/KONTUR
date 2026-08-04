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

        [Header("World SFX 3D falloff (TBD placeholder values, not GDD-sourced)")]
        [Tooltip("AudioSource.minDistance/maxDistance for world sounds (attached emitters and " +
                 "PlaySfxAtPoint) — tuned to house-room scale, not Unity's oversized defaults (1/500).")]
        public float WorldSfxMinDistance = 1f;
        public float WorldSfxMaxDistance = 15f;

        [Tooltip("Where the single persistent world AudioListener sits when no camera is selected " +
                 "— far enough past WorldSfxMaxDistance that every world sound falls silent, while " +
                 "UI/music (spatialBlend 0, listener-position-independent) stay audible.")]
        public Vector3 WorldListenerParkPosition = new(0f, -1000f, 0f);
    }
}
