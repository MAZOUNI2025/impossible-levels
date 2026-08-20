# IMPOSSIBLE LEVELS — Final Game Specification

## Release identity

IMPOSSIBLE LEVELS is a portrait Android puzzle game for a 13+ audience, marketed primarily to ages 13–28. The player sees a simple room, identifies the misleading assumption, performs a small interaction, and earns a satisfying completion result.

## Core loop

The player selects an unlocked level, reads a short objective, experiments with the environment, solves the puzzle, receives a completion animation and reward, and unlocks the next level. The level map displays locked stages with a visible progression path and the best star result for every completed stage.

## Level completion and rewards

A completed level awards one to three stars and ten base coins. The star result is deterministic: three stars are awarded for a clean solution within the target time without a hint; two stars are awarded when the player exceeds the target time or uses a hint once; one star is awarded for any valid completion after multiple hints or retries. The game never removes a previously earned best score.

The next level unlocks immediately after the current level is completed. A level can be replayed at any time after unlocking it. Replay improves the stored star result only when the new result is better. Coins are used for optional hints and cosmetic profile items, but no paid item is required to complete a level.

## First-session hook

The first 12 seconds show a deliberately obvious-looking room. The objective says “Open the door.” The player taps the door and receives a quick, playful shake with a short “Not yet” feedback. The key is visible but placed away from the door. When the player taps the key, the room lighting shifts, the door glows purple, and the player understands the central rule: the obvious answer is not always the first move. The sequence ends with the line “Looks Easy. Think Again.” and transitions into Level 2.

## Screens

| Screen | Required content |
|---|---|
| Splash | Logo, short loading transition, no network dependency |
| Main Menu | Play, level map, player avatar, settings, coins |
| Level Map | 30 level nodes, locked/unlocked state, stars, current progress |
| Gameplay HUD | Objective, level number, pause, hint, coin count |
| Success | Stars, coins earned, replay, next level, return to map |
| Failure | Retry, optional rewarded continue, return to map |
| Settings | Music volume, SFX volume, vibration toggle, language placeholder, reset progress, privacy policy |
| Player | Avatar placeholder, total stars, completed levels, total coins, cosmetics placeholder |

## Audio and haptics

Every important action has a short sound: tap, drag start, object move, invalid action, hint, key pickup, door unlock, level success, level failure, button press, and reward. Music consists of a calm menu loop and a subtle gameplay loop. Haptics are short and optional, controlled by the settings screen. No audio is required for the game to remain understandable.

## Visual and animation requirements

The game uses a mature 2.5D style with deep navy backgrounds, amber interactive rewards, teal switches, and purple goals. The player character has idle breathing, look-at feedback, and a short success reaction. Interactive objects have a tap scale pulse, drag shadow, completion glow, and reset animation. Scene transitions use a short fade or slide. Success uses a restrained burst of particles; failure uses a soft shake without aggressive effects.

## Production definition of done

The game may be called release-candidate complete only when all 30 levels are playable from the map, every completed level unlocks the next node, best stars persist after relaunch, audio and haptics can be disabled, the settings screen works, the player screen displays real totals, the game has an offline-safe core loop, the production ad and billing adapters are tested, and a signed AAB has passed installation and device tests.
