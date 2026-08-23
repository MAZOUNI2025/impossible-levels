using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using ImpossibleLevels.Audio;

namespace ImpossibleLevels.UI
{
    public sealed class HookIntroController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup overlay;
        [SerializeField] private RectTransform keyVisual;
        [SerializeField] private RectTransform doorVisual;
        [SerializeField] private TMP_Text message;
        [SerializeField] private float duration = 3.2f;

        private const string FirstLevelTutorialSeenKey = "il.tutorial.level1_seen";
        private bool firstLevelRuntimeMode;

        public static bool ShouldShowFirstLevelTutorial => PlayerPrefs.GetInt(FirstLevelTutorialSeenKey, 0) == 0;

        public void ConfigureRuntime(CanvasGroup runtimeOverlay, RectTransform runtimeKeyVisual, RectTransform runtimeDoorVisual, TMP_Text runtimeMessage)
        {
            overlay = runtimeOverlay;
            keyVisual = runtimeKeyVisual;
            doorVisual = runtimeDoorVisual;
            message = runtimeMessage;
            firstLevelRuntimeMode = true;
            duration = 3.2f;
        }

        private IEnumerator Start()
        {
            if (overlay == null) yield break;
            if (firstLevelRuntimeMode && !ShouldShowFirstLevelTutorial)
            {
                HideOverlay();
                yield break;
            }

            if (firstLevelRuntimeMode)
            {
                PlayerPrefs.SetInt(FirstLevelTutorialSeenKey, 1);
                PlayerPrefs.Save();
            }

            overlay.alpha = 1f;
            if (message != null)
            {
                message.alignment = LocalizationService.IsArabic ? TextAlignmentOptions.Right : TextAlignmentOptions.Center;
                LocalizationService.ApplyTo(message);
                message.text = firstLevelRuntimeMode
                    ? LocalizationService.Get("TUTORIAL_BODY")
                    : LocalizationService.Get("HOOK_OPEN");
            }

            if (firstLevelRuntimeMode)
            {
                yield return new WaitForSecondsRealtime(0.35f);
                yield return Pulse(keyVisual, 0.18f);
                yield return new WaitForSecondsRealtime(0.35f);
                yield return Pulse(doorVisual, 0.18f);
                yield return new WaitForSecondsRealtime(Mathf.Max(0f, duration - 0.95f));
                yield return FadeOut();
                yield break;
            }

            yield return new WaitForSecondsRealtime(0.55f);
            if (message != null) message.text = LocalizationService.Get("HOOK_NOT_YET");
            yield return Pulse(doorVisual, 0.18f);
            if (keyVisual != null) keyVisual.localScale = Vector3.one * 1.12f;
            if (AudioDirector.Instance != null) AudioDirector.Instance.Invalid();
            yield return new WaitForSecondsRealtime(0.4f);
            if (message != null) message.text = LocalizationService.Get("HOOK_TITLE");
            if (AudioDirector.Instance != null) AudioDirector.Instance.KeyPickup();
            yield return new WaitForSecondsRealtime(duration - 1.1f);
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
