using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImpossibleLevels.Levels
{
    [Serializable]
    public sealed class RuntimeLevelEntry
    {
        public int index;
        public string title;
        public PuzzleType type;
        public int difficulty;
        public string objective;
        public string hint;
        public LevelMechanicConfig mechanics;
    }

    public static class LevelCatalogRuntime
    {
        public static IReadOnlyList<RuntimeLevelEntry> All { get; } = new List<RuntimeLevelEntry>
        {
            Entry(1, "The Key Is Right There", PuzzleType.Interaction, 1, "Collect the key, then open the door.", "Tap the key first, then tap the door."),
            Entry(2, "Drag the Wrong Box", PuzzleType.Interaction, 1, "Place the block in the socket, then open the door.", "Drag the block into the matching socket."),
            Entry(3, "One Button", PuzzleType.Logic, 1, "Collect the key, turn on the switch, then open the door.", "Turn the switch on before trying the door."),
            Entry(4, "The Quiet Switch", PuzzleType.Observation, 1, "Reveal the hidden key, collect it, then open the door.", "Inspect the reveal trigger to show what the room hides."),
            Entry(5, "Do Not Touch the Door", PuzzleType.Trick, 2, "Collect the key, complete the sequence, then open the door.", "Tap the three markers in the demonstrated order."),
            Entry(6, "Falling Up", PuzzleType.Physics, 2, "Collect the key, then open the door.", "Tap the key first, then tap the door."),
            Entry(7, "The Fake Exit", PuzzleType.Observation, 2, "Place the block in the socket, then open the door.", "Drag the block into the matching socket."),
            Entry(8, "Three Seconds", PuzzleType.Timing, 2, "Collect the key, turn on the switch, then open the door.", "Turn the switch on before trying the door."),
            Entry(9, "The Heavy Key", PuzzleType.Physics, 2, "Reveal the hidden key, collect it, then open the door.", "Inspect the reveal trigger to show what the room hides."),
            Entry(10, "Behind the Text", PuzzleType.Interaction, 2, "Collect the key, complete the sequence, then open the door.", "Tap the three markers in the demonstrated order."),
            Entry(11, "Red Means Wait", PuzzleType.Timing, 3, "Collect the key, then open the door.", "Tap the key first, then tap the door."),
            Entry(12, "Two Doors", PuzzleType.Logic, 3, "Place the block in the socket, then open the door.", "Drag the block into the matching socket."),
            Entry(13, "The Stubborn Lever", PuzzleType.Interaction, 3, "Collect the key, turn on the switch, then open the door.", "Turn the switch on before trying the door."),
            Entry(14, "Small Gap", PuzzleType.Physics, 3, "Reveal the hidden key, collect it, then open the door.", "Inspect the reveal trigger to show what the room hides."),
            Entry(15, "The Missing Floor", PuzzleType.Trick, 3, "Collect the key, complete the sequence, then open the door.", "Tap the three markers in the demonstrated order."),
            Entry(16, "Silent Alarm", PuzzleType.Observation, 4, "Collect the key, then open the door.", "Tap the key first, then tap the door."),
            Entry(17, "The Long Way", PuzzleType.Logic, 4, "Place the block in the socket, then open the door.", "Drag the block into the matching socket."),
            Entry(18, "Almost Symmetric", PuzzleType.Observation, 4, "Collect the key, turn on the switch, then open the door.", "Turn the switch on before trying the door."),
            Entry(19, "Hold Your Breath", PuzzleType.Hold, 4, "Reveal the hidden key, collect it, then open the door.", "Inspect the reveal trigger to show what the room hides."),
            Entry(20, "The Third Tap", PuzzleType.Logic, 4, "Collect the key, complete the sequence, then open the door.", "Tap the three markers in the demonstrated order."),
            Entry(21, "Falling Key", PuzzleType.Timing, 5, "Collect the key, then open the door.", "Tap the key first, then tap the door."),
            Entry(22, "Wrong Layer", PuzzleType.Observation, 5, "Place the block in the socket, then open the door.", "Drag the block into the matching socket."),
            Entry(23, "The Locked Hint", PuzzleType.Trick, 5, "Collect the key, turn on the switch, then open the door.", "Turn the switch on before trying the door."),
            Entry(24, "Four Corners", PuzzleType.Observation, 5, "Reveal the hidden key, collect it, then open the door.", "Inspect the reveal trigger to show what the room hides."),
            Entry(25, "The Impatient Door", PuzzleType.Timing, 5, "Collect the key, complete the sequence, then open the door.", "Tap the three markers in the demonstrated order."),
            Entry(26, "Mirror Room", PuzzleType.Logic, 6, "Collect the key, then open the door.", "Tap the key first, then tap the door."),
            Entry(27, "The One-Way Box", PuzzleType.Interaction, 6, "Place the block in the socket, then open the door.", "Drag the block into the matching socket."),
            Entry(28, "The Last Coin", PuzzleType.Trick, 6, "Collect the key, turn on the switch, then open the door.", "Turn the switch on before trying the door."),
            Entry(29, "Two-Step Reset", PuzzleType.Logic, 6, "Reveal the hidden key, collect it, then open the door.", "Inspect the reveal trigger to show what the room hides."),
            Entry(30, "Looks Impossible", PuzzleType.Logic, 7, "Collect the key, complete the sequence, then open the door.", "Tap the three markers in the demonstrated order.")
        };

        private static RuntimeLevelEntry Entry(int index, string title, PuzzleType type, int difficulty, string objective, string hint)
        {
            return new RuntimeLevelEntry
            {
                index = index,
                title = title,
                type = type,
                difficulty = difficulty,
                objective = objective,
                hint = hint,
                mechanics = LevelMechanicConfig.ForEntry(index, type, difficulty, objective, hint)
            };
        }
    }
}
