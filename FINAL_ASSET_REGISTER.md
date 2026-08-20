# Final Asset Register

## Playable content

The runtime board generates 30 deterministic level variants from the selected level index. Levels 1–6 introduce key, door, decoys, and tap actions. Levels 7–12 add switches and timing pressure. Levels 13–18 add draggable blocks and target placement. Levels 19–24 combine decoys, switches, and movable blocks. Levels 25–30 use the full combination with increased decoy count and more demanding layouts.

Every completed level calls the progression service with a star result from 1–3 and a coin reward. The next level unlocks only after the current level is completed. Progress is stored locally using PlayerPrefs.

## Visual assets

`level_01.png` through `level_30.png` are the level-map thumbnails. The first ten were generated as polished art-direction references. Levels 11–30 are deterministic local thumbnails derived from the approved visual reference so the project remains complete even after the daily image-generation quota is exhausted; they should be replaced with individually illustrated thumbnails in a later art pass if a higher-end store presentation is required.

The UI icon set includes Play, Levels, Settings, Player, and Hint. `app_icon.png` is the application icon source.

## Audio assets

The project includes original menu and gameplay loops plus short generated click, invalid, key pickup, door unlock, hint, success, failure, and pause effects. `AudioDirector` centralizes playback and respects the player's music and SFX settings.

## Final Unity assembly

Create a MainMenu scene and a Gameplay scene. Put `GameBootstrap` in a persistent root. In Gameplay, add a Camera, an empty `LevelRoot`, `LevelRuntime`, `LevelCompletionRouter`, `ProceduralPuzzleBoard`, `GameHudController`, and the UI canvas. Assign the audio clips and music clips to the `AudioDirector` prefab. Add the scenes to Build Settings in order. Test tap, drag, retry, pause, level completion, unlock, stars, coins, and audio toggles on a real Android device before generating the signed AAB.
