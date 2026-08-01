using UnityEngine;
using VContainer;

namespace Game.Audio
{
    // Attach to any entity that needs a sound following its transform (Employee, Babooshka, ...).
    // For one-off world sounds that don't need to track a moving object, use
    // IAudioService.PlaySfxAtPoint instead — this component is only for the attached case.
    public sealed class AudioEmitter : MonoBehaviour
    {
        [SerializeField] private bool debugLogging = false;

        private IAudioService audioService;
        private AudioSource source;

        [Inject]
        public void Construct(IAudioService audioService)
        {
            this.audioService = audioService;
        }

        // Deferred to Start: [Inject] runs during the owning LifetimeScope's build, which isn't
        // guaranteed to complete before this object's own Awake — Unity only guarantees all
        // Awakes finish before any Start (same pattern as LocalizedTextTMP.Construct).
        private void Start()
        {
            if (audioService == null)
            {
                Debug.LogWarning($"[{name}] AudioEmitter has no IAudioService — add this GameObject to the owning LifetimeScope's Auto Inject Game Objects list.", this);
                return;
            }

            source = audioService.CreateAttachedSource(transform);
        }

        public void Play(SfxCue cue) => TryPlay(cue, loop: false);

        public void PlayLoop(SfxCue cue) => TryPlay(cue, loop: true);

        public void Stop() => source?.Stop();

        private void TryPlay(SfxCue cue, bool loop)
        {
            if (cue == null || source == null)
            {
#if UNITY_EDITOR
                if (debugLogging)
                {
                    string reason = source == null
                        ? "no AudioSource (IAudioService never injected — add this GameObject to the owning LifetimeScope's Auto Inject Game Objects list)"
                        : "cue is null (SfxCue field not assigned in the config)";
                    Debug.Log($"[{name}] AudioEmitter skipped: {reason}", this);
                }
#endif
                return;
            }

            ApplyCue(cue);
            source.loop = loop;
            source.Play();

#if UNITY_EDITOR
            if (debugLogging)
                Debug.Log($"[{name}] AudioEmitter played {cue.name} → clip \"{(source.clip != null ? source.clip.name : "none — Clips[] is empty on this SfxCue")}\"", this);
#endif
        }

        private void ApplyCue(SfxCue cue)
        {
            source.clip = cue.GetClip();
            source.volume = cue.Volume;
            source.pitch = cue.GetPitch();
        }
    }
}
