using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using ImpossibleLevels.Levels;
using ImpossibleLevels.UI;
using ImpossibleLevels.Audio;

namespace ImpossibleLevels.Core
{
    public sealed class RuntimeSceneBootstrap : MonoBehaviour
    {
        private static int TotalLevels => LevelCatalogRuntime.All == null ? 0 : LevelCatalogRuntime.All.Count;
        private Canvas canvas;
        private Camera gameplayCamera;
        private static TMP_FontAsset runtimeFontAsset;

        private void Awake()
        {
            EnsureGameServices();
            EnsureCamera();
            EnsureEventSystem();
            var sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Gameplay") BuildGameplay();
            else BuildMainMenu();
            if (AudioDirector.Instance != null)
            {
                if (sceneName == "Gameplay") AudioDirector.Instance.PlayGameplayMusic();
                else AudioDirector.Instance.PlayMenuMusic();
            }
        }

        private void EnsureGameServices()
        {
            if (FindFirstObjectByType<GameBootstrap>() != null) return;
            var services = new GameObject("GameServices");
            services.AddComponent<GameBootstrap>();
        }

        private void EnsureCamera()
        {
            gameplayCamera = Camera.main;
            if (gameplayCamera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                gameplayCamera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            gameplayCamera.transform.position = new Vector3(0f, 0f, -10f);
            gameplayCamera.orthographic = true;
            gameplayCamera.orthographicSize = 7.5f;
            gameplayCamera.clearFlags = CameraClearFlags.SolidColor;
            gameplayCamera.backgroundColor = new Color(0.035f, 0.055f, 0.14f);
        }

        private void EnsureEventSystem()
        {
            if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null) return;
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        private void BuildMainMenu()
        {
            canvas = CreateCanvas("MainMenuCanvas", true);
            var canvasRoot = canvas.transform as RectTransform;
            var mainScreen = CreateScreen("HomeScreen", canvasRoot);
            var mapScreen = CreateScreen("LevelMapScreen", canvasRoot);
            var profileScreen = CreateScreen("ProfileScreen", canvasRoot);
            var settingsScreen = CreateScreen("SettingsScreen", canvasRoot);

            // The menu is a presentation layer only: the atmosphere and motifs are non-interactive.
            var atmosphere = AddImagePanel(mainScreen, ArtAssetLibrary.GetUiIcon("main_menu_atmosphere"), Color.white, Vector2.zero, Vector2.one, false);
            atmosphere.type = Image.Type.Simple;
            AddPanel(mainScreen, new Color(0.015f, 0.025f, 0.09f, 0.32f), Vector2.zero, Vector2.one);
            AddPanel(mainScreen, new Color(0.005f, 0.012f, 0.05f, 0.64f), new Vector2(0.5f, 0.965f), new Vector2(1f, 0.07f));
            AddPanel(mainScreen, new Color(0.005f, 0.012f, 0.05f, 0.62f), new Vector2(0.5f, 0.035f), new Vector2(1f, 0.07f));
            AddPanel(mainScreen, new Color(0.10f, 0.82f, 0.78f, 0.22f), new Vector2(0.5f, 0.635f), new Vector2(0.62f, 0.004f));
            AddPanel(mainScreen, new Color(0.55f, 0.22f, 1f, 0.24f), new Vector2(0.5f, 0.285f), new Vector2(0.44f, 0.004f));
            AddImagePanel(mainScreen, ArtAssetLibrary.GetGameplaySprite("key"), new Color(1f, 0.78f, 0.24f, 0.30f), new Vector2(0.18f, 0.565f), new Vector2(0.11f, 0.085f), true);
            AddImagePanel(mainScreen, ArtAssetLibrary.GetGameplaySprite("door"), new Color(0.55f, 0.22f, 1f, 0.18f), new Vector2(0.82f, 0.575f), new Vector2(0.15f, 0.18f), true);

            var title = AddText(mainScreen, LocalizationService.Get("GAME_TITLE"), new Vector2(0.5f, 0.835f), new Vector2(0.92f, 0.085f), 56, Color.white, TextAlignmentOptions.Center);
            AddTextSurfaceEffect(title, new Color(0f, 0f, 0f, 0.62f), new Vector2(0f, -4f));
            title.characterSpacing = 2.5f;
            AddText(mainScreen, LocalizationService.Get("MENU_TAGLINE"), new Vector2(0.5f, 0.755f), new Vector2(0.86f, 0.045f), 24, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);
            AddText(mainScreen, LocalizationService.Get("MENU_SUBTITLE"), new Vector2(0.5f, 0.705f), new Vector2(0.88f, 0.04f), 17, new Color(0.82f, 0.86f, 0.96f), TextAlignmentOptions.Center);

            var menu = gameObject.AddComponent<MainMenuController>();
            AddMenuIconButton(mainScreen, LocalizationService.Get("MENU_PLAY"), "play", new Vector2(0.5f, 0.535f), new Vector2(0.44f, 0.145f), new Color(1f, 0.63f, 0.08f), menu.StartFirstLevel, true);
            AddMenuIconButton(mainScreen, LocalizationService.Get("MENU_LEVEL_MAP"), "levels", new Vector2(0.5f, 0.385f), new Vector2(0.36f, 0.115f), new Color(0.10f, 0.82f, 0.78f), () => ShowScreen(mapScreen.gameObject, mainScreen.gameObject, profileScreen.gameObject, settingsScreen.gameObject), false);
            AddMenuIconButton(mainScreen, LocalizationService.Get("MENU_PROFILE"), "player", new Vector2(0.29f, 0.205f), new Vector2(0.28f, 0.115f), new Color(0.55f, 0.22f, 1f), () => ShowScreen(profileScreen.gameObject, mainScreen.gameObject, mapScreen.gameObject, settingsScreen.gameObject), false);
            AddMenuIconButton(mainScreen, LocalizationService.Get("MENU_SETTINGS"), "settings", new Vector2(0.71f, 0.205f), new Vector2(0.28f, 0.115f), new Color(0.13f, 0.18f, 0.34f), () => ShowScreen(settingsScreen.gameObject, mainScreen.gameObject, mapScreen.gameObject, profileScreen.gameObject), false);
            AddText(mainScreen, LocalizationService.Get("MENU_FOOTER"), new Vector2(0.5f, 0.055f), new Vector2(0.92f, 0.03f), 16, new Color(0.70f, 0.76f, 0.90f), TextAlignmentOptions.Center);

            var mapController = gameObject.AddComponent<LevelMapController>();
            BuildLevelMap(mapScreen, mapController, mainScreen.gameObject, mapScreen.gameObject, profileScreen.gameObject, settingsScreen.gameObject);
            BuildProfile(profileScreen, mainScreen.gameObject, mapScreen.gameObject, profileScreen.gameObject, settingsScreen.gameObject);
            BuildSettings(settingsScreen, mainScreen.gameObject, mapScreen.gameObject, profileScreen.gameObject, settingsScreen.gameObject);
            var initialScreen = PlayerPrefs.GetString("il.main_menu_screen", string.Empty);
            PlayerPrefs.DeleteKey("il.main_menu_screen");
            if (initialScreen == "map") ShowScreen(mapScreen.gameObject, mainScreen.gameObject, profileScreen.gameObject, settingsScreen.gameObject);
            else if (initialScreen == "settings") ShowScreen(settingsScreen.gameObject, mainScreen.gameObject, mapScreen.gameObject, profileScreen.gameObject);
            else ShowScreen(mainScreen.gameObject, mapScreen.gameObject, profileScreen.gameObject, settingsScreen.gameObject);
        }

        private void BuildLevelMap(RectTransform screen, LevelMapController mapController, GameObject home, GameObject map, GameObject profile, GameObject settings)
        {
            AddPanel(screen, new Color(0.035f, 0.055f, 0.14f, 0.98f), Vector2.zero, Vector2.one);
            var progression = FindFirstObjectByType<ProgressionService>();
            var currentLevel = progression != null ? Mathf.Clamp(progression.HighestUnlockedLevel, 1, TotalLevels) : 1;
            var completedCount = 0;
            var totalStars = 0;
            for (var levelIndex = 1; levelIndex <= TotalLevels; levelIndex++)
            {
                var stars = mapController.GetLevelStars(levelIndex);
                if (stars > 0) completedCount++;
                totalStars += Mathf.Clamp(stars, 0, 3);
            }

            AddImagePanel(screen, ArtAssetLibrary.GetLevelThumbnail(currentLevel), new Color(1f, 1f, 1f, 0.045f), new Vector2(0.5f, 0.50f), new Vector2(0.86f, 0.58f), true);
            AddPanel(screen, new Color(0.10f, 0.82f, 0.78f, 0.12f), new Vector2(0.5f, 0.93f), new Vector2(0.92f, 0.15f));
            AddText(screen, LocalizationService.Get("MAP_TITLE"), new Vector2(0.5f, 0.962f), new Vector2(0.80f, 0.050f), 40, Color.white, TextAlignmentOptions.Center);
            AddText(screen, LocalizationService.Get("MAP_SUBTITLE"), new Vector2(0.5f, 0.918f), new Vector2(0.88f, 0.030f), 15, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);

            var progressLabel = AddText(screen, LocalizationService.Format("MAP_PROGRESS", completedCount, TotalLevels), new Vector2(0.25f, 0.862f), new Vector2(0.40f, 0.034f), 16, new Color(0.82f, 0.86f, 0.96f), TextAlignmentOptions.Center);
            progressLabel.raycastTarget = false;
            var starsLabel = AddText(screen, LocalizationService.Format("MAP_STARS", totalStars, TotalLevels * 3), new Vector2(0.75f, 0.862f), new Vector2(0.40f, 0.034f), 16, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Center);
            starsLabel.raycastTarget = false;
            var currentLabel = AddText(screen, LocalizationService.Format("MAP_CURRENT", currentLevel), new Vector2(0.5f, 0.818f), new Vector2(0.78f, 0.034f), 17, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Center);
            currentLabel.raycastTarget = false;

            var scroll = CreateScrollView(screen, new Vector2(0.5f, 0.44f), new Vector2(0.90f, 0.70f));
            var content = scroll.content;
            AddProgressionPath(content, mapController);
            for (var levelIndex = 1; levelIndex <= TotalLevels; levelIndex++)
            {
                var capturedLevel = levelIndex;
                var entry = LevelCatalogRuntime.All[levelIndex - 1];
                AddLevelCard(content, entry, mapController, currentLevel, () => mapController.SelectLevel(capturedLevel));
            }

            AddButton(screen, LocalizationService.Get("MENU_BACK"), new Vector2(0.5f, 0.045f), new Vector2(0.32f, 0.072f), new Color(0.13f, 0.18f, 0.34f), () => ShowScreen(home, map, profile, settings));
        }

        private void BuildProfile(RectTransform screen, GameObject home, GameObject map, GameObject profile, GameObject settings)
        {
            AddPanel(screen, new Color(0.035f, 0.055f, 0.14f, 0.98f), Vector2.zero, Vector2.one);
            AddImagePanel(screen, ArtAssetLibrary.GetLevelThumbnail(18), new Color(1f, 1f, 1f, 0.04f), new Vector2(0.5f, 0.53f), new Vector2(0.78f, 0.60f), true);
            AddPanel(screen, new Color(0.005f, 0.012f, 0.05f, 0.72f), new Vector2(0.5f, 0.84f), new Vector2(0.92f, 0.22f));
            AddText(screen, LocalizationService.Get("PROFILE_TITLE"), new Vector2(0.5f, 0.92f), new Vector2(0.84f, 0.065f), 43, Color.white, TextAlignmentOptions.Center);
            AddImagePanel(screen, ArtAssetLibrary.GetUiIcon("player"), Color.white, new Vector2(0.5f, 0.83f), new Vector2(0.16f, 0.10f), true);

            var progression = FindFirstObjectByType<ProgressionService>();
            var profileService = FindFirstObjectByType<PlayerProfileService>();
            var totalLevels = TotalLevels;
            if (profileService != null) profileService.RefreshTotals(totalLevels);
            var completed = profileService != null ? Mathf.Clamp(profileService.CompletedLevels, 0, totalLevels) : 0;
            var stars = profileService != null ? Mathf.Clamp(profileService.TotalStars, 0, totalLevels * 3) : 0;
            var coins = progression != null ? Mathf.Max(0, progression.Coins) : 0;
            var progress = totalLevels > 0 ? Mathf.Clamp01(completed / (float)totalLevels) : 0f;
            var percentage = Mathf.Clamp(Mathf.RoundToInt(progress * 100f), 0, 100);

            AddPanel(screen, new Color(0.06f, 0.08f, 0.16f, 0.94f), new Vector2(0.5f, 0.58f), new Vector2(0.90f, 0.36f));
            AddText(screen, LocalizationService.Get("PROFILE_PROGRESS"), new Vector2(0.5f, 0.715f), new Vector2(0.76f, 0.04f), 19, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);
            AddPanel(screen, new Color(0.08f, 0.10f, 0.19f, 1f), new Vector2(0.5f, 0.66f), new Vector2(0.72f, 0.028f));
            if (progress > 0f)
            {
                AddPanel(screen, new Color(0.10f, 0.82f, 0.78f, 1f), new Vector2(0.14f + 0.36f * progress, 0.66f), new Vector2(0.72f * progress, 0.028f));
            }
            AddText(screen, LocalizationService.Format("PROFILE_COMPLETED", completed), new Vector2(0.27f, 0.57f), new Vector2(0.40f, 0.055f), 21, Color.white, TextAlignmentOptions.Center);
            AddText(screen, LocalizationService.Format("PROFILE_TOTAL_LEVELS", totalLevels), new Vector2(0.73f, 0.57f), new Vector2(0.40f, 0.055f), 21, Color.white, TextAlignmentOptions.Center);
            AddText(screen, LocalizationService.Format("PROFILE_COMPLETION_PERCENT", percentage), new Vector2(0.5f, 0.49f), new Vector2(0.78f, 0.05f), 23, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Center);
            AddImagePanel(screen, ArtAssetLibrary.GetGameplaySprite("star_filled"), Color.white, new Vector2(0.29f, 0.39f), new Vector2(0.060f, 0.050f), true);
            AddText(screen, LocalizationService.Format("PROFILE_STARS_LABEL", stars, totalLevels * 3), new Vector2(0.43f, 0.39f), new Vector2(0.30f, 0.05f), 19, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Center);
            AddImagePanel(screen, ArtAssetLibrary.GetGameplaySprite("coin"), Color.white, new Vector2(0.64f, 0.39f), new Vector2(0.060f, 0.050f), true);
            AddText(screen, LocalizationService.Format("PROFILE_COINS_LABEL", coins), new Vector2(0.77f, 0.39f), new Vector2(0.26f, 0.05f), 19, new Color(1f, 0.63f, 0.08f), TextAlignmentOptions.Center);
            AddText(screen, LocalizationService.Get("PROFILE_HINT"), new Vector2(0.5f, 0.23f), new Vector2(0.86f, 0.05f), 17, new Color(0.72f, 0.78f, 0.92f), TextAlignmentOptions.Center);
            AddButton(screen, LocalizationService.Get("MENU_BACK"), new Vector2(0.5f, 0.09f), new Vector2(0.32f, 0.07f), new Color(0.13f, 0.18f, 0.34f), () => ShowScreen(home, map, profile, settings));
        }

        private void BuildSettings(RectTransform screen, GameObject home, GameObject map, GameObject profile, GameObject settingsScreen)
        {
            AddPanel(screen, new Color(0.035f, 0.055f, 0.14f, 0.98f), Vector2.zero, Vector2.one);
            AddImagePanel(screen, ArtAssetLibrary.GetLevelThumbnail(27), new Color(1f, 1f, 1f, 0.035f), new Vector2(0.5f, 0.53f), new Vector2(0.78f, 0.60f), true);
            AddPanel(screen, new Color(0.005f, 0.012f, 0.05f, 0.72f), new Vector2(0.5f, 0.84f), new Vector2(0.92f, 0.22f));
            AddText(screen, LocalizationService.Get("SETTINGS_TITLE"), new Vector2(0.5f, 0.92f), new Vector2(0.82f, 0.065f), 46, Color.white, TextAlignmentOptions.Center);
            AddImagePanel(screen, ArtAssetLibrary.GetUiIcon("settings"), Color.white, new Vector2(0.5f, 0.83f), new Vector2(0.16f, 0.10f), true);

            var controller = gameObject.AddComponent<SettingsController>();
            var profileService = FindFirstObjectByType<PlayerProfileService>();
            var musicEnabled = profileService == null || profileService.MusicEnabled;
            var sfxEnabled = profileService == null || profileService.SfxEnabled;
            var hapticsEnabled = profileService == null || profileService.HapticsEnabled;
            var activeColor = new Color(0.10f, 0.82f, 0.78f);
            var inactiveColor = new Color(0.13f, 0.18f, 0.34f);
            var specialColor = new Color(0.55f, 0.22f, 1f);

            AddPanel(screen, new Color(0.06f, 0.08f, 0.16f, 0.94f), new Vector2(0.5f, 0.66f), new Vector2(0.90f, 0.23f));
            AddText(screen, LocalizationService.Get("SETTINGS_AUDIO"), new Vector2(0.5f, 0.745f), new Vector2(0.82f, 0.035f), 20, activeColor, TextAlignmentOptions.Center);
            var musicState = AddText(screen, LocalizationService.Format("SETTINGS_STATE", LocalizationService.Get("SETTINGS_MUSIC"), LocalizationService.Get(musicEnabled ? "SETTINGS_ON" : "SETTINGS_OFF")), new Vector2(0.16f, 0.69f), new Vector2(0.20f, 0.045f), 17, Color.white, TextAlignmentOptions.Left);
            AddButton(screen, LocalizationService.Get("SETTINGS_ON"), new Vector2(0.43f, 0.69f), new Vector2(0.20f, 0.065f), musicEnabled ? activeColor : inactiveColor, () => { controller.SetMusic(true); musicState.text = LocalizationService.Format("SETTINGS_STATE", LocalizationService.Get("SETTINGS_MUSIC"), LocalizationService.Get("SETTINGS_ON")); });
            AddButton(screen, LocalizationService.Get("SETTINGS_OFF"), new Vector2(0.70f, 0.69f), new Vector2(0.20f, 0.065f), musicEnabled ? inactiveColor : specialColor, () => { controller.SetMusic(false); musicState.text = LocalizationService.Format("SETTINGS_STATE", LocalizationService.Get("SETTINGS_MUSIC"), LocalizationService.Get("SETTINGS_OFF")); });
            var sfxState = AddText(screen, LocalizationService.Format("SETTINGS_STATE", LocalizationService.Get("SETTINGS_SFX"), LocalizationService.Get(sfxEnabled ? "SETTINGS_ON" : "SETTINGS_OFF")), new Vector2(0.16f, 0.595f), new Vector2(0.20f, 0.045f), 17, Color.white, TextAlignmentOptions.Left);
            AddButton(screen, LocalizationService.Get("SETTINGS_ON"), new Vector2(0.43f, 0.595f), new Vector2(0.20f, 0.065f), sfxEnabled ? activeColor : inactiveColor, () => { controller.SetSfx(true); sfxState.text = LocalizationService.Format("SETTINGS_STATE", LocalizationService.Get("SETTINGS_SFX"), LocalizationService.Get("SETTINGS_ON")); });
            AddButton(screen, LocalizationService.Get("SETTINGS_OFF"), new Vector2(0.70f, 0.595f), new Vector2(0.20f, 0.065f), sfxEnabled ? inactiveColor : specialColor, () => { controller.SetSfx(false); sfxState.text = LocalizationService.Format("SETTINGS_STATE", LocalizationService.Get("SETTINGS_SFX"), LocalizationService.Get("SETTINGS_OFF")); });

            AddPanel(screen, new Color(0.06f, 0.08f, 0.16f, 0.94f), new Vector2(0.5f, 0.455f), new Vector2(0.90f, 0.13f));
            AddText(screen, LocalizationService.Get("SETTINGS_FEEDBACK"), new Vector2(0.5f, 0.495f), new Vector2(0.82f, 0.03f), 19, activeColor, TextAlignmentOptions.Center);
            var hapticsState = AddText(screen, LocalizationService.Format("SETTINGS_STATE", LocalizationService.Get("SETTINGS_HAPTICS"), LocalizationService.Get(hapticsEnabled ? "SETTINGS_ON" : "SETTINGS_OFF")), new Vector2(0.16f, 0.425f), new Vector2(0.22f, 0.045f), 17, Color.white, TextAlignmentOptions.Left);
            AddButton(screen, LocalizationService.Get("SETTINGS_ON"), new Vector2(0.43f, 0.425f), new Vector2(0.20f, 0.065f), hapticsEnabled ? activeColor : inactiveColor, () => { controller.SetHaptics(true); hapticsState.text = LocalizationService.Format("SETTINGS_STATE", LocalizationService.Get("SETTINGS_HAPTICS"), LocalizationService.Get("SETTINGS_ON")); });
            AddButton(screen, LocalizationService.Get("SETTINGS_OFF"), new Vector2(0.70f, 0.425f), new Vector2(0.20f, 0.065f), hapticsEnabled ? inactiveColor : specialColor, () => { controller.SetHaptics(false); hapticsState.text = LocalizationService.Format("SETTINGS_STATE", LocalizationService.Get("SETTINGS_HAPTICS"), LocalizationService.Get("SETTINGS_OFF")); });

            AddText(screen, LocalizationService.Get("SETTINGS_LANGUAGE"), new Vector2(0.5f, 0.325f), new Vector2(0.82f, 0.035f), 19, activeColor, TextAlignmentOptions.Center);
            AddButton(screen, LocalizationService.Get("SETTINGS_ENGLISH"), new Vector2(0.36f, 0.275f), new Vector2(0.28f, 0.065f), LocalizationService.CurrentLanguage == "en" ? activeColor : inactiveColor, () => { LocalizationService.SetLanguage("en"); SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
            AddButton(screen, LocalizationService.Get("SETTINGS_ARABIC"), new Vector2(0.64f, 0.275f), new Vector2(0.28f, 0.065f), LocalizationService.CurrentLanguage == "ar" ? activeColor : inactiveColor, () => { LocalizationService.SetLanguage("ar"); SceneManager.LoadScene(SceneManager.GetActiveScene().name); });

            AddText(screen, LocalizationService.Get("SETTINGS_PROGRESS"), new Vector2(0.5f, 0.195f), new Vector2(0.82f, 0.03f), 18, activeColor, TextAlignmentOptions.Center);
            AddButton(screen, LocalizationService.Get("SETTINGS_RESET_BUTTON"), new Vector2(0.5f, 0.145f), new Vector2(0.34f, 0.065f), specialColor, controller.ResetProgress);
            AddText(screen, LocalizationService.Format("SETTINGS_VERSION", Application.version), new Vector2(0.5f, 0.082f), new Vector2(0.72f, 0.028f), 15, new Color(0.78f, 0.82f, 0.94f), TextAlignmentOptions.Center);
            AddButton(screen, LocalizationService.Get("MENU_BACK"), new Vector2(0.5f, 0.0325f), new Vector2(0.32f, 0.055f), inactiveColor, () => ShowScreen(home, map, profile, settingsScreen));
        }

        private void BuildGameplay()
        {
            var runtime = gameObject.AddComponent<LevelRuntime>();
            var levelRootObject = new GameObject("LevelRoot");
            levelRootObject.transform.SetParent(transform, false);
            var board = gameObject.AddComponent<ProceduralPuzzleBoard>();
            board.Configure(runtime, levelRootObject.transform, gameplayCamera);
            BuildArenaPresentation();

            canvas = CreateCanvas("GameplayCanvas", true);
            var root = canvas.transform as RectTransform;
            var selectedLevel = Mathf.Clamp(PlayerPrefs.GetInt("il.selected_level", 1), 1, TotalLevels);
            var selectedEntry = LevelCatalogRuntime.All[selectedLevel - 1];

            var hudBar = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.96f), new Vector2(0.5f, 0.925f), new Vector2(1f, 0.15f));
            var levelLabel = AddTextRelative(hudBar, LocalizationService.Get("GAME_LEVEL_SHORT"), new Vector2(0.13f, 0.50f), new Vector2(0.22f, 0.62f), 26, Color.white, TextAlignmentOptions.Left);
            var starVisuals = new Image[3];
            for (var i = 0; i < starVisuals.Length; i++)
            {
                starVisuals[i] = AddImagePanelRelative(hudBar, ArtAssetLibrary.GetGameplaySprite("star_empty"), Color.white,
                    new Vector2(0.40f + i * 0.055f, 0.50f), new Vector2(0.045f, 0.54f), true);
            }
            var starsFallback = AddTextRelative(hudBar, StarString(0), new Vector2(0.49f, 0.50f), new Vector2(0.18f, 0.55f), 24, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Center);
            AddImagePanelRelative(hudBar, ArtAssetLibrary.GetGameplaySprite("coin"), Color.white,
                new Vector2(0.68f, 0.50f), new Vector2(0.055f, 0.60f), true);
            var coinLabel = AddTextRelative(hudBar, "0", new Vector2(0.77f, 0.50f), new Vector2(0.15f, 0.58f), 25, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Left);
            var pauseButton = AddGameplayIconButton(hudBar, LocalizationService.Get("GAME_PAUSE"), ArtAssetLibrary.GetGameplaySprite("pause"),
                new Vector2(0.92f, 0.50f), new Vector2(0.12f, 0.82f), new Color(0.13f, 0.18f, 0.34f), null);

            var objectivePanel = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.78f), new Vector2(0.5f, 0.795f), new Vector2(0.90f, 0.075f));
            AddTextRelative(objectivePanel, LocalizationService.GetLevelIdentity(selectedEntry.type, selectedEntry.difficulty), new Vector2(0.5f, 0.14f), new Vector2(0.94f, 0.22f), 12, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);
            var objectiveLabel = AddTextRelative(objectivePanel, LocalizationService.Get("GAME_OBJECTIVE"), new Vector2(0.5f, 0.59f), new Vector2(0.94f, 0.56f), 20, Color.white, TextAlignmentOptions.Center);
            var hintLabel = AddText(root, "", new Vector2(0.5f, 0.18f), new Vector2(0.88f, 0.055f), 19, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);

