using System;
using UnityEngine;
using VContainer.Unity;

namespace Game.Audio
{
    public sealed class AudioService : IAudioService, IStartable, ITickable, IDisposable
    {
        private const string MasterVolumeKey = "Audio.MasterVolume";
        private const string MusicVolumeKey = "Audio.MusicVolume";
        private const string SfxVolumeKey = "Audio.SfxVolume";
        private const float MuteDecibels = -80f;

        private readonly AudioConfig config;

        private GameObject root;
        private AudioListener worldListener;
        private AudioSource musicA;
        private AudioSource musicB;
        private AudioSource[] sfxPool;
        private int sfxCursor;

        private AudioSource activeMusic;
        private AudioSource fadingOutMusic;
        private float fadingOutStartVolume;
        private MusicCue currentCue;
        private float fadeTimer;
        private float fadeDuration;

        public float MasterVolume { get; private set; }
        public float MusicVolume { get; private set; }
        public float SfxVolume { get; private set; }

        public event Action VolumeChanged;

        public AudioService(AudioConfig config)
        {
            this.config = config;
        }

        public void Start()
        {
            root = new GameObject("AudioService");
            UnityEngine.Object.DontDestroyOnLoad(root);

            musicA = CreateSource("MusicA", config.MusicGroup, spatialBlend: 0f);
            musicB = CreateSource("MusicB", config.MusicGroup, spatialBlend: 0f);

            sfxPool = new AudioSource[Mathf.Max(1, config.SfxPoolSize)];
            for (int i = 0; i < sfxPool.Length; i++)
                sfxPool[i] = CreateSource($"Sfx{i}", config.WorldSfxGroup, spatialBlend: 0f);

            // Single persistent listener (Unity only ever hears through one) — always enabled, so
            // 2D UI/music sounds (spatialBlend 0, listener-position-independent) are always audible;
            // moved to the selected camera by SetWorldListenerPosition, or parked out of world-sfx
            // range by ParkWorldListener when no camera is selected.
            var listenerGo = new GameObject("WorldListener");
            listenerGo.transform.SetParent(root.transform, worldPositionStays: false);
            listenerGo.transform.position = config.WorldListenerParkPosition;
            worldListener = listenerGo.AddComponent<AudioListener>();

            MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, config.DefaultMasterVolume);
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, config.DefaultMusicVolume);
            SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, config.DefaultSfxVolume);

            ApplyVolumeToMixer(config.MasterVolumeParam, MasterVolume);
            ApplyVolumeToMixer(config.MusicVolumeParam, MusicVolume);
            ApplyVolumeToMixer(config.SfxVolumeParam, SfxVolume);
        }

        public void Tick()
        {
            if (activeMusic == null && fadingOutMusic == null) return;

            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeDuration);

            if (activeMusic != null)
                activeMusic.volume = Mathf.Lerp(0f, currentCue != null ? currentCue.Volume : 1f, t);

            if (fadingOutMusic != null)
            {
                fadingOutMusic.volume = Mathf.Lerp(fadingOutStartVolume, 0f, t);
                if (t >= 1f)
                {
                    fadingOutMusic.Stop();
                    fadingOutMusic = null;
                }
            }
        }

        public void SetMasterVolume(float value01)
        {
            MasterVolume = Mathf.Clamp01(value01);
            PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
            PlayerPrefs.Save();
            ApplyVolumeToMixer(config.MasterVolumeParam, MasterVolume);
            VolumeChanged?.Invoke();
        }

        public void SetMusicVolume(float value01)
        {
            MusicVolume = Mathf.Clamp01(value01);
            PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
            PlayerPrefs.Save();
            ApplyVolumeToMixer(config.MusicVolumeParam, MusicVolume);
            VolumeChanged?.Invoke();
        }

        public void SetSfxVolume(float value01)
        {
            SfxVolume = Mathf.Clamp01(value01);
            PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
            PlayerPrefs.Save();
            ApplyVolumeToMixer(config.SfxVolumeParam, SfxVolume);
            VolumeChanged?.Invoke();
        }

        public void PlayMusic(MusicCue cue, float fadeSeconds = -1f)
        {
            if (cue == null || cue == currentCue) return;

            currentCue = cue;
            fadeDuration = Mathf.Max(fadeSeconds >= 0f ? fadeSeconds : config.DefaultMusicFadeSeconds, 0.0001f);
            fadeTimer = 0f;

            var incoming = activeMusic == musicA ? musicB : musicA;
            incoming.clip = cue.Clip;
            incoming.loop = cue.Loop;
            incoming.volume = 0f;
            incoming.Play();

            if (activeMusic != null && activeMusic.isPlaying)
            {
                fadingOutMusic = activeMusic;
                fadingOutStartVolume = activeMusic.volume;
            }

            activeMusic = incoming;
        }

        public void StopMusic(float fadeSeconds = -1f)
        {
            currentCue = null;
            fadeDuration = Mathf.Max(fadeSeconds >= 0f ? fadeSeconds : config.DefaultMusicFadeSeconds, 0.0001f);
            fadeTimer = 0f;

            if (activeMusic != null)
            {
                fadingOutMusic = activeMusic;
                fadingOutStartVolume = activeMusic.volume;
            }

            activeMusic = null;
        }

        public void PlayUiSfx(SfxCue cue)
        {
            if (cue == null) return;

            var source = GetPooledSource();
            source.outputAudioMixerGroup = config.UiSfxGroup;
            source.spatialBlend = 0f;
            source.clip = cue.GetClip();
            source.volume = cue.Volume;
            source.pitch = cue.GetPitch();
            source.Play();
        }

        public void PlaySfxAtPoint(SfxCue cue, Vector3 position)
        {
            if (cue == null) return;

            var source = GetPooledSource();
            source.outputAudioMixerGroup = config.WorldSfxGroup;
            source.transform.position = position;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = config.WorldSfxMinDistance;
            source.maxDistance = config.WorldSfxMaxDistance;
            source.clip = cue.GetClip();
            source.volume = cue.Volume;
            source.pitch = cue.GetPitch();
            source.Play();
        }

        // Entities are heard through the single persistent world listener, repositioned to the
        // selected camera by SetWorldListenerPosition (or parked out of range by ParkWorldListener
        // when no camera is selected) — so attached emitters (footsteps, growls, ...) are fully 3D
        // (spatialBlend 1, room-scale min/max distance from AudioConfig): the player hears them
        // positioned as if standing where that camera is, from anywhere in range; distance/rolloff
        // alone decide audibility, with no per-zone gate on top.
        public AudioSource CreateAttachedSource(Transform parent)
        {
            var go = new GameObject("AudioEmitterSource");
            go.transform.SetParent(parent, worldPositionStays: false);
            go.transform.localPosition = Vector3.zero;

            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = config.WorldSfxMinDistance;
            source.maxDistance = config.WorldSfxMaxDistance;
            source.outputAudioMixerGroup = config.WorldSfxGroup;
            return source;
        }

        public void SetWorldListenerPosition(Vector3 position, Quaternion rotation)
        {
            worldListener.transform.SetPositionAndRotation(position, rotation);
        }

        public void ParkWorldListener()
        {
            worldListener.transform.position = config.WorldListenerParkPosition;
        }

        public void Dispose()
        {
            if (root != null) UnityEngine.Object.Destroy(root);
        }

        private AudioSource GetPooledSource()
        {
            for (int i = 0; i < sfxPool.Length; i++)
            {
                int index = (sfxCursor + i) % sfxPool.Length;
                if (!sfxPool[index].isPlaying)
                {
                    sfxCursor = (index + 1) % sfxPool.Length;
                    return sfxPool[index];
                }
            }

            var stolen = sfxPool[sfxCursor];
            sfxCursor = (sfxCursor + 1) % sfxPool.Length;
            return stolen;
        }

        private AudioSource CreateSource(string sourceName, UnityEngine.Audio.AudioMixerGroup group, float spatialBlend)
        {
            var go = new GameObject(sourceName);
            go.transform.SetParent(root.transform, worldPositionStays: false);

            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = spatialBlend;
            source.outputAudioMixerGroup = group;
            return source;
        }

        private void ApplyVolumeToMixer(string param, float value01)
        {
            if (config.Mixer == null || string.IsNullOrEmpty(param)) return;

            float dB = value01 <= 0.0001f ? MuteDecibels : Mathf.Log10(value01) * 20f;
            config.Mixer.SetFloat(param, dB);
        }

    }
}
