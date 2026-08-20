using UnityEngine;
using UnityEngine.SceneManagement;
using ImpossibleLevels.Core;

namespace ImpossibleLevels.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName = "Gameplay";
        [SerializeField] private ProgressionService progressionService;

        private void Awake()
        {
            progressionService = progressionService != null
                ? progressionService
                : FindFirstObjectByType<ProgressionService>();
        }

        public void StartFirstLevel()
        {
            LoadLevel(1);
        }

        public void LoadLevel(int levelIndex)
        {
            if (progressionService != null && !progressionService.IsUnlocked(levelIndex))
            {
                return;
            }

            PlayerPrefs.SetInt("il.selected_level", Mathf.Max(1, levelIndex));
            PlayerPrefs.Save();
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
