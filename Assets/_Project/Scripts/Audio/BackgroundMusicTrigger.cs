using Game.Bootstrap;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Audio
{
    // Drop on any scene GameObject to start a music cue when that scene/mission loads.
    // No code changes needed to swap tracks — just reassign musicCue in the inspector.
    public sealed class BackgroundMusicTrigger : MonoBehaviour
    {
        [SerializeField] private MusicCue musicCue;
        [SerializeField] private float fadeSeconds = -1f; // -1 = AudioConfig.DefaultMusicFadeSeconds

        // Resolved directly instead of [Inject], so this never depends on the GameObject being
        // added to the owning LifetimeScope's Auto Inject Game Objects list — same pattern as
        // GameCamera/AudioEmitter. Also means it works in scenes with no MissionScope of their
        // own (e.g. MainMenu), since IAudioService lives in the root GameLifetimeScope.
        private void Start()
        {
            IAudioService audioService = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IAudioService>();
            audioService.PlayMusic(musicCue, fadeSeconds);
        }
    }
}
