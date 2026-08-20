# IMPOSSIBLE LEVELS — First 30 Levels

The first world is called **The Obvious Room**. Its visual language is a clean dark-blue room with one bright accent object per puzzle. The player learns that the most visible object is not always the correct first action.

| Level | Title | Type | Core trick | Hint direction |
|---:|---|---|---|---|
| 1 | The Key Is Right There | Interaction | Tap the key, then the door | Notice the object that changes the goal |
| 2 | Drag the Wrong Box | Drag | Move a harmless box to reveal the real target | The smallest object may matter most |
| 3 | One Button | Logic | Tap the button twice, not once | The first result is not the final state |
| 4 | The Quiet Switch | Observation | A low-contrast switch opens the exit | Look at the room edges |
| 5 | Do Not Touch the Door | Trick | Touch the floor marker to complete the level | The goal label can be misleading |
| 6 | Falling Up | Physics | Reverse the apparent gravity with a switch | Watch the direction of movement |
| 7 | The Fake Exit | Observation | The brightest door is decoration | Check which object has a collider |
| 8 | Three Seconds | Timing | Hold until the ring completes | A short press is not enough |
| 9 | The Heavy Key | Physics | Push a crate onto a pressure plate | The key is not meant to be dragged |
| 10 | Behind the Text | Interaction | Move the objective label out of the way | UI can hide part of the puzzle |
| 11 | Red Means Wait | Timing | Avoid the red phase and tap during green | Color is a timing signal |
| 12 | Two Doors | Logic | Open the door with the matching symbol | Compare symbols, not colors |
| 13 | The Stubborn Lever | Drag | Pull farther than the first animation suggests | Keep dragging after the first click |
| 14 | Small Gap | Physics | Roll the ball through a narrow opening | Use momentum rather than precision |
| 15 | The Missing Floor | Trick | Drag the background tile into the gap | The scenery is interactive |
| 16 | Silent Alarm | Observation | Disable the tiny alarm before touching the goal | Find what is moving |
| 17 | The Long Way | Logic | Take the longer path to activate both plates | The direct path is a trap |
| 18 | Almost Symmetric | Observation | Use the one object that breaks symmetry | Look for the mismatch |
| 19 | Hold Your Breath | Hold | Hold the character still while the platform moves | Sometimes no movement is the solution |
| 20 | The Third Tap | Logic | Two taps prepare the object; the third completes it | Count the state changes |
| 21 | Falling Key | Timing | Catch the key after releasing the support | Release is the action |
| 22 | Wrong Layer | Interaction | Tap the shadow, not the visible object | Shadows can reveal the real target |
| 23 | The Locked Hint | Logic | Use the hint button only after moving the blocker | The hint is part of the puzzle |
| 24 | Four Corners | Observation | Tap corners in the order shown by subtle marks | Order is hidden in the environment |
| 25 | The Impatient Door | Timing | Wait through the fake failure animation | Do not retry immediately |
| 26 | Mirror Room | Logic | Recreate the pattern on the opposite side | Symmetry is the instruction |
| 27 | The One-Way Box | Drag | Drag around the obstacle, never through it | Plan the complete path |
| 28 | The Last Coin | Trick | Ignore the coin and move its shadow | The reward is a distraction |
| 29 | Two-Step Reset | Logic | Reset once to expose the second state | Retry can change the puzzle |
| 30 | Looks Impossible | Combined | Key, timing gate, hidden switch, and door | Combine what the world taught you |

## Level authoring rule

Every level must have a visible objective, a deterministic solution, a reset path under three seconds, and a hint that narrows the search without stating the exact sequence. A level fails review if the player can only solve it by random tapping or if the result depends on frame-rate timing.

## Difficulty curve

Levels 1–5 teach one interaction at a time. Levels 6–10 introduce misleading affordances. Levels 11–15 use timing and bounded physics. Levels 16–20 test observation and state changes. Levels 21–25 combine timing with environmental clues. Levels 26–30 combine two or more learned mechanics and serve as the first shareable “impossible” moments for short-form video marketing.
