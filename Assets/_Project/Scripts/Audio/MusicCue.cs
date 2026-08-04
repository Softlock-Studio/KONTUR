using UnityEngine;

namespace Game.Audio
{
    [CreateAssetMenu(menuName = "KONTUR/Audio/Music Cue", fileName = "MusicCue")]
    public sealed class MusicCue : ScriptableObject
    {
        [Tooltip("Аудиоклип музыкального трека.")]
        public AudioClip Clip;
        [Tooltip("Громкость трека (0–1) относительно группы микшера музыки.")]
        [Range(0f, 1f)] public float Volume = 1f;
        [Tooltip("Зациклить трек при проигрывании.")]
        public bool Loop = true;
    }
}
