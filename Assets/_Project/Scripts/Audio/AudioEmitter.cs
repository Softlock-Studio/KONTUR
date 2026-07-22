using Game.House;
using UnityEngine;
using VContainer;

namespace Game.Audio
{
    // Attach to any entity that needs a sound following its transform (Employee, Babooshka, ...).
    // For one-off world sounds that don't need to track a moving object, use
    // IAudioService.PlaySfxAtPoint instead — this component is only for the attached case.
    public sealed class AudioEmitter : MonoBehaviour
    {
        private IAudioService audioService;
        private ICameraObservationService cameraObservation;
        private AudioSource source;

        // cameraObservation is optional (default null) so scenes without the camera system
        // (e.g. standalone AI test scenes with no MissionScope) don't fail this whole injection
        // and lose audioService along with it — see IsAudible for the matching fail-open rule.
        [Inject]
        public void Construct(IAudioService audioService, ICameraObservationService cameraObservation = null)
        {
            this.audioService = audioService;
            this.cameraObservation = cameraObservation;
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

        public void Play(SfxCue cue, Zone zone = null)
        {
            if (cue == null || source == null || !IsAudible(zone)) return;

            ApplyCue(cue);
            source.loop = false;
            source.Play();
        }

        public void PlayLoop(SfxCue cue, Zone zone = null)
        {
            if (cue == null || source == null || !IsAudible(zone)) return;

            ApplyCue(cue);
            source.loop = true;
            source.Play();
        }

        public void Stop() => source?.Stop();

        // The player never hears this "in the room" — only through whichever camera is currently
        // selected. Falls open (audible) if the camera system isn't wired up in this scene, same
        // rationale as Zone.TrySpendResource's standalone-debug-path fallback.
        private bool IsAudible(Zone zone)
        {
            if (cameraObservation == null) return true;
            return zone != null ? cameraObservation.IsObserving(zone) : cameraObservation.IsObserving(transform.position);
        }

        private void ApplyCue(SfxCue cue)
        {
            source.clip = cue.GetClip();
            source.volume = cue.Volume;
            source.pitch = cue.GetPitch();
        }
    }
}