            if (selectedLevel == 1 && HookIntroController.ShouldShowFirstLevelTutorial)
            {
                BuildFirstLevelTutorial(root);
            }

            var pausePanel = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.98f), new Vector2(0.5f, 0.50f), new Vector2(0.72f, 0.42f));
            AddImagePanelRelative(pausePanel, ArtAssetLibrary.GetGameplaySprite("pause"), Color.white, new Vector2(0.20f, 0.70f), new Vector2(0.10f, 0.16f), true);
            AddTextRelative(pausePanel, LocalizationService.Get("GAME_PAUSE"), new Vector2(0.56f, 0.70f), new Vector2(0.70f, 0.16f), 42, Color.white, TextAlignmentOptions.Left);
            AddTextRelative(pausePanel, LocalizationService.Get("GAME_PAUSE_HINT"), new Vector2(0.50f, 0.53f), new Vector2(0.82f, 0.10f), 15, new Color(0.72f, 0.78f, 0.92f), TextAlignmentOptions.Center);
            AddPanelRelative(pausePanel, new Color(0.10f, 0.82f, 0.78f, 0.28f), new Vector2(0.50f, 0.46f), new Vector2(0.72f, 0.008f));
            var successPanel = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.98f), new Vector2(0.5f, 0.50f), new Vector2(0.84f, 0.64f));
            AddTextRelative(successPanel, LocalizationService.Get("GAME_COMPLETE"), new Vector2(0.5f, 0.91f), new Vector2(0.90f, 0.09f), 34, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);
            AddTextRelative(successPanel, LocalizationService.Get("GAME_COMPLETE_SUBTITLE"), new Vector2(0.5f, 0.845f), new Vector2(0.84f, 0.045f), 15, new Color(0.72f, 0.78f, 0.92f), TextAlignmentOptions.Center);
            AddPanelRelative(successPanel, new Color(0.08f, 0.10f, 0.19f, 0.92f), new Vector2(0.5f, 0.59f), new Vector2(0.88f, 0.45f));
            AddImagePanelRelative(successPanel, ArtAssetLibrary.GetGameplaySprite("star_filled"), Color.white, new Vector2(0.15f, 0.78f), new Vector2(0.08f, 0.10f), true);
            var completionStatsLabel = AddTextRelative(successPanel, LocalizationService.Get("GAME_STARS_THIS_RUN_UNAVAILABLE"), new Vector2(0.56f, 0.78f), new Vector2(0.68f, 0.09f), 21, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Left);
            AddImagePanelRelative(successPanel, ArtAssetLibrary.GetGameplaySprite("star_empty"), Color.white, new Vector2(0.15f, 0.67f), new Vector2(0.08f, 0.10f), true);
            var completionBestStarsLabel = AddTextRelative(successPanel, LocalizationService.Format("GAME_BEST_STARS", 0), new Vector2(0.56f, 0.67f), new Vector2(0.68f, 0.09f), 21, new Color(0.82f, 0.86f, 0.96f), TextAlignmentOptions.Left);
            AddImagePanelRelative(successPanel, ArtAssetLibrary.GetGameplaySprite("coin"), Color.white, new Vector2(0.15f, 0.56f), new Vector2(0.08f, 0.10f), true);
            var completionCoinsLabel = AddTextRelative(successPanel, LocalizationService.Get("GAME_COINS_THIS_COMPLETION_UNAVAILABLE"), new Vector2(0.56f, 0.56f), new Vector2(0.68f, 0.09f), 20, new Color(1f, 0.63f, 0.08f), TextAlignmentOptions.Left);
            AddImagePanelRelative(successPanel, ArtAssetLibrary.GetGameplaySprite("coin"), new Color(1f, 1f, 1f, 0.72f), new Vector2(0.15f, 0.45f), new Vector2(0.08f, 0.10f), true);
            var completionTotalCoinsLabel = AddTextRelative(successPanel, LocalizationService.Format("GAME_COINS_TOTAL", 0), new Vector2(0.56f, 0.45f), new Vector2(0.68f, 0.09f), 20, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Left);
            var completionProgressLabel = AddTextRelative(successPanel, LocalizationService.Format("GAME_PROGRESS_SUMMARY", 0, TotalLevels, 0, TotalLevels * 3), new Vector2(0.5f, 0.34f), new Vector2(0.84f, 0.08f), 15, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);
            var failPanel = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.98f), new Vector2(0.5f, 0.50f), new Vector2(0.78f, 0.42f));
            AddTextRelative(failPanel, LocalizationService.Get("GAME_FAILED"), new Vector2(0.5f, 0.73f), new Vector2(0.90f, 0.15f), 34, new Color(1f, 0.63f, 0.08f), TextAlignmentOptions.Center);
            AddTextRelative(failPanel, LocalizationService.Get("GAME_FAILURE_SUBTITLE"), new Vector2(0.5f, 0.55f), new Vector2(0.84f, 0.10f), 15, new Color(0.72f, 0.78f, 0.92f), TextAlignmentOptions.Center);
            AddPanelRelative(failPanel, new Color(1f, 0.63f, 0.08f, 0.26f), new Vector2(0.50f, 0.45f), new Vector2(0.72f, 0.008f));

            var retryButton = AddButtonRelative(failPanel, LocalizationService.Get("GAME_RETRY"), new Vector2(0.5f, 0.27f), new Vector2(0.58f, 0.18f), new Color(1f, 0.63f, 0.08f), null);
            var continueButton = AddButtonRelative(failPanel, LocalizationService.Get("GAME_CONTINUE"), new Vector2(0.5f, 0.06f), new Vector2(0.58f, 0.12f), new Color(0.10f, 0.82f, 0.78f), null);
            var hintButton = AddIconButton(root, LocalizationService.Get("GAME_HINT_BUTTON"), "hint", new Vector2(0.5f, 0.085f), new Vector2(0.32f, 0.095f), new Color(0.55f, 0.22f, 1f), board.UseHint);

            var router = gameObject.AddComponent<LevelCompletionRouter>();
            AddButtonRelative(successPanel, LocalizationService.Get("GAME_NEXT"), new Vector2(0.5f, 0.23f), new Vector2(0.68f, 0.10f), new Color(1f, 0.63f, 0.08f), router.LoadNextLevel);
            AddButtonRelative(successPanel, LocalizationService.Get("GAME_REPLAY"), new Vector2(0.30f, 0.115f), new Vector2(0.28f, 0.09f), new Color(0.10f, 0.82f, 0.78f), router.ReplayLevel);
            AddButtonRelative(successPanel, LocalizationService.Get("GAME_LEVEL_MAP"), new Vector2(0.70f, 0.115f), new Vector2(0.34f, 0.09f), new Color(0.55f, 0.22f, 1f), router.ReturnToLevelMap);
            AddButtonRelative(successPanel, LocalizationService.Get("GAME_MENU"), new Vector2(0.5f, 0.035f), new Vector2(0.42f, 0.045f), new Color(0.13f, 0.18f, 0.34f), router.ReturnToMenu);
            AddButtonRelative(pausePanel, LocalizationService.Get("GAME_RESUME"), new Vector2(0.5f, 0.36f), new Vector2(0.60f, 0.15f), new Color(0.10f, 0.82f, 0.78f), runtime.TogglePause);
            AddButtonRelative(pausePanel, LocalizationService.Get("GAME_RESTART"), new Vector2(0.5f, 0.19f), new Vector2(0.60f, 0.13f), new Color(1f, 0.63f, 0.08f), router.ReplayLevel);
            AddButtonRelative(pausePanel, LocalizationService.Get("GAME_SETTINGS"), new Vector2(0.30f, 0.055f), new Vector2(0.28f, 0.10f), new Color(0.55f, 0.22f, 1f), router.OpenSettings);
            AddButtonRelative(pausePanel, LocalizationService.Get("GAME_EXIT"), new Vector2(0.70f, 0.055f), new Vector2(0.38f, 0.10f), new Color(0.13f, 0.18f, 0.34f), router.ReturnToLevelMap);

            var hud = gameObject.AddComponent<GameHudController>();
            hud.Configure(runtime, pausePanel.gameObject, successPanel.gameObject, failPanel.gameObject, objectiveLabel, levelLabel, hintLabel, coinLabel,
                pauseButton, retryButton, continueButton, hintButton, starVisuals, starsFallback, completionStatsLabel, completionBestStarsLabel,
                completionCoinsLabel, completionTotalCoinsLabel, completionProgressLabel);
            var entry = LevelCatalogRuntime.All[selectedLevel - 1];
            hud.SetObjective(entry.objective, selectedLevel);
            hud.SetLevelHint(entry.hint);
            var progression = FindFirstObjectByType<ProgressionService>();
            if (progression != null) hud.SetCoins(progression.Coins);
            pausePanel.gameObject.SetActive(false);
            successPanel.gameObject.SetActive(false);
            failPanel.gameObject.SetActive(false);
        }

        private void BuildFirstLevelTutorial(RectTransform root)
        {
            var overlay = CreateScreen("FirstLevelTutorial", root);
            var canvasGroup = overlay.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            AddPanel(overlay, new Color(0.005f, 0.012f, 0.05f, 0.72f), new Vector2(0.5f, 0.665f), new Vector2(0.80f, 0.17f));
            AddPanel(overlay, new Color(0.10f, 0.82f, 0.78f, 0.85f), new Vector2(0.5f, 0.75f), new Vector2(0.50f, 0.004f));
            var card = AddPanel(overlay, new Color(0.06f, 0.08f, 0.16f, 0.98f), new Vector2(0.5f, 0.665f), new Vector2(0.76f, 0.14f));
            AddTextRelative(card, LocalizationService.Get("TUTORIAL_TITLE"), new Vector2(0.5f, 0.82f), new Vector2(0.74f, 0.20f), 20, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);
            var message = AddTextRelative(card, LocalizationService.Get("TUTORIAL_BODY"), new Vector2(0.5f, 0.51f), new Vector2(0.58f, 0.32f), 19, Color.white, TextAlignmentOptions.Center);
            var keyImage = AddImagePanelRelative(card, ArtAssetLibrary.GetGameplaySprite("key"), Color.white, new Vector2(0.14f, 0.52f), new Vector2(0.10f, 0.44f), true);
            var doorImage = AddImagePanelRelative(card, ArtAssetLibrary.GetGameplaySprite("door"), Color.white, new Vector2(0.86f, 0.52f), new Vector2(0.12f, 0.52f), true);
            AddTextRelative(card, LocalizationService.Get("TUTORIAL_KEY"), new Vector2(0.14f, 0.17f), new Vector2(0.22f, 0.18f), 12, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Center);
            AddTextRelative(card, LocalizationService.Get("TUTORIAL_DOOR"), new Vector2(0.86f, 0.17f), new Vector2(0.22f, 0.18f), 12, new Color(0.55f, 0.22f, 1f), TextAlignmentOptions.Center);

            var controller = overlay.gameObject.AddComponent<HookIntroController>();
            controller.ConfigureRuntime(canvasGroup, keyImage.rectTransform, doorImage.rectTransform, message);
        }

        private void BuildArenaPresentation()
        {
            var arenaObject = new GameObject("ArenaPresentation");
            arenaObject.transform.SetParent(transform, false);

            // Presentation-only depth layers. Gameplay nodes remain owned by ProceduralPuzzleBoard.
            AddArenaSprite(arenaObject.transform, "ArenaBackdrop", "floor", new Vector2(0f, -0.55f),
                new Vector2(8.35f, 12.10f), new Color(1f, 1f, 1f, 0.16f), -12, 0f);
            AddArenaSprite(arenaObject.transform, "ArenaLeftRail", "toprail", new Vector2(-4.00f, -0.55f),
                new Vector2(0.24f, 11.15f), new Color(0.10f, 0.82f, 0.78f, 0.42f), -11, 90f);
            AddArenaSprite(arenaObject.transform, "ArenaRightRail", "toprail", new Vector2(4.00f, -0.55f),
                new Vector2(0.24f, 11.15f), new Color(0.55f, 0.22f, 1f, 0.34f), -11, 90f);
            AddArenaSprite(arenaObject.transform, "ArenaTopAccent", "toprail", new Vector2(0f, 5.28f),
                new Vector2(7.55f, 0.20f), new Color(0.10f, 0.82f, 0.78f, 0.26f), -10, 0f);
            AddArenaSprite(arenaObject.transform, "ArenaBottomAccent", "floor", new Vector2(0f, -6.12f),
                new Vector2(7.70f, 0.24f), new Color(1f, 0.63f, 0.08f, 0.34f), -10, 0f);
        }

        private static void AddArenaSprite(Transform parent, string name, string assetName, Vector2 position,
            Vector2 scale, Color tint, int sortingOrder, float rotation)
        {
            var sprite = ArtAssetLibrary.GetGameplaySprite(assetName);
            if (sprite == null) return;

            var objectInstance = new GameObject(name);
            objectInstance.transform.SetParent(parent, false);
            objectInstance.transform.position = new Vector3(position.x, position.y, 1f);
            objectInstance.transform.localScale = new Vector3(scale.x, scale.y, 1f);
            objectInstance.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
            var renderer = objectInstance.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = tint;
            renderer.sortingOrder = sortingOrder;
        }

        private Canvas CreateCanvas(string name, bool applySafeArea = false)
        {
            var canvasObject = new GameObject(name);
            canvasObject.transform.SetParent(transform, false);
            var createdCanvas = canvasObject.AddComponent<Canvas>();
            createdCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();
            if (applySafeArea) canvasObject.AddComponent<SafeAreaFitter>();
            return createdCanvas;
        }

        private static RectTransform CreateScreen(string name, RectTransform parent)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        private static void ShowScreen(GameObject active, params GameObject[] others)
        {
            if (active != null) active.SetActive(true);
            foreach (var other in others)
            {
                if (other != null && other != active) other.SetActive(false);
            }
        }

        private static RectTransform AddPanel(RectTransform parent, Color color, Vector2 anchor, Vector2 size)
        {
            var obj = new GameObject("Panel");
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            SetNormalizedRect(rect, anchor, size);
            var image = obj.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return rect;
        }

        private static Image AddImagePanel(Transform parent, Sprite sprite, Color color, Vector2 anchor, Vector2 size, bool preserveAspect)
        {
            var obj = new GameObject("ArtImage");
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            SetNormalizedRect(rect, anchor, size);
            var image = obj.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.preserveAspect = preserveAspect;
            image.raycastTarget = false;
            return image;
        }

        private static RectTransform AddPanelRelative(RectTransform parent, Color color, Vector2 anchor, Vector2 size)
        {
            var obj = new GameObject("Panel");
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            SetNormalizedRect(rect, anchor, size);
            var image = obj.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return rect;
        }

        private static Image AddImagePanelRelative(RectTransform parent, Sprite sprite, Color color, Vector2 anchor, Vector2 size, bool preserveAspect)
        {
            var obj = new GameObject("ArtImage");
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            SetNormalizedRect(rect, anchor, size);
            var image = obj.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.preserveAspect = preserveAspect;
            image.raycastTarget = false;
            return image;
        }

        private static TMP_Text AddText(Transform parent, string value, Vector2 anchor, Vector2 size, float fontSize, Color color, TextAlignmentOptions alignment)
        {
            var obj = new GameObject("Text");
            obj.SetActive(false);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            SetNormalizedRect(rect, anchor, size);
            var text = obj.AddComponent<TextMeshProUGUI>();
            text.font = ResolveFontAsset();
            text.text = value;
            ConfigureText(text, fontSize, color, alignment);
            obj.SetActive(true);
            return text;
        }

        private static TMP_Text AddTextRelative(RectTransform parent, string value, Vector2 anchor, Vector2 size, float fontSize, Color color, TextAlignmentOptions alignment)
        {
            var obj = new GameObject("Text");
            obj.SetActive(false);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            SetNormalizedRect(rect, anchor, size);
            var text = obj.AddComponent<TextMeshProUGUI>();
            text.font = ResolveFontAsset();
            text.text = value;
            ConfigureText(text, fontSize, color, alignment);
            obj.SetActive(true);
            return text;
        }

        private static TMP_FontAsset ResolveFontAsset()
        {
            if (runtimeFontAsset != null) return runtimeFontAsset;
            if (TMP_Settings.instance != null) runtimeFontAsset = TMP_Settings.defaultFontAsset;
            if (runtimeFontAsset != null) return runtimeFontAsset;

            var builtInFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (builtInFont == null) return null;
            try
            {
                runtimeFontAsset = TMP_FontAsset.CreateFontAsset(builtInFont);
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning("IMPOSSIBLE LEVELS could not create a runtime TMP font: " + exception.Message);
            }
            return runtimeFontAsset;
        }

        private static Button AddButton(Transform parent, string label, Vector2 anchor, Vector2 size, Color color, UnityEngine.Events.UnityAction action)
        {
            var obj = new GameObject("Button_" + label.Replace(" ", "_"));
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            size.y = Mathf.Max(size.y, 0.055f);
            SetNormalizedRect(rect, anchor, size);
            var image = obj.AddComponent<Image>();
            image.color = color;
            AddButtonSurfaceEffects(obj, color);
            var button = obj.AddComponent<Button>();
            button.targetGraphic = image;
            ConfigureButton(button, color);
            if (action != null) button.onClick.AddListener(action);
            var motion = obj.AddComponent<MotionFeedback>();
            button.onClick.AddListener(motion.Press);
            AddTextRelative(rect, label, new Vector2(0.5f, 0.5f), Vector2.one, 27, Color.white, TextAlignmentOptions.Center);
            return button;
        }

        private static Button AddButtonRelative(RectTransform parent, string label, Vector2 anchor, Vector2 size, Color color, UnityEngine.Events.UnityAction action)
        {
            var obj = new GameObject("Button_" + label.Replace(" ", "_"));
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            size.y = Mathf.Max(size.y, 0.055f);
            SetNormalizedRect(rect, anchor, size);
            var image = obj.AddComponent<Image>();
            image.color = color;
            AddButtonSurfaceEffects(obj, color);
            var button = obj.AddComponent<Button>();
            button.targetGraphic = image;
            ConfigureButton(button, color);
            if (action != null) button.onClick.AddListener(action);
            var motion = obj.AddComponent<MotionFeedback>();
            button.onClick.AddListener(motion.Press);
            AddTextRelative(rect, label, new Vector2(0.5f, 0.5f), Vector2.one, 27, Color.white, TextAlignmentOptions.Center);
            return button;
        }

        private static void AddButtonSurfaceEffects(GameObject obj, Color color)
        {
            var shadow = obj.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.38f);
            shadow.effectDistance = new Vector2(0f, -5f);
            shadow.useGraphicAlpha = true;
            var outline = obj.AddComponent<Outline>();
            outline.effectColor = new Color(0.72f, 0.86f, 1f, Mathf.Clamp01(0.16f + color.a * 0.14f));
            outline.effectDistance = new Vector2(2f, 2f);
        }

        private static void AddTextSurfaceEffect(TMP_Text text, Color effectColor, Vector2 distance)
        {
            if (text == null) return;
            var shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = effectColor;
            shadow.effectDistance = distance;
            shadow.useGraphicAlpha = true;
        }

        private static Button AddMenuIconButton(Transform parent, string label, string iconName, Vector2 anchor, Vector2 size, Color color, UnityEngine.Events.UnityAction action, bool primary)
        {
            var button = AddIconButton(parent, label, iconName, anchor, size, color, action);
            var labelText = button.GetComponentInChildren<TMP_Text>();
            if (primary && labelText != null)
            {
                labelText.fontSize = 22f;
                labelText.fontStyle = FontStyles.Bold;
            }
            return button;
        }

        private static Button AddIconButton(Transform parent, string label, string iconName, Vector2 anchor, Vector2 size, Color color, UnityEngine.Events.UnityAction action)
        {
            var button = AddButton(parent, label, anchor, size, color, action);
            var rect = button.transform as RectTransform;
            var icon = AddImagePanelRelative(rect, ArtAssetLibrary.GetUiIcon(iconName), Color.white, new Vector2(0.5f, 0.64f), new Vector2(0.42f, 0.42f), true);
            icon.raycastTarget = false;
            var labelText = rect.GetComponentInChildren<TMP_Text>();
            if (labelText != null)
            {
                labelText.rectTransform.anchorMin = new Vector2(0.04f, 0.035f);
                labelText.rectTransform.anchorMax = new Vector2(0.96f, 0.34f);
                labelText.rectTransform.sizeDelta = Vector2.zero;
                labelText.margin = new Vector4(8f, 4f, 8f, 4f);
                labelText.fontSize = Mathf.Max(17f, labelText.fontSize * 0.72f);
            }
            return button;
        }

        private static Button AddGameplayIconButton(Transform parent, string label, Sprite sprite, Vector2 anchor, Vector2 size, Color color, UnityEngine.Events.UnityAction action)
        {
            var rectParent = parent as RectTransform;
            var button = rectParent != null
                ? AddButtonRelative(rectParent, label, anchor, size, color, action)
                : AddButton(parent, label, anchor, size, color, action);
            var rect = button.transform as RectTransform;
            var icon = AddImagePanelRelative(rect, sprite, Color.white, new Vector2(0.5f, 0.64f), new Vector2(0.46f, 0.46f), true);
            icon.raycastTarget = false;
            var labelText = rect.GetComponentInChildren<TMP_Text>();
            if (labelText != null)
            {
                labelText.rectTransform.anchorMin = new Vector2(0.04f, 0.035f);
                labelText.rectTransform.anchorMax = new Vector2(0.96f, 0.34f);
                labelText.rectTransform.sizeDelta = Vector2.zero;
                labelText.margin = new Vector4(6f, 4f, 6f, 4f);
                labelText.fontSize = Mathf.Max(13f, labelText.fontSize * 0.58f);
            }
            return button;
        }

        private static ScrollRect CreateScrollView(RectTransform parent, Vector2 anchor, Vector2 size)
        {
            var scrollObject = new GameObject("LevelMapScroll");
            scrollObject.transform.SetParent(parent, false);
            var scrollRectTransform = scrollObject.AddComponent<RectTransform>();
            SetNormalizedRect(scrollRectTransform, anchor, size);

            var scroll = scrollObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 34f;
            scroll.inertia = true;
            scroll.decelerationRate = 0.135f;

            var viewportObject = new GameObject("Viewport");
            viewportObject.transform.SetParent(scrollObject.transform, false);
            var viewport = viewportObject.AddComponent<RectTransform>();
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.offsetMin = Vector2.zero;
            viewport.offsetMax = Vector2.zero;
            var viewportImage = viewportObject.AddComponent<Image>();
            viewportImage.color = new Color(0f, 0f, 0f, 0.12f);
            viewportImage.raycastTarget = true;
            var mask = viewportObject.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            var contentObject = new GameObject("Content");
            contentObject.transform.SetParent(viewportObject.transform, false);
            var content = contentObject.AddComponent<RectTransform>();
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(0f, 0f);
            var layout = contentObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(456f, 300f);
            layout.spacing = new Vector2(18f, 18f);
            layout.padding = new RectOffset(18, 18, 18, 24);
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 2;
            var fitter = contentObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.viewport = viewport;
            scroll.content = content;
            scroll.verticalNormalizedPosition = 1f;
            return scroll;
        }

        private static void AddProgressionPath(RectTransform content, LevelMapController mapController)
        {
            var pathObject = new GameObject("ProgressionPath");
            pathObject.transform.SetParent(content, false);
            var pathRect = pathObject.AddComponent<RectTransform>();
            pathRect.anchorMin = new Vector2(0f, 1f);
            pathRect.anchorMax = new Vector2(1f, 1f);
            pathRect.pivot = new Vector2(0.5f, 1f);
            pathRect.anchoredPosition = Vector2.zero;
            pathRect.sizeDelta = Vector2.zero;
            var ignoredLayout = pathObject.AddComponent<LayoutElement>();
            ignoredLayout.ignoreLayout = true;

            const float contentHeight = 4794f;
            const float firstRowCenter = 1f - 168f / contentHeight;
            const float rowStep = 318f / contentHeight;
            for (var row = 0; row < 15; row++)
            {
                var y = firstRowCenter - row * rowStep;
                var horizontalTarget = row * 2 + 2;
                AddMapConnector(pathRect, new Vector2(0.5f, y), new Vector2(0.52f, 0.008f), MapConnectorColor(mapController, horizontalTarget));
                if (row >= 14) continue;
                var nextY = firstRowCenter - (row + 1) * rowStep;
                var verticalTarget = row * 2 + 3;
                var x = row % 2 == 0 ? 0.75f : 0.25f;
                AddMapConnector(pathRect, new Vector2(x, (y + nextY) * 0.5f), new Vector2(0.008f, rowStep), MapConnectorColor(mapController, verticalTarget));
            }
        }

        private static Color MapConnectorColor(LevelMapController mapController, int targetLevel)
        {
            if (targetLevel <= TotalLevels && mapController.GetLevelStars(targetLevel) > 0)
            {
                return new Color(1f, 0.78f, 0.24f, 0.76f);
            }
            if (targetLevel <= TotalLevels && mapController.IsLevelUnlocked(targetLevel))
            {
                return new Color(0.10f, 0.82f, 0.78f, 0.62f);
            }
            return new Color(0.28f, 0.36f, 0.56f, 0.42f);
        }

        private static void AddMapConnector(RectTransform parent, Vector2 anchor, Vector2 size, Color color)
        {
            AddPanelRelative(parent, color, anchor, size);
        }

        private static void AddLevelCard(RectTransform content, RuntimeLevelEntry entry, LevelMapController mapController, int currentLevel, UnityEngine.Events.UnityAction action)
        {
            var cardObject = new GameObject("LevelCard_" + entry.index.ToString("00"));
            cardObject.transform.SetParent(content, false);
            var card = cardObject.AddComponent<RectTransform>();
            card.sizeDelta = new Vector2(456f, 300f);
            var unlocked = mapController.IsLevelUnlocked(entry.index);
            var isCurrent = entry.index == currentLevel;
            var earnedStars = mapController.GetLevelStars(entry.index);
            var isCompleted = earnedStars > 0;
            var cardImage = cardObject.AddComponent<Image>();
            cardImage.color = isCurrent
                ? new Color(0.18f, 0.25f, 0.46f, 0.99f)
                : unlocked ? new Color(0.13f, 0.18f, 0.34f, 0.98f) : new Color(0.08f, 0.10f, 0.19f, 0.98f);
            var cardShadow = cardObject.AddComponent<Shadow>();
            cardShadow.effectColor = new Color(0f, 0f, 0f, 0.42f);
            cardShadow.effectDistance = new Vector2(0f, -6f);
            cardShadow.useGraphicAlpha = true;
            var cardOutline = cardObject.AddComponent<Outline>();
            cardOutline.effectColor = isCurrent ? new Color(1f, 0.78f, 0.24f, 0.95f) : new Color(0.36f, 0.68f, 0.92f, unlocked ? 0.42f : 0.20f);
            cardOutline.effectDistance = isCurrent ? new Vector2(5f, 5f) : new Vector2(2f, 2f);
            var accent = LevelAccentColor(entry.index);
            AddPanelRelative(card, new Color(accent.r, accent.g, accent.b, unlocked ? 0.88f : 0.38f), new Vector2(0.5f, 0.985f), new Vector2(0.84f, 0.018f));

            var button = cardObject.AddComponent<Button>();
            button.targetGraphic = cardImage;
            ConfigureButton(button, cardImage.color);
            button.interactable = unlocked;
            if (unlocked) button.onClick.AddListener(action);
            var motion = cardObject.AddComponent<MotionFeedback>();
            if (unlocked) button.onClick.AddListener(motion.Press);

            var thumbFrame = AddPanelRelative(card, new Color(accent.r, accent.g, accent.b, unlocked ? 0.38f : 0.22f), new Vector2(0.5f, 0.62f), new Vector2(0.94f, 0.62f));
            var frameOutline = thumbFrame.gameObject.AddComponent<Outline>();
            frameOutline.effectColor = new Color(0.72f, 0.86f, 1f, unlocked ? 0.42f : 0.24f);
            frameOutline.effectDistance = new Vector2(2f, 2f);
            var thumb = AddImagePanelRelative(card, ArtAssetLibrary.GetLevelThumbnail(entry.index), unlocked ? Color.white : new Color(0.48f, 0.50f, 0.60f, 1f), new Vector2(0.5f, 0.62f), new Vector2(0.90f, 0.58f), true);
            thumb.raycastTarget = false;
            AddTextRelative(card, entry.index.ToString("00"), new Vector2(0.13f, 0.92f), new Vector2(0.20f, 0.12f), 25, Color.white, TextAlignmentOptions.Center);
            AddLevelStars(card, earnedStars);
            AddPanelRelative(card, new Color(accent.r, accent.g, accent.b, unlocked ? 0.28f : 0.20f), new Vector2(0.5f, 0.34f), new Vector2(0.78f, 0.072f));
            AddTextRelative(card, LocalizationService.GetLevelIdentity(entry.type, entry.difficulty), new Vector2(0.5f, 0.34f), new Vector2(0.74f, 0.064f), 11, new Color(0.88f, 0.94f, 1f), TextAlignmentOptions.Center);
            AddTextRelative(card, LocalizationService.GetLevelTitle(entry.index, entry.title), new Vector2(0.5f, 0.12f), new Vector2(0.90f, 0.13f), 16, Color.white, TextAlignmentOptions.Center);
            if (isCurrent)
            {
                AddPanelRelative(card, new Color(1f, 0.78f, 0.24f, 0.92f), new Vector2(0.77f, 0.235f), new Vector2(0.28f, 0.065f));
                AddTextRelative(card, LocalizationService.Get("LEVEL_CURRENT"), new Vector2(0.77f, 0.235f), new Vector2(0.26f, 0.058f), 12, new Color(0.035f, 0.055f, 0.14f), TextAlignmentOptions.Center);
            }
            else if (isCompleted)
            {
                AddPanelRelative(card, new Color(0.10f, 0.82f, 0.78f, 0.84f), new Vector2(0.77f, 0.235f), new Vector2(0.30f, 0.065f));
                AddTextRelative(card, LocalizationService.Get("LEVEL_COMPLETED"), new Vector2(0.77f, 0.235f), new Vector2(0.28f, 0.058f), 11, new Color(0.035f, 0.055f, 0.14f), TextAlignmentOptions.Center);
            }
            if (!unlocked)
            {
                AddPanelRelative(card, new Color(0.01f, 0.02f, 0.07f, 0.58f), new Vector2(0.5f, 0.62f), new Vector2(0.90f, 0.58f));
                AddTextRelative(card, LocalizationService.Get("LEVEL_LOCKED"), new Vector2(0.5f, 0.60f), new Vector2(0.8f, 0.14f), 22, Color.white, TextAlignmentOptions.Center);
            }
        }

        private static void AddLevelStars(RectTransform card, int stars)
        {
            var filled = ArtAssetLibrary.GetGameplaySprite("star_filled");
            var empty = ArtAssetLibrary.GetGameplaySprite("star_empty");
            if (filled == null || empty == null)
            {
                AddTextRelative(card, StarString(stars), new Vector2(0.72f, 0.92f), new Vector2(0.48f, 0.12f), 22, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Center);
                return;
            }

            for (var i = 0; i < 3; i++)
            {
                var star = AddImagePanelRelative(card, i < stars ? filled : empty, Color.white,
                    new Vector2(0.62f + i * 0.075f, 0.92f), new Vector2(0.060f, 0.115f), true);
                star.raycastTarget = false;
            }
        }

        private static void SetNormalizedRect(RectTransform rect, Vector2 center, Vector2 size)
        {
            if (center == Vector2.zero && size == Vector2.one)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
            }
            else
            {
                var half = size * 0.5f;
                rect.anchorMin = new Vector2(Mathf.Clamp01(center.x - half.x), Mathf.Clamp01(center.y - half.y));
                rect.anchorMax = new Vector2(Mathf.Clamp01(center.x + half.x), Mathf.Clamp01(center.y + half.y));
            }
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void ConfigureText(TMP_Text text, float fontSize, Color color, TextAlignmentOptions alignment)
        {
            text.fontSize = fontSize;
            text.fontSizeMin = Mathf.Max(12f, fontSize * 0.62f);
            text.fontSizeMax = fontSize;
            text.enableAutoSizing = true;
            text.color = color;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Ellipsis;
            LocalizationService.ApplyTo(text);
        }

        private static void ConfigureButton(Button button, Color color)
        {
            var colors = button.colors;
            colors.normalColor = color;
            colors.highlightedColor = Color.Lerp(color, Color.white, 0.12f);
            colors.pressedColor = Color.Lerp(color, Color.black, 0.16f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = Color.Lerp(color, new Color(0.035f, 0.055f, 0.14f, 1f), 0.42f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.06f;
            button.colors = colors;
        }

        private static Color LevelAccentColor(int levelIndex)
        {
            var hue = Mathf.Repeat((levelIndex - 1) * 0.1375f, 1f);
            return Color.HSVToRGB(hue, 0.62f, 0.92f);
        }

        private static string StarString(int stars)
        {
            var value = string.Empty;
            for (var i = 0; i < 3; i++) value += i < stars ? "★" : "☆";
            return value;
        }
    }
}
