using UnityEngine;
using UnityEngine.SceneManagement;
using ImpossibleLevels.Core;

namespace ImpossibleLevels.UI
{
    public sealed class SettingsController : MonoBehaviour
    {
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        private PlayerProfileService profile;

        private void Awake()
        {
            profile = FindFirstObjectByType<PlayerProfileService>();
        }

        public void SetMusic(bool enabled)
        {
            if (profile != null) profile.SetMusicEnabled(enabled);
        }

        public void SetSfx(bool enabled)
        {
            if (profile != null) profile.SetSfxEnabled(enabled);
        }

        public void SetHaptics(bool enabled)
        {
            if (profile != null) profile.SetHapticsEnabled(enabled);
        }

        public void ResetProgress()
        {
            var selectedLanguage = LocalizationService.CurrentLanguage;
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("il.language", selectedLanguage);
            PlayerPrefs.SetInt("il.highest_unlocked", 1);
            PlayerPrefs.SetInt("il.music", 1);
            PlayerPrefs.SetInt("il.sfx", 1);
            PlayerPrefs.SetInt("il.haptics", 1);
            PlayerPrefs.Save();
            if (profile != null) profile.RefreshTotals(30);
        }

        public void Close()
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    public sealed class LevelMapController : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName = "Gameplay";
        [SerializeField] private ProgressionService progression;

        private void Awake()
        {
            progression = progression != null ? progression : FindFirstObjectByType<ProgressionService>();
        }

        public bool IsLevelUnlocked(int levelIndex)
        {
            return progression != null && progression.IsUnlocked(levelIndex);
        }

        public int GetLevelStars(int levelIndex)
        {
            return progression == null ? 0 : progression.GetStars(levelIndex);
        }

        public void SelectLevel(int levelIndex)
        {
            if (!IsLevelUnlocked(levelIndex)) return;
            PlayerPrefs.SetInt("il.selected_level", Mathf.Clamp(levelIndex, 1, 30));
            PlayerPrefs.Save();
            SceneManager.LoadScene(gameplaySceneName);
        }
    }
}
