# Build and Publish Guide

## What is included

The package contains the project specification, Unity-ready C# scripts, first 30-level catalog, visual reference, compliance notes, MVP test plan, and Unity scene setup instructions. It is a source scaffold, not yet a compiled APK/AAB, because the Unity Editor and Android build toolchain are not available in this sandbox session.

## Build steps in Unity

1. Install a current Unity LTS release with 2D Core and Android Build Support.
2. Open or create a Unity project and copy the `Assets` directory into the project root.
3. Install or enable TextMeshPro and create the `MainMenu` and `Gameplay` scenes described in `UNITY_SETUP.md`.
4. Add both scenes to Build Settings, with `MainMenu` first.
5. Assemble Level 1 using the `KeyDoorPuzzle`, `LevelRuntime`, and `InteractiveObject2D` components.
6. Run the offline tests in `MVP_TEST_PLAN.md` in the Editor, then on a real Android device.
7. Replace the offline monetization adapter with the chosen production ad and billing SDK only after the core tests pass.
8. Set Android package name, app icon, signing keystore, version code, and version name.
9. Build an Android App Bundle (`.aab`) for Google Play. Use an APK only for direct device testing.
10. Complete the Google Play target-audience, content-rating, Data safety, ads, and store-listing declarations accurately.

## Release gate

Do not publish until Level 1–5 are playable, progress survives relaunch, ads do not interrupt active puzzle interaction, the app works with network disabled, the privacy policy is available at a public URL, and the store listing screenshots match the actual game.

## Recommended first release

Launch with 30 levels, no forced interstitial on the first session, opt-in rewarded hints, a remove-ads purchase, and a short onboarding sequence. Add new level packs only after retention and crash data show that the first world is stable.
