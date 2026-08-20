using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ImpossibleLevels.Levels;

namespace ImpossibleLevels.UI
{
    public sealed class GameHudController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject successPanel;
        [SerializeField] private GameObject failPanel;

        [Header("Labels")]
        [SerializeField] private TMP_Text objectiveLabel;
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private TMP_Text hintLabel;
        [SerializeField] private TMP_Text coinLabel;

        [Header("Buttons")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button hintButton;

        [Header("Runtime")]
        [SerializeField] private LevelRuntime levelRuntime;

        private int hintsUsed;
        private float levelStartedAt;

        public void Configure(LevelRuntime runtime, GameObject pause, GameObject success, GameObject fail,
            TMP_Text objective, TMP_Text level, TMP_Text hint, TMP_Text coins,
            Button pauseControl, Button retryControl, Button continueControl, Button hintControl)
        {
            levelRuntime = runtime;
            pausePanel = pause;
            successPanel = success;
            failPanel = fail;
            objectiveLabel = objective;
            levelLabel = level;
            hintLabel = hint;
            coinLabel = coins;
            pauseButton = pauseControl;
            retryButton = retryControl;
            continueButton = continueControl;
            hintButton = hintControl;
            if (pauseButton != null) pauseButton.onClick.AddListener(OnPausePressed);
            if (retryButton != null) retryButton.onClick.AddListener(OnRetryPressed);
            if (continueButton != null) continueButton.onClick.AddListener(OnContinuePressed);
            if (hintButton != null) hintButton.onClick.AddListener(OnHintPressed);
            if (levelRuntime != null) levelRuntime.StateChanged += OnStateChanged;
        }

        private void Awake()
        {
            if (pauseButton != null) pauseButton.onClick.AddListener(OnPausePressed);
            if (retryButton != null) retryButton.onClick.AddListener(OnRetryPressed);
            if (continueButton != null) continueButton.onClick.AddListener(OnContinuePressed);
            if (hintButton != null) hintButton.onClick.AddListener(OnHintPressed);
        }

        private void OnEnable()
        {
            if (levelRuntime == null) levelRuntime = FindFirstObjectByType<LevelRuntime>();
            if (levelRuntime != null) levelRuntime.StateChanged += OnStateChanged;
            levelStartedAt = Time.unscaledTime;
            SetPanel(pausePanel, false);
            SetPanel(successPanel, false);
            SetPanel(failPanel, false);
        }

        private void OnDisable()
        {
            if (levelRuntime != null) levelRuntime.StateChanged -= OnStateChanged;
        }

        public void SetObjective(string objective, int levelIndex)
        {
            if (objectiveLabel != null) objectiveLabel.text = objective;
            if (levelLabel != null) levelLabel.text = $"LEVEL {levelIndex:00}";
        }

        public void SetCoins(int coins)
        {
            if (coinLabel != null) coinLabel.text = coins.ToString();
        }

        public void SetHint(string hint)
        {
            if (hintLabel != null) hintLabel.text = hint;
            hintsUsed++;
            SetPanel(hintLabel != null ? hintLabel.gameObject : null, true);
        }

        private void OnStateChanged(LevelState state)
        {
            SetPanel(pausePanel, state == LevelState.Paused);
            SetPanel(successPanel, state == LevelState.Completed);
            SetPanel(failPanel, state == LevelState.Failed);
        }

        private void OnPausePressed()
        {
            if (levelRuntime != null) levelRuntime.TogglePause();
        }

        private void OnRetryPressed()
        {
            hintsUsed = 0;
            levelStartedAt = Time.unscaledTime;
            if (levelRuntime != null) levelRuntime.RetryLevel();
        }

        private void OnContinuePressed()
        {
            if (levelRuntime != null && levelRuntime.State == LevelState.Failed)
            {
                levelRuntime.BeginLevel();
            }
        }

        private void OnHintPressed()
        {
            if (hintLabel != null)
            {
                SetHint("Look at the object that does not behave as expected.");
            }
        }

        private static void SetPanel(GameObject panel, bool visible)
        {
            if (panel != null) panel.SetActive(visible);
        }
    }
}
