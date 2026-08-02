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

        // Moves the single persistent world listener to hear as if standing at this position/
        // rotation — call when a camera becomes the selected one. UI/music are unaffected (they
        // don't spatialize), only attached/point world sounds change in audibility from this.
        void SetWorldListenerPosition(Vector3 position, Quaternion rotation);

        // Parks the world listener far away so every world sound falls silent — call when no
        // camera is selected (no signal).
        void ParkWorldListener();
    }
}
