using UnityEngine;

namespace Game.Audio
{
    [CreateAssetMenu(menuName = "KONTUR/Audio/Sfx Cue", fileName = "SfxCue")]
    public sealed class SfxCue : ScriptableObject
    {
        public AudioClip[] Clips;
        [Range(0f, 1f)] public float Volume = 1f;
        public Vector2 PitchRange = new Vector2(1f, 1f);

        public AudioClip GetClip()
        {
            if (Clips == null || Clips.Length == 0) return null;
            return Clips[Random.Range(0, Clips.Length)];
        }

        public float GetPitch() => Random.Range(PitchRange.x, PitchRange.y);
    }
}
