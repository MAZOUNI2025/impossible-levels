using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ImpossibleLevels.Core;
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
        [SerializeField] private TMP_Text starsFallback;
        [SerializeField] private TMP_Text completionStatsLabel;
        [SerializeField] private TMP_Text completionCoinsLabel;

        [Header("Star Visuals")]
        [SerializeField] private Image[] starVisuals;

        [Header("Buttons")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button hintButton;

        [Header("Runtime")]
        [SerializeField] private LevelRuntime levelRuntime;

        public void Configure(LevelRuntime runtime, GameObject pause, GameObject success, GameObject fail,
            TMP_Text objective, TMP_Text level, TMP_Text hint, TMP_Text coins,
            Button pauseControl, Button retryControl, Button continueControl, Button hintControl,
            Image[] stars, TMP_Text starFallback, TMP_Text completionStats, TMP_Text completionCoins)
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
            starVisuals = stars;
            starsFallback = starFallback;
            completionStatsLabel = completionStats;
            completionCoinsLabel = completionCoins;
            BindButtonListeners();
            BindRuntimeEvents();
        }

        private void Awake()
        {
            BindButtonListeners();
        }

        private void OnEnable()
        {
            if (levelRuntime == null) levelRuntime = FindFirstObjectByType<LevelRuntime>();
            BindRuntimeEvents();
            SetPanel(pausePanel, false);
            SetPanel(successPanel, false);
            SetPanel(failPanel, false);
            SetPanel(hintLabel != null ? hintLabel.gameObject : null, false);
            RefreshStars();
        }

        private void OnDisable()
        {
            if (levelRuntime != null) levelRuntime.StateChanged -= OnStateChanged;
        }

        public void SetObjective(string objective, int levelIndex)
        {
            if (objectiveLabel != null) objectiveLabel.text = objective;
            if (levelLabel != null) levelLabel.text = $"LEVEL {levelIndex:00}";
            RefreshStars();
        }

        public void SetCoins(int coins)
        {
            if (coinLabel != null) coinLabel.text = Mathf.Max(0, coins).ToString();
        }

        public void SetHint(string hint)
        {
            if (hintLabel != null) hintLabel.text = hint;
            SetPanel(hintLabel != null ? hintLabel.gameObject : null, true);
            RefreshCoinsFromProgression();
        }

        private void OnStateChanged(LevelState state)
        {
            SetPanel(pausePanel, state == LevelState.Paused);
            SetPanel(successPanel, state == LevelState.Completed);
            SetPanel(failPanel, state == LevelState.Failed);
            if (state == LevelState.Completed) UpdateCompletionSummary();
        }

        private void OnPausePressed()
        {
            if (levelRuntime != null) levelRuntime.TogglePause();
        }

        private void OnRetryPressed()
        {
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

        private void BindButtonListeners()
        {
            BindButton(pauseButton, OnPausePressed);
            BindButton(retryButton, OnRetryPressed);
            BindButton(continueButton, OnContinuePressed);
            BindButton(hintButton, OnHintPressed);
        }

        private void BindRuntimeEvents()
        {
            if (levelRuntime == null) return;
            levelRuntime.StateChanged -= OnStateChanged;
            levelRuntime.StateChanged += OnStateChanged;
        }

        private static void BindButton(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null) return;
            button.onClick.RemoveListener(action);
            button.onClick.AddListener(action);
        }

        private void RefreshCoinsFromProgression()
        {
            var progression = FindFirstObjectByType<ProgressionService>();
            if (progression != null) SetCoins(progression.Coins);
        }

        private void RefreshStars()
        {
            var stars = 0;
            if (levelRuntime != null)
            {
                var progression = FindFirstObjectByType<ProgressionService>();
                if (progression != null) stars = progression.GetStars(levelRuntime.LevelIndex);
            }

            var filled = ArtAssetLibrary.GetGameplaySprite("star_filled");
            var empty = ArtAssetLibrary.GetGameplaySprite("star_empty");
            var hasVisualStars = filled != null && empty != null && starVisuals != null && starVisuals.Length >= 3;
            if (hasVisualStars)
            {
                for (var i = 0; i < 3; i++)
                {
                    starVisuals[i].sprite = i < stars ? filled : empty;
                    starVisuals[i].color = Color.white;
                    starVisuals[i].gameObject.SetActive(true);
                }
            }
            else if (starVisuals != null)
            {
                for (var i = 0; i < starVisuals.Length; i++)
                {
                    if (starVisuals[i] != null) starVisuals[i].gameObject.SetActive(false);
                }
            }

            if (starsFallback != null)
            {
                starsFallback.text = StarString(stars);
                starsFallback.gameObject.SetActive(!hasVisualStars);
            }
        }

        private void UpdateCompletionSummary()
        {
            var stars = 0;
            var reward = 0;
            if (levelRuntime != null)
            {
                var progression = FindFirstObjectByType<ProgressionService>();
                if (progression != null) stars = progression.GetStars(levelRuntime.LevelIndex);
                reward = levelRuntime.CalculateCoinReward(stars);
            }

            if (completionStatsLabel != null) completionStatsLabel.text = $"STARS EARNED  {stars} / 3";
            if (completionCoinsLabel != null) completionCoinsLabel.text = $"COINS EARNED  +{reward}";
            RefreshStars();
            RefreshCoinsFromProgression();
        }

        private static string StarString(int stars)
        {
            stars = Mathf.Clamp(stars, 0, 3);
            return new string('★', stars) + new string('☆', 3 - stars);
        }

        private static void SetPanel(GameObject panel, bool visible)
        {
            if (panel != null) panel.SetActive(visible);
        }
    }
}
