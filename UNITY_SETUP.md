# Unity Setup — Vertical Slice

## Recommended Unity version

Use a current Unity LTS release with 2D Core, TextMeshPro, Android Build Support, Android SDK & NDK Tools, and OpenJDK installed through Unity Hub.

## Scenes

Create two scenes:

- `MainMenu`
- `Gameplay`

Set both scenes in Build Settings, with `MainMenu` first.

## Bootstrap

In `MainMenu`, create an empty object named `GameBootstrap` and add `GameBootstrap.cs`. The object creates `TouchInputRouter` and `ProgressionService` automatically and keeps them alive between scenes.

## Gameplay scene

Create an orthographic camera at `(0, 0, -10)` and a root object named `LevelRoot`. Add `LevelRuntime.cs` to a separate object named `LevelRuntime`. Add `TouchInputRouter` only if it is not already created by `GameBootstrap`.

Every draggable or tappable object needs:

1. A `SpriteRenderer`.
2. A `Collider2D`.
3. `InteractiveObject2D.cs`.
4. A selected interaction mode: Tap or Drag.

## Level 1 vertical slice

Create three objects:

| Object | Component | Purpose |
|---|---|---|
| Character | SpriteRenderer + Collider2D | Player avatar |
| Key | SpriteRenderer + Collider2D + InteractiveObject2D | Tap target |
| Door | SpriteRenderer + Collider2D | Goal object |

For the first slice, the key interaction should enable the door. The door interaction should call `LevelRuntime.CompleteLevel()`. Keep the success condition deterministic and independent of physics.

## UI

Create a Canvas using `Canvas Scaler` set to `Scale With Screen Size`, reference resolution `1080 x 1920`, and match value `0.5`. Add:

- Objective text.
- Level number text.
- Coin counter.
- Pause button and pause panel.
- Hint button and hint text.
- Success panel with next-level button.
- Failure panel with retry and optional continue buttons.

Attach `GameHudController.cs` to a UI root and assign the panels, labels, buttons, and `LevelRuntime` references.

## Visual rules

Use a clean dark navy background, a warm accent color for interactive objects, and a contrasting goal color. Keep text short and readable. Avoid childish rounded bubble typography. Use shadows and parallax layers only after the basic level interaction is stable.

## Android baseline

Set portrait orientation first. Use IL2CPP for release builds, ARM64 architecture, a development build for internal testing, and a release keystore before publishing. Do not add ad SDKs until the offline vertical slice has passed functional testing.

## Offline acceptance test

Disable Wi-Fi or use airplane mode. Launch the game, start Level 1, tap the key, open the door, complete the level, close the app, relaunch, and verify that Level 2 is unlocked. The game must remain playable without any network connection or AI service.
