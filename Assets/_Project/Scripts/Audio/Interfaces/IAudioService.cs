using System;
using UnityEngine;

namespace Game.Audio
{
    public interface IAudioService
    {
        float MasterVolume { get; }
        float MusicVolume { get; }
        float SfxVolume { get; }

        event Action VolumeChanged;

        void SetMasterVolume(float value01);
        void SetMusicVolume(float value01);
        void SetSfxVolume(float value01);

        void PlayMusic(MusicCue cue, float fadeSeconds = -1f);
        void StopMusic(float fadeSeconds = -1f);

        void PlayUiSfx(SfxCue cue);
        void PlaySfxAtPoint(SfxCue cue, Vector3 position);

        AudioSource CreateAttachedSource(Transform parent);
    }
}
