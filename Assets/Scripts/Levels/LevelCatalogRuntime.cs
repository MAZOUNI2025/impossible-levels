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
            Entry(1, "The Key Is Right There", PuzzleType.Interaction, 1, "Open the door.", "Notice the object that changes the goal."),
            Entry(2, "Drag the Wrong Box", PuzzleType.Interaction, 1, "Reveal the real target.", "The smallest object may matter most."),
            Entry(3, "One Button", PuzzleType.Logic, 1, "Activate the button.", "The first result is not the final state."),
            Entry(4, "The Quiet Switch", PuzzleType.Observation, 1, "Find the hidden switch.", "Look at the room edges."),
            Entry(5, "Do Not Touch the Door", PuzzleType.Trick, 2, "Reach the goal without the door.", "The goal label can be misleading."),
            Entry(6, "Falling Up", PuzzleType.Physics, 2, "Reach the exit.", "Watch the direction of movement."),
            Entry(7, "The Fake Exit", PuzzleType.Observation, 2, "Find the real exit.", "Check which object has a collider."),
            Entry(8, "Three Seconds", PuzzleType.Timing, 2, "Hold until the ring completes.", "A short press is not enough."),
            Entry(9, "The Heavy Key", PuzzleType.Physics, 2, "Activate the pressure plate.", "The key is not meant to be dragged."),
            Entry(10, "Behind the Text", PuzzleType.Interaction, 2, "Reveal the hidden control.", "UI can hide part of the puzzle."),
            Entry(11, "Red Means Wait", PuzzleType.Timing, 3, "Tap during the safe phase.", "Color is a timing signal."),
            Entry(12, "Two Doors", PuzzleType.Logic, 3, "Open the matching door.", "Compare symbols, not colors."),
            Entry(13, "The Stubborn Lever", PuzzleType.Interaction, 3, "Pull the lever fully.", "Keep dragging after the first click."),
            Entry(14, "Small Gap", PuzzleType.Physics, 3, "Roll the ball to the goal.", "Use momentum rather than precision."),
            Entry(15, "The Missing Floor", PuzzleType.Trick, 3, "Cross the missing section.", "The scenery is interactive."),
            Entry(16, "Silent Alarm", PuzzleType.Observation, 4, "Disable the alarm.", "Find what is moving."),
            Entry(17, "The Long Way", PuzzleType.Logic, 4, "Activate both plates.", "The direct path is a trap."),
            Entry(18, "Almost Symmetric", PuzzleType.Observation, 4, "Find the mismatch.", "Look for the object that breaks symmetry."),
            Entry(19, "Hold Your Breath", PuzzleType.Hold, 4, "Keep the character still.", "Sometimes no movement is the solution."),
            Entry(20, "The Third Tap", PuzzleType.Logic, 4, "Complete the three-state switch.", "Count the state changes."),
            Entry(21, "Falling Key", PuzzleType.Timing, 5, "Catch the key.", "Release is the action."),
            Entry(22, "Wrong Layer", PuzzleType.Observation, 5, "Tap the true target.", "Shadows can reveal the real target."),
            Entry(23, "The Locked Hint", PuzzleType.Trick, 5, "Unlock the hint path.", "The hint is part of the puzzle."),
            Entry(24, "Four Corners", PuzzleType.Observation, 5, "Tap the corners in order.", "Order is hidden in the environment."),
            Entry(25, "The Impatient Door", PuzzleType.Timing, 5, "Wait for the true opening.", "Do not retry immediately."),
            Entry(26, "Mirror Room", PuzzleType.Logic, 6, "Recreate the pattern.", "Symmetry is the instruction."),
            Entry(27, "The One-Way Box", PuzzleType.Interaction, 6, "Move the box around the obstacle.", "Plan the complete path."),
            Entry(28, "The Last Coin", PuzzleType.Trick, 6, "Reach the door.", "The reward is a distraction."),
            Entry(29, "Two-Step Reset", PuzzleType.Logic, 6, "Reveal the second state.", "Retry can change the puzzle."),
            Entry(30, "Looks Impossible", PuzzleType.Logic, 7, "Open the final door.", "Combine what the world taught you.")
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
