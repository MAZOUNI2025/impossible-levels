using System;
using UnityEngine;

namespace ImpossibleLevels.Core
{
    public readonly struct PointerSample
    {
        public readonly Vector2 ScreenPosition;
        public readonly int PointerId;
        public readonly bool IsTouch;
        public readonly bool IsCanceled;

        public PointerSample(Vector2 screenPosition, int pointerId, bool isTouch, bool isCanceled = false)
        {
            ScreenPosition = screenPosition;
            PointerId = pointerId;
            IsTouch = isTouch;
            IsCanceled = isCanceled;
        }
    }

    public sealed class TouchInputRouter : MonoBehaviour
    {
        public event Action<PointerSample> PointerPressed;
        public event Action<PointerSample> PointerMoved;
        public event Action<PointerSample> PointerReleased;

        private bool mousePressed;

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                for (var i = 0; i < Input.touchCount; i++)
                {
                    var touch = Input.GetTouch(i);
                    var sample = new PointerSample(touch.position, touch.fingerId, true);
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            PointerPressed?.Invoke(sample);
                            break;
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            PointerMoved?.Invoke(sample);
                            break;
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            PointerReleased?.Invoke(new PointerSample(touch.position, touch.fingerId, true, touch.phase == TouchPhase.Canceled));
                            break;
                    }
                }
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                mousePressed = true;
                PointerPressed?.Invoke(new PointerSample(Input.mousePosition, 0, false));
            }

            if (mousePressed && Input.GetMouseButton(0))
            {
                PointerMoved?.Invoke(new PointerSample(Input.mousePosition, 0, false));
            }

            if (mousePressed && Input.GetMouseButtonUp(0))
            {
                mousePressed = false;
                PointerReleased?.Invoke(new PointerSample(Input.mousePosition, 0, false));
            }
#endif
        }
    }
}
