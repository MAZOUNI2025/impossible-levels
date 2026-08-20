using System;
using UnityEngine;

namespace ImpossibleLevels.Levels
{
    public enum LevelState
    {
        Loading,
        Playing,
        Failed,
        Completed,
        Paused
    }

    public sealed class LevelRuntime : MonoBehaviour
    {
        public event Action<LevelState> StateChanged;
        public event Action<int> LevelCompleted;

        [SerializeField] private int levelIndex = 1;
        [SerializeField] private int defaultStars = 3;
        [SerializeField] private int coinReward = 10;
        [SerializeField] private Transform levelRoot;

        public LevelState State { get; private set; } = LevelState.Loading;
        public int LevelIndex => levelIndex;

        public void SetLevelIndex(int index)
        {
            levelIndex = Mathf.Clamp(index, 1, 30);
        }

        private void Start()
        {
            BeginLevel();
        }

        public void BeginLevel()
        {
            SetState(LevelState.Playing);
        }

        public void CompleteLevel()
        {
            if (State != LevelState.Playing)
            {
                return;
            }

            SetState(LevelState.Completed);
            LevelCompleted?.Invoke(levelIndex);
        }

        public void FailLevel()
        {
            if (State != LevelState.Playing)
            {
                return;
            }

            SetState(LevelState.Failed);
        }

        public void RetryLevel()
        {
            if (levelRoot != null)
            {
                for (var i = levelRoot.childCount - 1; i >= 0; i--)
                {
                    Destroy(levelRoot.GetChild(i).gameObject);
                }
            }

            BeginLevel();
        }

        public void TogglePause()
        {
            if (State == LevelState.Playing)
            {
                SetState(LevelState.Paused);
                Time.timeScale = 0f;
            }
            else if (State == LevelState.Paused)
            {
                Time.timeScale = 1f;
                SetState(LevelState.Playing);
            }
        }

        public int CalculateStars(float completionTimeSeconds, int hintCount)
        {
            var stars = defaultStars;
            if (completionTimeSeconds > 45f) stars--;
            if (completionTimeSeconds > 90f) stars--;
            if (hintCount > 0) stars--;
            return Mathf.Clamp(stars, 1, 3);
        }

        private void SetState(LevelState nextState)
        {
            State = nextState;
            StateChanged?.Invoke(State);
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}
