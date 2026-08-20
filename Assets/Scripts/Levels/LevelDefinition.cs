using System;
using UnityEngine;

namespace ImpossibleLevels.Levels
{
    public enum PuzzleType
    {
        Logic,
        Physics,
        Observation,
        Timing,
        Trick,
        Interaction
    }

    [CreateAssetMenu(fileName = "LevelDefinition", menuName = "Impossible Levels/Level Definition")]
    public sealed class LevelDefinition : ScriptableObject
    {
        [SerializeField] private int levelIndex = 1;
        [SerializeField] private string displayTitle = "Open the Door";
        [SerializeField, TextArea(2, 4)] private string objective = "Open the door.";
        [SerializeField] private PuzzleType puzzleType = PuzzleType.Logic;
        [SerializeField, Range(1, 10)] private int difficulty = 1;
        [SerializeField] private GameObject levelPrefab;
        [SerializeField] private string hintText = "Try interacting with the object closest to the goal.";
        [SerializeField] private float targetCompletionSeconds = 30f;

        public int LevelIndex => levelIndex;
        public string DisplayTitle => displayTitle;
        public string Objective => objective;
        public PuzzleType Type => puzzleType;
        public int Difficulty => difficulty;
        public GameObject LevelPrefab => levelPrefab;
        public string HintText => hintText;
        public float TargetCompletionSeconds => targetCompletionSeconds;
    }

    [Serializable]
    public struct LevelProgress
    {
        public int LevelIndex;
        public int Stars;
        public bool Completed;
        public int HintCount;
    }
}
