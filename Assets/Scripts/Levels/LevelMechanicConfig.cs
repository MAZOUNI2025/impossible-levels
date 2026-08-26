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
        public int contentTier;
        public int variationIndex;
        public int decoyCount;
        public bool requiresBlockPlacement;
        public bool requiresSwitch;
        public bool requiresReveal;
        public bool requiresSequence;
        public int layoutVariant;

        public static LevelMechanicConfig ForEntry(int index, PuzzleType type, int difficulty, string objective, string hint)
        {
            var safeIndex = Math.Max(1, index);
            var band = (safeIndex - 1) / 5;
            var slot = (safeIndex - 1) % 5;
            var rule = (GameplayRule)slot;
            var config = new LevelMechanicConfig
            {
                levelId = safeIndex,
                catalogType = type,
                catalogObjective = objective,
                rule = rule,
                deterministicSeed = 7919 * safeIndex + 17,
                hint = hint,
                difficulty = Math.Max(1, difficulty),
                contentTier = band + 1,
                variationIndex = safeIndex - 1,
                layoutVariant = safeIndex - 1
            };

            ConfigureRecipe(config, safeIndex);
            config.sequenceLength = SequenceLengthFor(safeIndex, config.requiresSequence, band);
            config.objects = ObjectsFor(config);
            config.goal = GoalFor(config);
            config.failCondition = FailFor(config);
            config.tutorialCue = TutorialFor(config);
            return config;
        }

        private static void ConfigureRecipe(LevelMechanicConfig config, int index)
        {
            // Levels 1-5 teach one mechanic at a time. Later rooms deliberately
            // combine the same proven interactions instead of changing input systems.
            switch (index)
            {
                case 2:
                    config.requiresBlockPlacement = true;
                    break;
                case 3:
                    config.requiresSwitch = true;
                    break;
                case 4:
                    config.requiresReveal = true;
                    break;
                case 5:
                    config.requiresSequence = true;
                    config.decoyCount = 1;
                    break;
                case 6:
                    config.decoyCount = 1;
                    break;
                case 7:
                    config.requiresBlockPlacement = true;
                    config.decoyCount = 2;
                    break;
                case 8:
                    config.requiresSwitch = true;
                    config.decoyCount = 1;
                    break;
                case 9:
                    config.requiresReveal = true;
                    config.decoyCount = 2;
                    break;
                case 10:
                    config.requiresSequence = true;
                    config.decoyCount = 1;
                    break;
                case 11:
                    config.requiresSwitch = true;
                    config.decoyCount = 2;
                    break;
                case 12:
                    config.requiresBlockPlacement = true;
                    config.requiresSwitch = true;
                    config.decoyCount = 1;
                    break;
                case 13:
                    config.requiresSwitch = true;
                    config.requiresSequence = true;
                    config.decoyCount = 2;
                    break;
                case 14:
                    config.requiresReveal = true;
                    config.requiresBlockPlacement = true;
                    config.decoyCount = 1;
                    break;
                case 15:
                    config.requiresBlockPlacement = true;
                    config.requiresSequence = true;
                    config.decoyCount = 2;
                    break;
                case 16:
                    config.requiresReveal = true;
                    config.requiresSwitch = true;
                    config.decoyCount = 2;
                    break;
                case 17:
                    config.requiresBlockPlacement = true;
                    config.requiresSwitch = true;
                    config.decoyCount = 3;
                    break;
                case 18:
                    config.requiresReveal = true;
                    config.requiresSequence = true;
                    config.decoyCount = 2;
                    break;
                case 19:
                    config.requiresSwitch = true;
                    config.requiresSequence = true;
                    config.decoyCount = 2;
                    break;
                case 20:
                    config.requiresBlockPlacement = true;
                    config.requiresReveal = true;
                    config.requiresSwitch = true;
                    config.requiresSequence = true;
                    config.decoyCount = 2;
                    break;
                case 21:
                    config.requiresSequence = true;
                    config.decoyCount = 3;
                    break;
                case 22:
                    config.requiresBlockPlacement = true;
                    config.requiresReveal = true;
                    config.decoyCount = 2;
                    break;
                case 23:
                    config.requiresSwitch = true;
                    config.requiresSequence = true;
                    config.decoyCount = 2;
                    break;
                case 24:
                    config.requiresBlockPlacement = true;
                    config.requiresReveal = true;
                    config.requiresSwitch = true;
                    config.decoyCount = 3;
                    break;
                case 25:
                    config.requiresReveal = true;
                    config.requiresSwitch = true;
                    config.requiresSequence = true;
                    config.decoyCount = 3;
                    break;
                case 26:
                    config.requiresBlockPlacement = true;
                    config.requiresReveal = true;
                    config.requiresSequence = true;
                    config.decoyCount = 3;
                    break;
                case 27:
                    config.requiresBlockPlacement = true;
                    config.requiresSwitch = true;
                    config.requiresSequence = true;
                    config.decoyCount = 3;
                    break;
                case 28:
                    config.requiresBlockPlacement = true;
                    config.requiresReveal = true;
                    config.decoyCount = 3;
                    break;
                case 29:
                    config.requiresBlockPlacement = true;
                    config.requiresReveal = true;
                    config.requiresSwitch = true;
                    config.requiresSequence = true;
                    config.decoyCount = 3;
                    break;
                case 30:
                    config.requiresBlockPlacement = true;
                    config.requiresReveal = true;
                    config.requiresSwitch = true;
                    config.requiresSequence = true;
                    config.decoyCount = 3;
                    break;
            }
        }

        private static int SequenceLengthFor(int index, bool requiresSequence, int band)
        {
            if (!requiresSequence) return 0;
            return index switch
            {
                5 => 3,
                10 => 3,
                13 => 3,
                15 => 4,
                18 => 4,
                19 => 4,
                20 => 4,
                21 => 4,
                23 => 4,
                25 => 4,
                26 => 4,
                27 => 5,
                29 => 5,
                30 => 5,
                _ => 2 + Math.Min(3, band)
            };
        }

        private static string[] ObjectsFor(LevelMechanicConfig config)
        {
            var objects = new[] { "Player", "Key", "Door" };
            if (config.requiresBlockPlacement) objects = AddObject(objects, "Block", "BlockTarget");
            if (config.requiresSwitch) objects = AddObject(objects, "Switch");
            if (config.requiresReveal) objects = AddObject(objects, "Key(hidden)", "RevealTrigger");
            if (config.requiresSequence) objects = AddObject(objects, "SequenceStep");
            if (config.decoyCount > 0) objects = AddObject(objects, "Decoy");
            return objects;
        }

        private static string[] AddObject(string[] source, params string[] additions)
        {
            var result = new string[source.Length + additions.Length];
            Array.Copy(source, result, source.Length);
            Array.Copy(additions, 0, result, source.Length, additions.Length);
            return result;
        }

        private static string GoalFor(LevelMechanicConfig config)
        {
            var goal = "Collect the key";
            if (config.requiresReveal) goal = "Reveal the key, then collect it";
            if (config.requiresBlockPlacement) goal += ", place the block in the socket";
            if (config.requiresSwitch) goal += ", turn on the switch";
            if (config.requiresSequence) goal += ", complete the sequence";
            return goal + ", then open the door.";
        }

        private static string FailFor(LevelMechanicConfig config)
        {
            if (config.requiresSequence) return "An incorrect sequence step resets the sequence; incomplete requirements keep the door locked.";
            if (config.requiresBlockPlacement) return "An invalid drop returns the block to its start position; incomplete requirements keep the door locked.";
            return "Tapping the locked door gives invalid feedback and does not complete the level.";
        }

        private static string TutorialFor(LevelMechanicConfig config)
        {
            if (config.requiresReveal) return "Inspect the reveal marker first, then follow the remaining room requirements.";
            if (config.requiresBlockPlacement) return "Drag the block into its socket, then complete the other visible requirements.";
            if (config.requiresSwitch) return "Turn the switch on, then complete the other visible requirements.";
            if (config.requiresSequence) return "Collect the key, then tap the sequence markers in order.";
            return "Tap the key, then tap the door.";
        }
    }
}
