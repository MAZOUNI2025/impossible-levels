using UnityEngine;

namespace ImpossibleLevels.Levels
{
    public sealed class KeyDoorPuzzle : MonoBehaviour
    {
        [SerializeField] private InteractiveObject2D key;
        [SerializeField] private InteractiveObject2D door;
        [SerializeField] private LevelRuntime levelRuntime;
        [SerializeField] private GameObject keyVisual;
        [SerializeField] private GameObject openDoorVisual;
        [SerializeField] private GameObject closedDoorVisual;

        private bool keyCollected;

        private void Awake()
        {
            if (levelRuntime == null) levelRuntime = FindFirstObjectByType<LevelRuntime>();
            if (key != null) key.Interacted += OnKeyInteracted;
            if (door != null) door.Interacted += OnDoorInteracted;
            RefreshVisuals();
        }

        private void OnDestroy()
        {
            if (key != null) key.Interacted -= OnKeyInteracted;
            if (door != null) door.Interacted -= OnDoorInteracted;
        }

        private void OnKeyInteracted(InteractiveObject2D interactedKey)
        {
            if (keyCollected) return;
            keyCollected = true;
            RefreshVisuals();
        }

        private void OnDoorInteracted(InteractiveObject2D interactedDoor)
        {
            if (!keyCollected || levelRuntime == null) return;
            levelRuntime.CompleteLevel();
            if (openDoorVisual != null) openDoorVisual.SetActive(true);
            if (closedDoorVisual != null) closedDoorVisual.SetActive(false);
        }

        private void RefreshVisuals()
        {
            if (keyVisual != null) keyVisual.SetActive(!keyCollected);
            if (openDoorVisual != null) openDoorVisual.SetActive(false);
            if (closedDoorVisual != null) closedDoorVisual.SetActive(true);
        }
    }
}
