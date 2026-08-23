using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ImpossibleLevels.Core;
using ImpossibleLevels.Levels;
using ImpossibleLevels.Audio;

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
        [SerializeField] private TMP_Text completionBestStarsLabel;
        [SerializeField] private TMP_Text completionCoinsLabel;
        [SerializeField] private TMP_Text completionTotalCoinsLabel;
        [SerializeField] private TMP_Text completionProgressLabel;

        [Header("Star Visuals")]
        [SerializeField] private Image[] starVisuals;

        [Header("Buttons")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button hintButton;

        [Header("Runtime")]
        [SerializeField] private LevelRuntime levelRuntime;

        private ProceduralPuzzleBoard puzzleBoard;
        private bool hasCurrentRunSummary;
        private int currentRunStars;
        private int currentRunReward;

        private CanvasGroup pauseCanvasGroup;
        private CanvasGroup successCanvasGroup;
        private CanvasGroup failCanvasGroup;
        private Coroutine pauseTransition;
        private Coroutine successTransition;
        private Coroutine failTransition;
        private Coroutine starsReveal;
        private Coroutine coinPulse;
        private Coroutine hintPulse;
        private int lastDisplayedCoins = -1;
        private string levelHint;

        public void Configure(LevelRuntime runtime, GameObject pause, GameObject success, GameObject fail,
            TMP_Text objective, TMP_Text level, TMP_Text hint, TMP_Text coins,
            Button pauseControl, Button retryControl, Button continueControl, Button hintControl,
            Image[] stars, TMP_Text starFallback, TMP_Text completionStats, TMP_Text completionBestStars,
            TMP_Text completionCoins, TMP_Text completionTotalCoins, TMP_Text completionProgress)
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
            completionBestStarsLabel = completionBestStars;
            completionCoinsLabel = completionCoins;
            completionTotalCoinsLabel = completionTotalCoins;
            completionProgressLabel = completionProgress;
            BindButtonListeners();
            BindRuntimeEvents();
            BindPuzzleBoardEvents();
        }

        private void Awake()
        {
            BindButtonListeners();
        }

        private void OnEnable()
        {
            if (levelRuntime == null) levelRuntime = FindFirstObjectByType<LevelRuntime>();
            BindRuntimeEvents();
            BindPuzzleBoardEvents();
            SetPanelImmediate(pausePanel, ref pauseCanvasGroup, false);
            SetPanelImmediate(successPanel, ref successCanvasGroup, false);
            SetPanelImmediate(failPanel, ref failCanvasGroup, false);
            SetPanel(hintLabel != null ? hintLabel.gameObject : null, false);
            RefreshStars();
        }

        private void OnDisable()
        {
            if (levelRuntime != null) levelRuntime.StateChanged -= OnStateChanged;
            if (puzzleBoard != null) puzzleBoard.CompletionSummaryReady -= OnCompletionSummaryReady;
            StopFeedbackCoroutines();
        }

        public void SetObjective(string objective, int levelIndex)
        {
            if (objectiveLabel != null) objectiveLabel.text = LocalizationService.GetLevelObjective(levelIndex, objective);
            if (levelLabel != null) levelLabel.text = LocalizationService.Format("GAME_LEVEL", levelIndex);
            RefreshStars();
        }

        public void SetLevelHint(string hint)
        {
            levelHint = hint;
        }

        public void SetCoins(int coins)
        {
            var safeCoins = Mathf.Max(0, coins);
            if (coinLabel != null) coinLabel.text = safeCoins.ToString();
            if (lastDisplayedCoins >= 0 && lastDisplayedCoins != safeCoins && coinLabel != null)
            {
                PulseTransform(coinLabel.rectTransform, ref coinPulse, 1.08f, 0.12f);
            }
            lastDisplayedCoins = safeCoins;
        }

        public void SetHint(string hint)
        {
            if (hintLabel != null)
            {
                hintLabel.text = LocalizationService.GetLevelHint(levelRuntime != null ? levelRuntime.LevelIndex : 1, hint);
                hintLabel.color = new Color(0.10f, 0.95f, 0.82f);
                PulseTransform(hintLabel.rectTransform, ref hintPulse, 1.03f, 0.10f);
            }
            SetPanel(hintLabel != null ? hintLabel.gameObject : null, true);
            RefreshCoinsFromProgression();
        }

        private void OnStateChanged(LevelState state)
        {
            if (state == LevelState.Paused) ShowPanel(pausePanel, ref pauseCanvasGroup, ref pauseTransition);
            else HidePanel(pausePanel, ref pauseCanvasGroup, ref pauseTransition);

            if (state == LevelState.Completed)
            {
                UpdateCompletionSummary();
                ShowPanel(successPanel, ref successCanvasGroup, ref successTransition);
                RevealStars();
            }
            else HidePanel(successPanel, ref successCanvasGroup, ref successTransition);

            if (state == LevelState.Failed) ShowPanel(failPanel, ref failCanvasGroup, ref failTransition);
            else HidePanel(failPanel, ref failCanvasGroup, ref failTransition);
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
                SetHint(levelHint ?? LocalizationService.Get("GAME_HINT"));
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

        private void BindPuzzleBoardEvents()
        {
            if (puzzleBoard == null) puzzleBoard = FindFirstObjectByType<ProceduralPuzzleBoard>();
            if (puzzleBoard == null) return;
            puzzleBoard.CompletionSummaryReady -= OnCompletionSummaryReady;
            puzzleBoard.CompletionSummaryReady += OnCompletionSummaryReady;
        }

        private void OnCompletionSummaryReady(int levelIndex, int stars, int reward)
        {
            if (levelRuntime != null && levelIndex != levelRuntime.LevelIndex) return;
            hasCurrentRunSummary = true;
            currentRunStars = Mathf.Clamp(stars, 0, 3);
            currentRunReward = Mathf.Max(0, reward);
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
            var progression = FindFirstObjectByType<ProgressionService>();
            var profile = FindFirstObjectByType<PlayerProfileService>();
            if (profile != null) profile.RefreshTotals(30);
            var bestStars = 0;
            if (progression != null && levelRuntime != null) bestStars = Mathf.Clamp(progression.GetStars(levelRuntime.LevelIndex), 0, 3);

            if (completionStatsLabel != null)
            {
                completionStatsLabel.text = hasCurrentRunSummary
                    ? LocalizationService.Format("GAME_STARS_THIS_RUN", currentRunStars)
                    : LocalizationService.Get("GAME_STARS_THIS_RUN_UNAVAILABLE");
            }
            if (completionBestStarsLabel != null)
            {
                completionBestStarsLabel.text = LocalizationService.Format("GAME_BEST_STARS", bestStars);
            }
            if (completionCoinsLabel != null)
            {
                completionCoinsLabel.text = hasCurrentRunSummary
                    ? LocalizationService.Format("GAME_COINS_THIS_COMPLETION", currentRunReward)
                    : LocalizationService.Get("GAME_COINS_THIS_COMPLETION_UNAVAILABLE");
            }
            if (completionTotalCoinsLabel != null)
            {
                var totalCoins = progression != null ? progression.Coins : 0;
                completionTotalCoinsLabel.text = LocalizationService.Format("GAME_COINS_TOTAL", totalCoins);
            }
            if (completionProgressLabel != null)
            {
                var completed = profile != null ? profile.CompletedLevels : 0;
                var totalStars = profile != null ? profile.TotalStars : 0;
                completionProgressLabel.text = LocalizationService.Format("GAME_PROGRESS_SUMMARY", completed, 30, totalStars, 90);
            }
            RefreshStars();
            RefreshCoinsFromProgression();
        }

        private void RevealStars()
        {
            if (starVisuals == null || starVisuals.Length < 3) return;
            if (starsReveal != null) StopCoroutine(starsReveal);
            starsReveal = StartCoroutine(RevealStarsRoutine());
        }

        private IEnumerator RevealStarsRoutine()
        {
            for (var i = 0; i < 3; i++)
            {
                if (starVisuals[i] == null) continue;
                var original = starVisuals[i].color;
                starVisuals[i].color = new Color(original.r, original.g, original.b, 0f);
                yield return new WaitForSecondsRealtime(0.07f);
                starVisuals[i].color = original;
                yield return new WaitForSecondsRealtime(0.05f);
            }
            starsReveal = null;
        }

        private void ShowPanel(GameObject panel, ref CanvasGroup group, ref Coroutine routine)
        {
            if (panel == null) return;
            EnsureCanvasGroup(panel, ref group);
            if (routine != null) StopCoroutine(routine);
            panel.SetActive(true);
            routine = StartCoroutine(PanelInRoutine(panel.transform as RectTransform, group));
        }

        private void HidePanel(GameObject panel, ref CanvasGroup group, ref Coroutine routine)
        {
            if (panel == null) return;
            if (routine != null) StopCoroutine(routine);
            routine = null;
            if (group == null) EnsureCanvasGroup(panel, ref group);
            group.alpha = 1f;
            var rect = panel.transform as RectTransform;
            if (rect != null) rect.localScale = Vector3.one;
            panel.SetActive(false);
        }

        private IEnumerator PanelInRoutine(RectTransform rect, CanvasGroup group)
        {
            if (rect == null || group == null) yield break;
            rect.localScale = Vector3.one * 0.95f;
            group.alpha = 0f;
            var elapsed = 0f;
            while (elapsed < 0.16f)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / 0.16f);
                group.alpha = t;
                rect.localScale = Vector3.LerpUnclamped(Vector3.one * 0.95f, Vector3.one, t);
                yield return null;
            }
            group.alpha = 1f;
            rect.localScale = Vector3.one;
        }

        private static void SetPanelImmediate(GameObject panel, ref CanvasGroup group, bool visible)
        {
            if (panel == null) return;
            if (group == null) group = panel.GetComponent<CanvasGroup>() ?? panel.AddComponent<CanvasGroup>();
            group.alpha = visible ? 1f : 0f;
            panel.SetActive(visible);
        }

        private static void EnsureCanvasGroup(GameObject panel, ref CanvasGroup group)
        {
            if (group == null) group = panel.GetComponent<CanvasGroup>() ?? panel.AddComponent<CanvasGroup>();
        }

        private void PulseTransform(RectTransform target, ref Coroutine routine, float peakScale, float duration)
        {
            if (target == null) return;
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(PulseRoutine(target, peakScale, duration));
        }

        private IEnumerator PulseRoutine(RectTransform target, float peakScale, float duration)
        {
            var baseScale = target.localScale;
            target.localScale = baseScale * peakScale;
            yield return new WaitForSecondsRealtime(duration);
            if (target != null) target.localScale = baseScale;
        }

        private void StopFeedbackCoroutines()
        {
            if (pauseTransition != null) StopCoroutine(pauseTransition);
            if (successTransition != null) StopCoroutine(successTransition);
            if (failTransition != null) StopCoroutine(failTransition);
            if (starsReveal != null) StopCoroutine(starsReveal);
            if (coinPulse != null) StopCoroutine(coinPulse);
            if (hintPulse != null) StopCoroutine(hintPulse);
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
