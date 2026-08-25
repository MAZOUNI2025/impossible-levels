using System;

namespace ImpossibleLevels.Levels
{
    public enum GameplayRule
    {
        KeyDoor,
        DragPlace,
        SwitchState,
        RevealObservation,
        FairSequence
    }

    [Serializable]
    public sealed class LevelMechanicConfig
    {
        public int levelId;
        public PuzzleType catalogType;
        public string catalogObjective;
        public GameplayRule rule;
        public int deterministicSeed;
        public string[] objects;
        public string goal;
        public string failCondition;
        public string hint;
        public int difficulty;
        public string tutorialCue;
        public int sequenceLength;

        public static LevelMechanicConfig ForEntry(int index, PuzzleType type, int difficulty, string objective, string hint)
        {
            var rule = index switch
            {
                2 => GameplayRule.DragPlace,
                3 => GameplayRule.SwitchState,
                4 => GameplayRule.RevealObservation,
                5 => GameplayRule.FairSequence,
                _ => GameplayRule.KeyDoor
            };

            var config = new LevelMechanicConfig
            {
                levelId = index,
                catalogType = type,
                catalogObjective = objective,
                rule = rule,
                deterministicSeed = 7919 * index + 17,
                objects = ObjectsFor(rule),
                goal = GoalFor(rule),
                failCondition = FailFor(rule),
                hint = hint,
                difficulty = difficulty,
                tutorialCue = TutorialFor(rule),
                sequenceLength = rule == GameplayRule.FairSequence ? 3 : 0
            };

            return config;
        }

        private static string[] ObjectsFor(GameplayRule rule)
        {
            return rule switch
            {
                GameplayRule.DragPlace => new[] { "Player", "Key", "Door", "Block", "BlockTarget" },
                GameplayRule.SwitchState => new[] { "Player", "Key", "Door", "Switch" },
                GameplayRule.RevealObservation => new[] { "Player", "Key(hidden)", "Door", "RevealTrigger" },
                GameplayRule.FairSequence => new[] { "Player", "Key", "Door", "SequenceStep" },
                _ => new[] { "Player", "Key", "Door" }
            };
        }

        private static string GoalFor(GameplayRule rule)
        {
            return rule switch
            {
                GameplayRule.DragPlace => "Collect the key, place the block in the socket, then open the door.",
                GameplayRule.SwitchState => "Collect the key, turn the switch on, then open the door.",
                GameplayRule.RevealObservation => "Reveal the hidden key, collect it, then open the door.",
                GameplayRule.FairSequence => "Collect the key, complete the fixed sequence, then open the door.",
                _ => "Collect the key, then open the door."
            };
        }

        private static string FailFor(GameplayRule rule)
        {
            return rule switch
            {
                GameplayRule.DragPlace => "An invalid drop returns the block to its start position.",
                GameplayRule.SwitchState => "There is no random failure; an incomplete state keeps the door locked.",
                GameplayRule.RevealObservation => "The hidden key remains unavailable until the reveal trigger is used.",
                GameplayRule.FairSequence => "An incorrect step resets the fixed sequence to the beginning.",
                _ => "Tapping the locked door gives invalid feedback and does not complete the level."
            };
        }

        private static string TutorialFor(GameplayRule rule)
        {
            return rule switch
            {
                GameplayRule.DragPlace => "Drag the block into the matching socket.",
                GameplayRule.SwitchState => "Turn the switch on before trying the door.",
                GameplayRule.RevealObservation => "Inspect the reveal trigger to show what the room hides.",
                GameplayRule.FairSequence => "Tap the three sequence markers in the demonstrated order.",
                _ => "Tap the key, then tap the door."
            };
        }
    }
}
