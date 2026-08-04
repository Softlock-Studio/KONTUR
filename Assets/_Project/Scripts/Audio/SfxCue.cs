using UnityEngine;

namespace Game.Audio
{
    [CreateAssetMenu(menuName = "KONTUR/Audio/Sfx Cue", fileName = "SfxCue")]
    public sealed class SfxCue : ScriptableObject
    {
        [Tooltip("Варианты аудиоклипов — при каждом проигрывании случайно выбирается один клип из списка.")]
        public AudioClip[] Clips;
        [Tooltip("Громкость звука (0–1) относительно группы микшера, на которую он играет.")]
        [Range(0f, 1f)] public float Volume = 1f;
        [Tooltip("Диапазон случайной высоты тона (pitch) при проигрывании: X — минимум, Y — максимум. Одинаковые значения X и Y отключают вариацию.")]
        public Vector2 PitchRange = new Vector2(1f, 1f);

        public AudioClip GetClip()
        {
            if (Clips == null || Clips.Length == 0) return null;
            return Clips[Random.Range(0, Clips.Length)];
        }

        public float GetPitch() => Random.Range(PitchRange.x, PitchRange.y);
    }
}
