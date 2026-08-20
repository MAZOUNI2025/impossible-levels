# Art and Audio Import Map

## UI icons

Copy the following files into `Assets/Art/UI/` and assign them to the matching buttons:

| File | UI use |
|---|---|
| `ui_play.png` | Main Menu Play button |
| `ui_levels.png` | Level Map button |
| `ui_settings.png` | Settings button |
| `ui_player.png` | Player Profile button |
| `ui_hint.png` | Gameplay Hint button |
| `app_icon.png` | Android application icon and store asset source |

Use Sprite (2D and UI), preserve the square aspect ratio, set a readable size for small screens, and keep text labels beside important icons for accessibility.

## Audio

Copy `audio/menu_loop.wav` and `audio/gameplay_loop.wav` into `Assets/Audio/Music/`. Copy all files under `audio/sfx/` into `Assets/Audio/SFX/`. Assign the clips to `AudioDirector` in a persistent AudioDirector prefab. Set menu and gameplay loops to Loop. Use compressed audio for Android after verifying that short SFX remain crisp.

## Motion pass

Apply a short scale pulse to buttons on press, a 0.16-second color pulse to invalid objects, a glow and upward particle burst on success, a door fade on unlock, a level-node pop when a stage unlocks, and a 0.2-second screen shake only on invalid actions. Keep all motion optional or subtle enough to remain comfortable for 13+ general audiences.

## Runtime level board

Attach `ProceduralPuzzleBoard` to a Gameplay scene object, assign the Main Camera and an empty LevelRoot transform, and add `LevelRuntime` and `LevelCompletionRouter` to the same scene. The board can generate a deterministic playable board for each selected level index and accepts touch input on Android plus mouse input in the Editor.
