using UnityEngine;
using UnityEngine.SceneManagement;
using ImpossibleLevels.Core;

namespace ImpossibleLevels.Levels
{
    public sealed class LevelCompletionRouter : MonoBehaviour
    {
        [SerializeField] private LevelRuntime levelRuntime;
        [SerializeField] private string gameplaySceneName = "Gameplay";
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private float completionDelay = 0.8f;

        private bool routed;

        private void Awake()
        {
            if (levelRuntime == null) levelRuntime = FindFirstObjectByType<LevelRuntime>();
        }

        private void OnEnable()
        {
            if (levelRuntime != null) levelRuntime.LevelCompleted += OnLevelCompleted;
        }

        private void OnDisable()
        {
            if (levelRuntime != null) levelRuntime.LevelCompleted -= OnLevelCompleted;
        }

        public void LoadNextLevel()
        {
            if (levelRuntime == null) return;
            routed = false;
            if (levelRuntime.LevelIndex >= 30)
            {
                ReturnToMenu();
                return;
            }

            PlayerPrefs.SetInt("il.selected_level", levelRuntime.LevelIndex + 1);
            PlayerPrefs.Save();
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void ReplayLevel()
        {
            if (levelRuntime == null) return;
            routed = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void ReturnToLevelMap()
        {
            LoadMainMenu("map");
        }

        public void OpenSettings()
        {
            LoadMainMenu("settings");
        }

        public void ReturnToMenu()
        {
            LoadMainMenu(string.Empty);
        }

        private void LoadMainMenu(string initialScreen)
        {
            routed = false;
            Time.timeScale = 1f;
            if (string.IsNullOrEmpty(initialScreen)) PlayerPrefs.DeleteKey("il.main_menu_screen");
            else PlayerPrefs.SetString("il.main_menu_screen", initialScreen);
            PlayerPrefs.Save();
            SceneManager.LoadScene(mainMenuSceneName);
        }

        private void OnLevelCompleted(int completedLevel)
        {
            if (routed) return;
            routed = true;

            // Progression is saved by the active level controller with the actual
            // star result and reward. This router only controls the post-completion flow.
            Invoke(nameof(EnableNextButtonRoute), completionDelay);
        }

        private void EnableNextButtonRoute()
        {
            routed = false;
        }
    }
}
