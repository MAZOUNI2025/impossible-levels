using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using ImpossibleLevels.Audio;
using ImpossibleLevels.Levels;

namespace ImpossibleLevels.UI
{
    public sealed class HookIntroController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup overlay;
        [SerializeField] private RectTransform primaryVisual;
        [SerializeField] private RectTransform secondaryVisual;
        [SerializeField] private TMP_Text message;
        [SerializeField] private float duration = 3.2f;

        private const string FirstLevelTutorialSeenKey = "il.tutorial.level1_seen";
        private int tutorialLevel = 1;
        private GameplayRule tutorialRule = GameplayRule.KeyDoor;
        private bool runtimeMode;

        public static bool ShouldShowFirstLevelTutorial => ShouldShowTutorial(1);

        public static bool ShouldShowTutorial(int levelId)
        {
            if (levelId < 1 || levelId > 5) return false;
            return PlayerPrefs.GetInt(TutorialSeenKey(levelId), 0) == 0;
        }

        public void ConfigureRuntime(CanvasGroup runtimeOverlay, RectTransform runtimePrimaryVisual,
            RectTransform runtimeSecondaryVisual, TMP_Text runtimeMessage, int levelId, GameplayRule rule)
        {
            overlay = runtimeOverlay;
            primaryVisual = runtimePrimaryVisual;
            secondaryVisual = runtimeSecondaryVisual;
            message = runtimeMessage;
            runtimeMode = true;
            tutorialLevel = Mathf.Clamp(levelId, 1, 5);
            tutorialRule = rule;
            duration = tutorialLevel == 1 ? 3.2f : 2.8f;
        }

        public static string GetTutorialBodyKey(GameplayRule rule)
        {
            return rule switch
            {
                GameplayRule.DragPlace => "TUTORIAL_DRAG_BODY",
                GameplayRule.SwitchState => "TUTORIAL_SWITCH_BODY",
                GameplayRule.RevealObservation => "TUTORIAL_REVEAL_BODY",
                GameplayRule.FairSequence => "TUTORIAL_SEQUENCE_BODY",
                _ => "TUTORIAL_BODY"
            };
        }

        public static string GetPrimaryVisualKey(GameplayRule rule)
        {
            return rule switch
            {
                GameplayRule.DragPlace => "block",
                GameplayRule.SwitchState => "switch",
                GameplayRule.RevealObservation => "reveal_trigger",
                GameplayRule.FairSequence => "star_filled",
                _ => "key"
            };
        }

        public static string GetSecondaryVisualKey(GameplayRule rule)
        {
            return rule == GameplayRule.RevealObservation ? "key" : "door";
        }

        public static string GetPrimaryLabelKey(GameplayRule rule)
        {
            return rule switch
            {
                GameplayRule.DragPlace => "TUTORIAL_BLOCK",
                GameplayRule.SwitchState => "TUTORIAL_SWITCH",
                GameplayRule.RevealObservation => "TUTORIAL_REVEAL",
                GameplayRule.FairSequence => "TUTORIAL_SEQUENCE",
                _ => "TUTORIAL_KEY"
            };
        }

        public static string GetSecondaryLabelKey(GameplayRule rule)
        {
            return rule == GameplayRule.RevealObservation ? "TUTORIAL_KEY" : "TUTORIAL_DOOR";
        }

        private static string TutorialSeenKey(int levelId)
        {
            return levelId == 1 ? FirstLevelTutorialSeenKey : "il.tutorial.level" + levelId + "_seen";
        }

