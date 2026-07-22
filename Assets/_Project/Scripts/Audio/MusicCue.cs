using UnityEngine;

namespace Game.Audio
{
    [CreateAssetMenu(menuName = "KONTUR/Audio/Music Cue", fileName = "MusicCue")]
    public sealed class MusicCue : ScriptableObject
    {
        public AudioClip Clip;
        [Range(0f, 1f)] public float Volume = 1f;
        public bool Loop = true;
    }
}
