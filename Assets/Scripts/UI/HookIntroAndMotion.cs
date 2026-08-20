using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ImpossibleLevels.Audio;

namespace ImpossibleLevels.UI
{
    public sealed class HookIntroController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup overlay;
        [SerializeField] private RectTransform keyVisual;
        [SerializeField] private RectTransform doorVisual;
        [SerializeField] private Text message;
        [SerializeField] private float duration = 3.2f;

        private IEnumerator Start()
        {
            if (overlay == null) yield break;
            overlay.alpha = 1f;
            if (message != null) message.text = "Open the door.";
            yield return new WaitForSecondsRealtime(0.55f);
            if (message != null) message.text = "Not yet.";
            yield return Pulse(doorVisual, 0.18f);
            if (keyVisual != null) keyVisual.localScale = Vector3.one * 1.12f;
            if (AudioDirector.Instance != null) AudioDirector.Instance.Invalid();
            yield return new WaitForSecondsRealtime(0.4f);
            if (message != null) message.text = "Looks Easy. Think Again.";
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

    public sealed class MotionFeedback : MonoBehaviour
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

        public void Press()
        {
            if (target == null) return;
            StopAllCoroutines();
            StartCoroutine(PressRoutine());
            if (AudioDirector.Instance != null) AudioDirector.Instance.Tap();
        }

        private IEnumerator PressRoutine()
        {
            target.localScale = baseScale * pressScale;
            yield return new WaitForSecondsRealtime(duration);
            target.localScale = baseScale;
        }
    }
}
