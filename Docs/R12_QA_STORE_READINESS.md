# IMPOSSIBLE LEVELS — R12 QA, Release Candidate & Store Readiness

## Scope and baseline

R12 was audited from the clean `main` baseline at `72976c0930f602384698bfd7b736558992731485`. The work in this phase is limited to regression evidence, release/store preparation, and truthful documentation. No gameplay, progression, input, HUD, level catalog, monetization behavior, Unity version, or Build Settings logic was changed.

The repository contains the two configured Unity scenes in this order:

| Build index | Scene | Enabled | Static result |
| ---: | --- | --- | --- |
| 0 | `Assets/Scenes/MainMenu.unity` | Yes | Present and listed in `EditorBuildSettings.asset` |
| 1 | `Assets/Scenes/Gameplay.unity` | Yes | Present and listed in `EditorBuildSettings.asset` |

## Static asset inventory

| Area | Verified inventory | Result |
| --- | ---: | --- |
| Unity scenes | 2 | Present: MainMenu and Gameplay |
| C# scripts | 24 | Present; delimiter sanity passed |
| Level thumbnails | 30 PNG + 30 matching `.meta` files | Complete for levels 01–30 |
| UI PNG assets | 6 | `ui_play`, `ui_levels`, `ui_player`, `ui_settings`, `ui_hint`, `ui_main_menu_atmosphere` |
| Gameplay PNG assets | 13 | Player, key, door, switch, block, decoy, floor, toprail, background, pause, coin, star states |
| Runtime audio files | 10 | Music and SFX files present under `Assets/Resources/Audio` |
| Runtime localization | English and Arabic | Dictionary parity checked in earlier R10 gate; R12 static inventory retained |
| Packages | Unity UI, TextMeshPro, 2D Sprite, Timeline, audio/physics/UI modules | No ads, billing, analytics, or Firebase SDK present |

The audio files under `Assets/Resources/Audio` currently have no committed `.meta` files in the repository. Unity can regenerate import metadata, but this remains a source-control/reimport risk and must be checked in a real Unity Editor before release.

## QA matrix

| Test area | Static evidence | Android/runtime evidence | Gate status |
| --- | --- | --- | --- |
| Main Menu → Level Map | Scene and route symbols present; build scene order verified | Not run | Static pass; runtime not verified |
| Locked/current/completed/stars map states | Progression and map code retained; 30-level catalog retained | Not run | Static pass; runtime not verified |
| Level 1 tutorial | R7 progressive tutorial code and localized cues retained | Not run | Static pass; runtime not verified |
| Gameplay mechanics | R2/R4 validators pass: five rules, router path, completion guard, 30 entries | Not run | Static pass; runtime not verified |
| Hint and economy | R8 validator pass: affordability and first-clear policy | Not run | Static pass; runtime not verified |
| Completion → stars/coins/next | Completion pipeline and R8 changes retained | Not run | Static pass; runtime not verified |
| Pause/resume/restart/settings/exit | Route symbols and controller callbacks present | Not run | Static pass; runtime not verified |
| English ↔ Arabic and RTL | EN/AR parity and idempotent RTL checks passed in R10 | Not run | Static pass; visual runtime not verified |
| Audio/SFX/haptics settings | Audio assets and setting controls present | Not run | Static pass; device behavior not verified |
| Persistence after restart | PlayerPrefs paths are present | Not run | Static pass only; persistence not exercised |
| Monetization | R11 fail-closed gateway retained; no provider SDK installed | Not run | Safe boundary pass; production monetization not configured |
| Performance/crash soak | No Unity profiler, device log, or signed build available | Not run | **Blocked** |
| Android install/launch | No Unity Editor, Android SDK/ADB, emulator, or device available | Not run | **Blocked** |

## Store preparation

The repository is ready for a listing-content review, but it does not yet contain evidence-grade store media. Google Play's official preview-assets guidance defines the acceptable screenshot formats and constraints; the final images must be captured from the real Android build rather than recreated from static source art [1]. A real gameplay trailer also remains outstanding because no gameplay capture was available in this environment.

| Store deliverable | Current state | Required before submission |
| --- | --- | --- |
| App name, short description, full description | Draft supplied in `Docs/R12_STORE_LISTING_COPY.md` | Product/legal review and final character-limit check in Play Console |
| Phone screenshots | Not captured | Capture Main Menu, Map, Tutorial, puzzle interaction, completion/reward, and Arabic RTL on a real device; validate PNG/JPEG and current Console limits [1] |
| Feature graphic | Not supplied | Produce a 1024×500 Play listing graphic using approved final branding and check current Console form [1] |
| Gameplay trailer | Not supplied | Record actual portrait gameplay, edit without fabricated UI claims, and upload only after runtime acceptance |
| App icon / launcher configuration | Not verifiable from current two-file `ProjectSettings` directory | Configure and verify in Unity PlayerSettings and Android build output |
| Content rating | Not submitted | Complete the Play Console questionnaire accurately for the final game and ads/content [2] |
| Target API | Not verifiable from repository-only settings | Confirm generated Android build targets the applicable current Play requirement; Google's published schedule states API 36 for new apps and updates from August 31, 2026 [3] [4] |
| Privacy/Data safety | No provider SDK enabled; legal URL not supplied | Provide a public privacy-policy URL and complete declarations after final SDK/data-flow decisions [5] |

## Release-candidate decision

The **static release-preparation gate is conditionally documented**, not an Android release-candidate acceptance. The project has the expected core assets, scenes, level catalog, localization, audio files, and protected gameplay systems, but performance, crash behavior, visual layout, audio/haptics, persistence, and monetization behavior cannot be accepted without a Unity Android build and device/emulator run.

> **ANDROID RUNTIME TEST: NOT AVAILABLE.**

Accordingly, the project must not yet be described as APK-ready, Play-Store-ready, or commercially publishable. R13 must remain blocked until the full device path is exercised and evidence is recorded.

## References

[1]: https://support.google.com/googleplay/android-developer/answer/9866151?hl=en "Google Play — Add preview assets to showcase your app"

[2]: https://support.google.com/googleplay/android-developer/answer/9859655?hl=en "Google Play — Content rating requirements"

[3]: https://support.google.com/googleplay/android-developer/answer/11926878?hl=en "Google Play — Target API level requirements"

[4]: https://developer.android.com/google/play/requirements/target-sdk "Android Developers — Target API level requirements"

[5]: https://support.google.com/googleplay/android-developer/answer/10144311?hl=en "Google Play — User Data policy"
