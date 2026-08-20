# IMPOSSIBLE LEVELS — Project Specification

## Product decision

IMPOSSIBLE LEVELS is a 2.5D casual puzzle game for Android, built with Unity and C#. The official target audience is **13+**, with primary marketing focused on ages **13–28**. The game must not use any AI, LLM, cloud inference, or player-dependent AI service at runtime.

## Product promise

> Looks Easy. Think Again.

Every level should be understandable within five seconds, playable in a short session, and end with a satisfying “How did I not think of that?” moment. The game should feel clever rather than childish, surprising rather than frustrating, and polished rather than technically heavy.

## MVP scope

The first release candidate contains one world and 30 short levels. The first five levels teach tap, drag, environmental interaction, observation, and timing. Levels 6–10 introduce trick logic. Levels 11–15 introduce deterministic 2D physics. Levels 16–20 combine interaction and observation. Levels 21–25 increase timing and misleading affordances. Levels 26–30 combine two or more mechanics without requiring hidden rules.

Each level contains a goal, an interaction set, a success condition, a fail condition where appropriate, a reset action, and an optional hint. The first level must be completable without a hint. Hints must explain direction rather than reveal the entire solution.

## Runtime architecture

The project is divided into six runtime systems:

| System | Responsibility |
|---|---|
| Game Flow | Boot, main menu, level selection, loading, pause, success, failure, retry, and next level |
| Level Runtime | Spawns level data, interactive objects, goal object, fail zones, and completion state |
| Input | Tap, drag, swipe, and hold with touch-first handling and mouse fallback for Editor testing |
| Progress | Saves unlocked levels, stars, coins, hint usage, and settings locally |
| Presentation | Camera, parallax layers, shadows, particles, transitions, audio, and feedback |
| Monetization | Optional rewarded hint/continue and one-time remove-ads purchase; no forced ad during active play |

## Technical constraints

Use an orthographic camera. Use Physics2D only where a level requires it, and keep physics deterministic and bounded. Prefer SpriteRenderer, Canvas UI, lightweight 2D colliders, object pooling for repeated effects, and ScriptableObjects for level data. Avoid runtime downloads, remote configuration dependencies, and any service that creates a cost per active player.

The initial Android target should be optimized for low and mid-range devices. The game should use a fixed reference resolution, scalable Canvas UI, compressed textures, short audio clips, and minimal particles. Gameplay must remain usable at 16:9 and common tablet aspect ratios.

## Art direction

The visual language is stylized mobile casual puzzle: clean shapes, saturated but mature colors, readable silhouettes, soft shadows, shallow depth, layered backgrounds, gentle parallax, subtle lighting, and expressive but simple character animation. Avoid nursery motifs, baby language, excessive childish typography, realistic violence, sexual content, gambling mechanics, and dark adult themes.

## Monetization rules

The game is free to download. Rewarded ads are opt-in and may offer one hint, one continue, or a temporary coin multiplier. Interstitial ads may only appear at safe transition points after a completed or failed level and must be frequency-capped. A one-time remove-ads purchase is available. No purchase is required to complete a level, and no ad may appear during active puzzle interaction.

## Acceptance criteria for the first playable build

1. The player can launch the game, start Level 1, interact with objects, complete the level, and proceed to Level 2.
2. Retry resets the level deterministically.
3. Touch input works on Android and mouse input works in the Unity Editor.
4. Progress survives app restart using local storage.
5. The game runs without network access and without AI services.
6. The interface is readable, non-childish, and consistent with a 13+ casual puzzle title.
7. Ads and purchases are behind interfaces so they can be disabled during development and testing.
8. Every level can be authored without changing gameplay code.

## Build order

Build the game flow and input layer first, then the reusable interactive object layer, then Level 1–5 as a vertical slice. Only after the vertical slice is stable should the remaining 25 levels, monetization adapters, art polish, and store assets be added.

## Current status

The age direction is locked at 13+. The next implementation milestone is the vertical slice: boot screen, main menu, level runtime, touch interaction, one complete level, retry, success screen, and local progress save.
