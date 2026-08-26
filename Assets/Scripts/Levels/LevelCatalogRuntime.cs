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
            Entry(2, "Drag the Wrong Box", PuzzleType.Interaction, 1, "Fit the block into its socket, then open the door.", "Drag the block into the matching socket."),
            Entry(3, "One Button", PuzzleType.Logic, 1, "Collect the key and power the switch before opening the door.", "Turn the switch on before trying the door."),
            Entry(4, "The Quiet Switch", PuzzleType.Observation, 1, "Inspect the room to reveal the hidden key, then escape.", "Inspect the reveal trigger to show what the room hides."),
            Entry(5, "Do Not Touch the Door", PuzzleType.Trick, 2, "Collect the key and clear the three-step sequence before opening the door.", "Tap the three markers in the demonstrated order."),
            Entry(6, "Falling Up", PuzzleType.Physics, 2, "Spot the real key among the decoys, then open the door.", "Tap the real key, then head for the door."),
            Entry(7, "The Fake Exit", PuzzleType.Observation, 2, "Place the block correctly while ignoring the decoy shapes, then escape.", "Drag the block into the socket and ignore the decoy shapes."),
            Entry(8, "Three Seconds", PuzzleType.Timing, 2, "Power the switch, avoid the false targets, and open the door.", "Power the switch, then look for the safe route."),
            Entry(9, "The Heavy Key", PuzzleType.Physics, 2, "Reveal the hidden key, collect it, and ignore the decoys on the way out.", "Use the reveal marker before searching for the key."),
            Entry(10, "Behind the Text", PuzzleType.Interaction, 2, "Collect the key, then solve the marked sequence despite the distractions.", "Collect the key, then start at the first sequence marker."),
            Entry(11, "Red Means Wait", PuzzleType.Timing, 3, "Find the key, activate the switch, and choose the safe route to the door.", "Study the room, choose the key, then activate the switch."),
            Entry(12, "Two Doors", PuzzleType.Logic, 3, "Place the block and activate the switch before making your escape.", "Finish the block placement before toggling the switch."),
            Entry(13, "The Stubborn Lever", PuzzleType.Interaction, 3, "Power the switch, then complete the sequence and open the door.", "After powering the switch, tap the markers in order."),
            Entry(14, "Small Gap", PuzzleType.Physics, 3, "Reveal the key, fit the block into its socket, then escape.", "Reveal the key first, then move the block to its socket."),
            Entry(15, "The Missing Floor", PuzzleType.Trick, 3, "Place the block and finish the sequence before touching the door.", "Place the block, then begin with sequence marker one."),
            Entry(16, "Silent Alarm", PuzzleType.Observation, 4, "Reveal the key, power the switch, then open the exit.", "Use the reveal marker and power the switch before the door."),
            Entry(17, "The Long Way", PuzzleType.Logic, 4, "Fit the block, activate the switch, and avoid three decoys.", "Place the block, activate the switch, and ignore the decoys."),
            Entry(18, "Almost Symmetric", PuzzleType.Observation, 4, "Reveal the key, then follow every marker in the correct order.", "Reveal the key, then tap markers from first to last."),
            Entry(19, "Hold Your Breath", PuzzleType.Hold, 4, "Activate the switch and finish the sequence before opening the door.", "Power the switch and do not skip a sequence marker."),
            Entry(20, "The Third Tap", PuzzleType.Logic, 4, "Reveal, collect, place, power, and complete the sequence before the final tap.", "This room needs every tool; begin with the reveal marker."),
            Entry(21, "Falling Key", PuzzleType.Timing, 5, "Collect the key and complete the four-marker sequence through the decoys.", "Collect the key, then follow four markers in order."),
            Entry(22, "Wrong Layer", PuzzleType.Observation, 5, "Reveal the key, then place the block in the distant socket.", "Reveal the key, then drag the block to the distant socket."),
            Entry(23, "The Locked Hint", PuzzleType.Trick, 5, "Power the switch and complete the sequence without trusting the decoys.", "Power the switch before starting the sequence."),
            Entry(24, "Four Corners", PuzzleType.Observation, 5, "Reveal the key, place the block, and power the switch to escape.", "Reveal, place, then power the switch in that order."),
            Entry(25, "The Impatient Door", PuzzleType.Timing, 5, "Reveal the key, power the switch, and complete the sequence at the exit.", "After revealing and powering, start at marker one."),
            Entry(26, "Mirror Room", PuzzleType.Logic, 6, "Reveal the key, place the block, and finish the sequence in order.", "Reveal the key, place the block, then follow the sequence."),
            Entry(27, "The One-Way Box", PuzzleType.Interaction, 6, "Place the block, power the switch, and complete the five-marker sequence.", "Place the block, power the switch, then clear five markers."),
            Entry(28, "The Last Coin", PuzzleType.Trick, 6, "Reveal the key and place the block while the decoys guard the route.", "The key is hidden; reveal it, place the block, and ignore decoys."),
            Entry(29, "Two-Step Reset", PuzzleType.Logic, 6, "Reveal, place, power, and sequence every requirement in this final test.", "Do not stop after one requirement; complete all four."),
            Entry(30, "Looks Impossible", PuzzleType.Logic, 7, "Master the block, reveal, switch, and five-step sequence, then open the door.", "Begin with reveal, then block, switch, and the final sequence.")
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
