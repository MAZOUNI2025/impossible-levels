using UnityEngine;
using ImpossibleLevels.Core;

namespace ImpossibleLevels.Audio
{
    public sealed class AudioDirector : MonoBehaviour
    {
        public static AudioDirector Instance { get; private set; }

        [Header("Music")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip menuLoop;
        [SerializeField] private AudioClip gameplayLoop;

        [Header("SFX")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip uiTap;
        [SerializeField] private AudioClip invalid;
        [SerializeField] private AudioClip keyPickup;
        [SerializeField] private AudioClip doorUnlock;
        [SerializeField] private AudioClip hint;
        [SerializeField] private AudioClip success;
        [SerializeField] private AudioClip failure;
        [SerializeField] private AudioClip pause;

        private PlayerProfileService profile;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            profile = FindFirstObjectByType<PlayerProfileService>();
            EnsureAudioSources();
            LoadResources();
        }

        private void EnsureAudioSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.playOnAwake = false;
                musicSource.loop = true;
                musicSource.volume = 0.42f;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.loop = false;
                sfxSource.volume = 0.72f;
            }
        }

        private void LoadResources()
        {
            menuLoop = Resources.Load<AudioClip>("Audio/Music/menu_loop");
            gameplayLoop = Resources.Load<AudioClip>("Audio/Music/gameplay_loop");
            uiTap = Resources.Load<AudioClip>("Audio/SFX/ui_tap");
            invalid = Resources.Load<AudioClip>("Audio/SFX/ui_invalid");
            keyPickup = Resources.Load<AudioClip>("Audio/SFX/key_pickup");
            doorUnlock = Resources.Load<AudioClip>("Audio/SFX/door_unlock");
            hint = Resources.Load<AudioClip>("Audio/SFX/hint");
            success = Resources.Load<AudioClip>("Audio/SFX/success");
            failure = Resources.Load<AudioClip>("Audio/SFX/failure");
            pause = Resources.Load<AudioClip>("Audio/SFX/pause");
        }

        public void PlayMenuMusic()
        {
            PlayMusic(menuLoop);
        }

        public void PlayGameplayMusic()
        {
            PlayMusic(gameplayLoop);
        }

        public void Tap() => PlaySfx(uiTap);
        public void Invalid() => PlaySfx(invalid);
        public void KeyPickup() => PlaySfx(keyPickup);
        public void DoorUnlock() => PlaySfx(doorUnlock);
        public void Hint() => PlaySfx(hint);
        public void Success() => PlaySfx(success);
        public void Failure() => PlaySfx(failure);
        public void Pause() => PlaySfx(pause);

        // Presentation-only variants reuse the curated SFX bank. The user's SFX
        // preference remains authoritative while each board action has a physical origin.
        public void TapAt(Vector3 worldPosition, float pitch = 1f) => PlaySpatialSfx(uiTap, worldPosition, pitch);
        public void InvalidAt(Vector3 worldPosition, float pitch = 1f) => PlaySpatialSfx(invalid, worldPosition, pitch);
        public void KeyPickupAt(Vector3 worldPosition, float pitch = 1f) => PlaySpatialSfx(keyPickup, worldPosition, pitch);
        public void DoorUnlockAt(Vector3 worldPosition, float pitch = 1f) => PlaySpatialSfx(doorUnlock, worldPosition, pitch);
        public void HintAt(Vector3 worldPosition, float pitch = 1f) => PlaySpatialSfx(hint, worldPosition, pitch);
        public void SuccessAt(Vector3 worldPosition, float pitch = 1f) => PlaySpatialSfx(success, worldPosition, pitch);

        private void PlayMusic(AudioClip clip)
        {
            if (musicSource == null || clip == null) return;
            if (!IsMusicEnabled())
            {
                musicSource.Stop();
                return;
            }

            if (musicSource.clip == clip && musicSource.isPlaying) return;
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        private void PlaySfx(AudioClip clip)
        {
            if (sfxSource == null || clip == null || !IsSfxEnabled()) return;
            sfxSource.PlayOneShot(clip);
        }

        private void PlaySpatialSfx(AudioClip clip, Vector3 worldPosition, float pitch)
        {
            if (clip == null || !IsSfxEnabled()) return;

            var effectObject = new GameObject("SpatialSfx");
            effectObject.transform.position = worldPosition;
            var source = effectObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 1.25f;
            source.maxDistance = 14f;
            source.dopplerLevel = 0f;
            source.volume = 0.72f;
            source.pitch = Mathf.Clamp(pitch, 0.55f, 1.55f);
            source.Play();
            Destroy(effectObject, clip.length / source.pitch + 0.12f);
        }

        private bool IsMusicEnabled()
        {
            return profile == null || profile.MusicEnabled;
        }

        private bool IsSfxEnabled()
        {
            return profile == null || profile.SfxEnabled;
        }
    }
}
