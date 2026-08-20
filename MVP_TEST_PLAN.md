# IMPOSSIBLE LEVELS — MVP Test Plan

## Functional acceptance

| ID | Test | Expected result |
|---|---|---|
| T01 | Launch the app from a clean install | MainMenu opens without errors |
| T02 | Press Start | Gameplay loads Level 1 |
| T03 | Tap the key | Key disappears or changes state and the door remains locked until the next action |
| T04 | Tap the door before collecting the key | Nothing completes; the player receives a readable feedback cue |
| T05 | Collect the key, then tap the door | LevelRuntime enters Completed and the success panel appears |
| T06 | Press Retry after a failed level | Level state resets and all interactive objects return to initial state |
| T07 | Close and relaunch after completing Level 1 | Level 2 is unlocked locally |
| T08 | Press Pause during active play | Time stops and pause panel appears |
| T09 | Resume from pause | Time resumes and the level remains in the same state |
| T10 | Use the hint button | Hint text appears and the hint count is recorded |

## Input acceptance

Touch tests must be executed on an Android device. Mouse fallback must be executed in the Unity Editor. Tap should not trigger when the pointer is outside the collider. Drag must follow the pointer without changing the object's Z position. A canceled touch must not complete a puzzle.

## Offline acceptance

Run the full T01–T10 sequence with network access disabled. The core game must remain usable. Rewarded ads and purchases may be unavailable, but the game must not crash, freeze, or block Level 1.

## Monetization acceptance

No interstitial or rewarded ad is shown automatically during active interaction. Rewarded ads are opt-in. If the provider is unavailable, the game shows a neutral message and preserves the player's progress. Remove-ads state is restored after relaunch when the production billing adapter is enabled.

## Content acceptance

All visible text is concise and appropriate for a 13+ audience. No level depends on random tapping, hidden frame-rate behavior, violence, gambling, sexual content, or external chat. The game remains understandable without reading long instructions.

## Performance acceptance

On a low-to-mid-range Android test device, the first level should load quickly, maintain stable input response, avoid allocations in the per-frame interaction path, and remain readable at portrait 16:9 and common tablet aspect ratios.
