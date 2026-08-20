using UnityEngine;

namespace ImpossibleLevels.Core
{
    public sealed class PlayerProfileService : MonoBehaviour
    {
        public static PlayerProfileService Instance { get; private set; }

        public bool MusicEnabled { get; private set; }
        public bool SfxEnabled { get; private set; }
        public bool HapticsEnabled { get; private set; }
        public int TotalStars { get; private set; }
        public int CompletedLevels { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        public void SetMusicEnabled(bool value)
        {
            MusicEnabled = value;
            PlayerPrefs.SetInt("il.music", value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetSfxEnabled(bool value)
        {
            SfxEnabled = value;
            PlayerPrefs.SetInt("il.sfx", value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetHapticsEnabled(bool value)
        {
            HapticsEnabled = value;
            PlayerPrefs.SetInt("il.haptics", value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void RefreshTotals(int totalLevels)
        {
            var stars = 0;
            var completed = 0;
            for (var i = 1; i <= totalLevels; i++)
            {
                var levelStars = PlayerPrefs.GetInt("il.level_stars." + i, 0);
                stars += levelStars;
                if (levelStars > 0) completed++;
            }

            TotalStars = stars;
            CompletedLevels = completed;
        }

        public void ResetProfile()
        {
            SetMusicEnabled(true);
            SetSfxEnabled(true);
            SetHapticsEnabled(true);
            RefreshTotals(30);
        }

        private void Load()
        {
            MusicEnabled = PlayerPrefs.GetInt("il.music", 1) == 1;
            SfxEnabled = PlayerPrefs.GetInt("il.sfx", 1) == 1;
            HapticsEnabled = PlayerPrefs.GetInt("il.haptics", 1) == 1;
            RefreshTotals(30);
        }
    }
}