        private IEnumerator Start()
        {
            if (overlay == null) yield break;
            if (!runtimeMode)
            {
                overlay.alpha = 1f;
                if (message != null)
                {
                    message.alignment = LocalizationService.IsArabic ? TextAlignmentOptions.Right : TextAlignmentOptions.Center;
                    message.text = LocalizationService.Get("HOOK_OPEN");
                    LocalizationService.ApplyTo(message);
                }
                yield return new WaitForSecondsRealtime(0.55f);
                if (message != null) message.text = LocalizationService.Get("HOOK_NOT_YET");
                yield return Pulse(secondaryVisual, 0.18f);
                if (primaryVisual != null) primaryVisual.localScale = Vector3.one * 1.12f;
                if (AudioDirector.Instance != null) AudioDirector.Instance.Invalid();
                yield return new WaitForSecondsRealtime(0.4f);
                if (message != null) message.text = LocalizationService.Get("HOOK_TITLE");
                if (AudioDirector.Instance != null) AudioDirector.Instance.KeyPickup();
                yield return new WaitForSecondsRealtime(Mathf.Max(0f, duration - 1.1f));
                yield return FadeOut();
                yield break;
            }
            if (!ShouldShowTutorial(tutorialLevel))
            {
                HideOverlay();
                yield break;
            }

            PlayerPrefs.SetInt(TutorialSeenKey(tutorialLevel), 1);
            PlayerPrefs.Save();
            overlay.alpha = 1f;
            overlay.interactable = false;
            overlay.blocksRaycasts = false;

            if (message != null)
            {
                message.alignment = LocalizationService.IsArabic ? TextAlignmentOptions.Right : TextAlignmentOptions.Center;
                message.text = LocalizationService.Get(GetTutorialBodyKey(tutorialRule));
                LocalizationService.ApplyTo(message);
            }

            yield return new WaitForSecondsRealtime(tutorialLevel == 1 ? 0.35f : 0.22f);
            yield return Pulse(primaryVisual, 0.18f);
            yield return new WaitForSecondsRealtime(tutorialLevel == 1 ? 0.35f : 0.28f);
            yield return Pulse(secondaryVisual, 0.18f);
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, duration - (tutorialLevel == 1 ? 0.95f : 0.80f)));
            yield return FadeOut();
        }

        private IEnumerator FadeOut()
        {
            var start = overlay.alpha;
            var elapsed = 0f;
            while (elapsed < 0.45f)
            {
                elapsed += Time.unscaledDeltaTime;
                overlay.alpha = Mathf.Lerp(start, 0f, elapsed / 0.45f);
                yield return null;
            }
            HideOverlay();
        }

        private void HideOverlay()
        {
            if (overlay == null) return;
            overlay.alpha = 0f;
            overlay.blocksRaycasts = false;
            overlay.interactable = false;
        }

        private static IEnumerator Pulse(RectTransform target, float amount)
        {
            if (target == null) yield break;
            var start = target.localScale;
            target.localScale = start * (1f + amount);
            yield return new WaitForSecondsRealtime(0.08f);
            target.localScale = start;
        }
    }

    public sealed class MotionFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ICancelHandler
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private float pressScale = 0.94f;
        [SerializeField] private float duration = 0.08f;

        private Vector3 baseScale;

        private void Awake()
        {
            if (target == null) target = transform as RectTransform;
            if (target != null) baseScale = target.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SetPressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Release();
        }

        public void OnCancel(BaseEventData eventData)
        {
            Release();
        }

        public void Press()
        {
            if (target == null) return;
            StopAllCoroutines();
            StartCoroutine(PressRoutine());
            if (AudioDirector.Instance != null) AudioDirector.Instance.Tap();
            HapticsFeedback.TryPulse();
        }

        private void SetPressed()
        {
            if (target == null) return;
            StopAllCoroutines();
            target.localScale = baseScale * pressScale;
        }

        private void Release()
        {
            if (target == null) return;
            StopAllCoroutines();
            StartCoroutine(ReleaseRoutine());
        }

        private IEnumerator PressRoutine()
        {
            SetPressed();
            yield return new WaitForSecondsRealtime(duration);
            target.localScale = baseScale;
        }

        private IEnumerator ReleaseRoutine()
        {
            yield return new WaitForSecondsRealtime(duration);
            if (target != null) target.localScale = baseScale;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            if (target != null) target.localScale = baseScale;
        }
    }
}
