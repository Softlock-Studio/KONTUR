using UnityEngine;
using VContainer;

namespace Game.Audio
{
    // Drop on any scene GameObject to start a music cue when that scene/mission loads.
    // No code changes needed to swap tracks — just reassign musicCue in the inspector.
    public sealed class BackgroundMusicTrigger : MonoBehaviour
    {
        [SerializeField] private MusicCue musicCue;
        [SerializeField] private float fadeSeconds = -1f; // -1 = AudioConfig.DefaultMusicFadeSeconds

        private IAudioService audioService;

        [Inject]
        public void Construct(IAudioService audioService)
        {
            this.audioService = audioService;
        }

        // Deferred to Start: see AudioEmitter for why [Inject] can't be relied on in Awake.
        private void Start()
        {
            if (audioService == null)
            {
                Debug.LogWarning($"[{name}] BackgroundMusicTrigger has no IAudioService — add this GameObject to the owning LifetimeScope's Auto Inject Game Objects list.", this);
                return;
            }

            audioService.PlayMusic(musicCue, fadeSeconds);
        }
    }
}
