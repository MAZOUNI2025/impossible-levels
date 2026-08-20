using System;
using UnityEngine;
using ImpossibleLevels.Core;

namespace ImpossibleLevels.Levels
{
    public enum InteractionMode
    {
        Tap,
        Drag,
        Hold
    }

    [RequireComponent(typeof(Collider2D))]
    public sealed class InteractiveObject2D : MonoBehaviour
    {
        public event Action<InteractiveObject2D> Interacted;

        [SerializeField] private InteractionMode interactionMode = InteractionMode.Tap;
        [SerializeField] private Camera worldCamera;
        [SerializeField] private float dragZ = 0f;

        private TouchInputRouter inputRouter;
        private bool dragging;
        private int activePointerId = -1;
        private Vector3 dragOffset;

        private void Awake()
        {
            worldCamera = worldCamera != null ? worldCamera : Camera.main;
            inputRouter = FindFirstObjectByType<TouchInputRouter>();
        }

        private void OnEnable()
        {
            if (inputRouter == null)
            {
                inputRouter = FindFirstObjectByType<TouchInputRouter>();
            }

            if (inputRouter != null)
            {
                inputRouter.PointerPressed += OnPointerPressed;
                inputRouter.PointerMoved += OnPointerMoved;
                inputRouter.PointerReleased += OnPointerReleased;
            }
        }

        private void OnDisable()
        {
            if (inputRouter != null)
            {
                inputRouter.PointerPressed -= OnPointerPressed;
                inputRouter.PointerMoved -= OnPointerMoved;
                inputRouter.PointerReleased -= OnPointerReleased;
            }
        }

        private void OnPointerPressed(PointerSample sample)
        {
            if (!IsHit(sample) || dragging)
            {
                return;
            }

            activePointerId = sample.PointerId;
            if (interactionMode == InteractionMode.Drag)
            {
                dragging = true;
                var world = ScreenToWorld(sample.ScreenPosition);
                dragOffset = transform.position - world;
            }
            else if (interactionMode == InteractionMode.Tap)
            {
                Interacted?.Invoke(this);
            }
        }

        private void OnPointerMoved(PointerSample sample)
        {
            if (!dragging || sample.PointerId != activePointerId)
            {
                return;
            }

            transform.position = ScreenToWorld(sample.ScreenPosition) + dragOffset;
        }

        private void OnPointerReleased(PointerSample sample)
        {
            if (sample.PointerId != activePointerId)
            {
                return;
            }

            if (dragging)
            {
                dragging = false;
                Interacted?.Invoke(this);
            }

            activePointerId = -1;
        }

        private bool IsHit(PointerSample sample)
        {
            var world = ScreenToWorld(sample.ScreenPosition);
            var hit = Physics2D.OverlapPoint(world);
            return hit != null && hit.transform == transform;
        }

        private Vector3 ScreenToWorld(Vector2 screenPosition)
        {
            var world = worldCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Mathf.Abs(worldCamera.transform.position.z - dragZ)));
            world.z = dragZ;
            return world;
        }
    }
}
