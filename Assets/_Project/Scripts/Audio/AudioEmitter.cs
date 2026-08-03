using Game.Bootstrap;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Audio
{
    // Attach to any entity that needs a sound following its transform (Employee, Babooshka, ...).
    // For one-off world sounds that don't need to track a moving object, use
    // IAudioService.PlaySfxAtPoint instead — this component is only for the attached case.
    //
    // One AudioSource per AudioChannel (not a pool): different channels never cut each other off
    // (e.g. an Action-channel attack bark can play over Movement-channel footsteps), but two
    // sounds on the *same* channel compete for that one source. Whether the new one is allowed to
    // cut off whichever is already playing there is decided per-call by mustFinish — see Play.
    public sealed class AudioEmitter : MonoBehaviour
    {
        private static readonly int ChannelCount = System.Enum.GetValues(typeof(AudioChannel)).Length;

        [Header("Auto-play (optional)")]
        [Tooltip("For static level props with no script deciding when to make sound (a TV, a " +
                 "vent, a dripping pipe, ...) — plays this once the emitter is ready. Leave empty " +
                 "for anything driven by code instead (Play/PlayLoop calls, e.g. Employee/Babooshka).")]
        [SerializeField] private SfxCue autoPlayCue;
        [SerializeField] private AudioChannel autoPlayChannel = AudioChannel.General;
        [SerializeField] private bool autoPlayLoop = true;

        [SerializeField] private bool debugLogging = false;

        private IAudioService audioService;
        private AudioSource[] sources;

        // Per channel: was the sound currently playing there started with mustFinish: true? If so,
        // a new same-channel Play() is held off (returns false, plays nothing) instead of cutting
        // it short; once it finishes on its own, the channel is free again for anything.
        private bool[] protectedChannel;

        // Resolved directly instead of [Inject], so this never depends on the GameObject being
        // added to the owning LifetimeScope's Auto Inject Game Objects list — same pattern as
        // GameCamera/CamerasView. One-time scene lookup, cached; not repeated per Play() call.
        private void Start()
        {
            audioService = LifetimeScope.Find<GameLifetimeScope>().Container.Resolve<IAudioService>();

            sources = new AudioSource[ChannelCount];
            protectedChannel = new bool[ChannelCount];
            for (int i = 0; i < sources.Length; i++)
                sources[i] = audioService.CreateAttachedSource(transform);

            if (autoPlayCue != null)
            {
                if (autoPlayLoop) PlayLoop(autoPlayCue, autoPlayChannel);
                else Play(autoPlayCue, autoPlayChannel);
            }
        }

        // Returns whether it actually started — false means the channel is busy with a
        // mustFinish: true sound and this call was held off instead of cutting it off. Callers
        // that need to retry (LoopingSoundEmitter) can poll on the return value.
        public bool Play(SfxCue cue, AudioChannel channel = AudioChannel.General, bool mustFinish = true) =>
            TryPlay(cue, channel, loop: false, mustFinish);

        public bool PlayLoop(SfxCue cue, AudioChannel channel = AudioChannel.General, bool mustFinish = true) =>
            TryPlay(cue, channel, loop: true, mustFinish);

        public void Stop()
        {
            if (sources == null) return;
            foreach (AudioSource source in sources) source.Stop();
            System.Array.Clear(protectedChannel, 0, protectedChannel.Length);
        }

        private bool TryPlay(SfxCue cue, AudioChannel channel, bool loop, bool mustFinish)
        {
            int index = (int)channel;
            AudioSource source = sources?[index];

            if (cue == null || source == null)
            {
#if UNITY_EDITOR
                if (debugLogging)
                {
                    string reason = source == null
                        ? "no AudioSource yet (Start() hasn't run on this AudioEmitter — called too early, or the GameObject is inactive)"
                        : "cue is null (SfxCue field not assigned in the config)";
                    Debug.Log($"[{name}] AudioEmitter skipped: {reason}", this);
                }
#endif
                return false;
            }

            if (source.isPlaying && protectedChannel[index])
            {
#if UNITY_EDITOR
                if (debugLogging)
                    Debug.Log($"[{name}] AudioEmitter held off {cue.name} on {channel} — a mustFinish sound is still playing there", this);
#endif
                return false;
            }

            ApplyCue(source, cue);
            source.loop = loop;
            source.Play();
            protectedChannel[index] = mustFinish;

#if UNITY_EDITOR
            if (debugLogging)
                Debug.Log($"[{name}] AudioEmitter played {cue.name} on {channel} → clip \"{(source.clip != null ? source.clip.name : "none — Clips[] is empty on this SfxCue")}\" | active listener: {DescribeActiveListener()}", this);
#endif
            return true;
        }

#if UNITY_EDITOR
        // Answers "where is this actually being heard from right now" — logs every enabled
        // AudioListener in the scene (there should be exactly one; more is a Unity-level bug that
        // logs its own warning, zero means this sound is genuinely inaudible).
        private static string DescribeActiveListener()
        {
            AudioListener[] listeners = Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
            var enabled = new System.Collections.Generic.List<string>();
            foreach (AudioListener listener in listeners)
                if (listener.enabled && listener.gameObject.activeInHierarchy)
                    enabled.Add($"{listener.name} @ {listener.transform.position}");

            return enabled.Count == 0 ? "NONE (should be silent — if you can hear this, something else is off)" : string.Join(", ", enabled);
        }
#endif

        private static void ApplyCue(AudioSource source, SfxCue cue)
        {
            source.clip = cue.GetClip();
            source.volume = cue.Volume;
            source.pitch = cue.GetPitch();
        }
    }
}
