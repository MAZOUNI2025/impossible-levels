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
        private const int TotalLevels = 30;
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
            gameplayCamera.orthographicSize = 7.1f;
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

            AddImagePanel(mainScreen, ArtAssetLibrary.GetLevelThumbnail(1), new Color(1f, 1f, 1f, 0.20f), Vector2.zero, Vector2.one, false);
            AddImagePanel(mainScreen, ArtAssetLibrary.GetLevelThumbnail(12), new Color(1f, 1f, 1f, 0.12f), new Vector2(0.18f, 0.56f), new Vector2(0.30f, 0.24f), true);
            AddImagePanel(mainScreen, ArtAssetLibrary.GetLevelThumbnail(24), new Color(1f, 1f, 1f, 0.10f), new Vector2(0.82f, 0.58f), new Vector2(0.30f, 0.24f), true);
            AddPanel(mainScreen, new Color(0.035f, 0.055f, 0.14f, 0.78f), Vector2.zero, Vector2.one);
            AddText(mainScreen, "IMPOSSIBLE LEVELS", new Vector2(0.5f, 0.82f), new Vector2(0.92f, 0.09f), 54, Color.white, TextAlignmentOptions.Center);
            AddText(mainScreen, "Looks easy. Think again.", new Vector2(0.5f, 0.735f), new Vector2(0.86f, 0.05f), 25, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);
            AddText(mainScreen, "30 puzzles. One rule: question everything.", new Vector2(0.5f, 0.685f), new Vector2(0.86f, 0.04f), 17, new Color(0.78f, 0.82f, 0.94f), TextAlignmentOptions.Center);

            var menu = gameObject.AddComponent<MainMenuController>();
            AddIconButton(mainScreen, "PLAY", "play", new Vector2(0.5f, 0.52f), new Vector2(0.34f, 0.14f), new Color(1f, 0.63f, 0.08f), menu.StartFirstLevel);
            AddIconButton(mainScreen, "LEVEL MAP", "levels", new Vector2(0.5f, 0.36f), new Vector2(0.34f, 0.14f), new Color(0.10f, 0.82f, 0.78f), () => ShowScreen(mapScreen.gameObject, mainScreen.gameObject, profileScreen.gameObject, settingsScreen.gameObject));
            AddIconButton(mainScreen, "PLAYER", "player", new Vector2(0.29f, 0.18f), new Vector2(0.26f, 0.13f), new Color(0.55f, 0.22f, 1f), () => ShowScreen(profileScreen.gameObject, mainScreen.gameObject, mapScreen.gameObject, settingsScreen.gameObject));
            AddIconButton(mainScreen, "SETTINGS", "settings", new Vector2(0.71f, 0.18f), new Vector2(0.26f, 0.13f), new Color(0.13f, 0.18f, 0.34f), () => ShowScreen(settingsScreen.gameObject, mainScreen.gameObject, mapScreen.gameObject, profileScreen.gameObject));
            AddText(mainScreen, "13+  |  30 deterministic puzzles  |  Offline progression", new Vector2(0.5f, 0.055f), new Vector2(0.92f, 0.035f), 17, new Color(0.7f, 0.75f, 0.88f), TextAlignmentOptions.Center);

            var mapController = gameObject.AddComponent<LevelMapController>();
            BuildLevelMap(mapScreen, mapController, mainScreen.gameObject, mapScreen.gameObject, profileScreen.gameObject, settingsScreen.gameObject);
            BuildProfile(profileScreen, mainScreen.gameObject, mapScreen.gameObject, profileScreen.gameObject, settingsScreen.gameObject);
            BuildSettings(settingsScreen, mainScreen.gameObject, mapScreen.gameObject, profileScreen.gameObject, settingsScreen.gameObject);
            ShowScreen(mainScreen.gameObject, mapScreen.gameObject, profileScreen.gameObject, settingsScreen.gameObject);
        }

        private void BuildLevelMap(RectTransform screen, LevelMapController mapController, GameObject home, GameObject map, GameObject profile, GameObject settings)
        {
            AddPanel(screen, new Color(0.035f, 0.055f, 0.14f, 0.98f), Vector2.zero, Vector2.one);
            AddPanel(screen, new Color(0.10f, 0.82f, 0.78f, 0.10f), new Vector2(0.5f, 0.935f), new Vector2(0.92f, 0.12f));
            AddText(screen, "LEVEL MAP", new Vector2(0.5f, 0.952f), new Vector2(0.80f, 0.055f), 40, Color.white, TextAlignmentOptions.Center);
            AddText(screen, "Choose a challenge. Complete it to unlock the next.", new Vector2(0.5f, 0.895f), new Vector2(0.88f, 0.035f), 16, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);

            var selectedLevel = Mathf.Clamp(PlayerPrefs.GetInt("il.selected_level", 1), 1, TotalLevels);
            var currentLabel = AddText(screen, "CURRENT LEVEL  " + selectedLevel.ToString("00"), new Vector2(0.5f, 0.845f), new Vector2(0.78f, 0.035f), 17, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Center);
            currentLabel.raycastTarget = false;

            var scroll = CreateScrollView(screen, new Vector2(0.5f, 0.485f), new Vector2(0.90f, 0.67f));
            var content = scroll.content;
            for (var levelIndex = 1; levelIndex <= TotalLevels; levelIndex++)
            {
                var capturedLevel = levelIndex;
                var entry = LevelCatalogRuntime.All[levelIndex - 1];
                AddLevelCard(content, entry, mapController, () => mapController.SelectLevel(capturedLevel));
            }

            AddButton(screen, "BACK", new Vector2(0.5f, 0.065f), new Vector2(0.32f, 0.07f), new Color(0.13f, 0.18f, 0.34f), () => ShowScreen(home, map, profile, settings));
        }

        private void BuildProfile(RectTransform screen, GameObject home, GameObject map, GameObject profile, GameObject settings)
        {
            AddPanel(screen, new Color(0.035f, 0.055f, 0.14f, 0.98f), Vector2.zero, Vector2.one);
            AddText(screen, "PLAYER PROFILE", new Vector2(0.5f, 0.90f), new Vector2(0.84f, 0.08f), 43, Color.white, TextAlignmentOptions.Center);
            AddImagePanel(screen, ArtAssetLibrary.GetUiIcon("player"), Color.white, new Vector2(0.5f, 0.72f), new Vector2(0.22f, 0.13f), true);

            var progression = FindFirstObjectByType<ProgressionService>();
            var profileService = FindFirstObjectByType<PlayerProfileService>();
            var completed = profileService != null ? profileService.CompletedLevels : 0;
            var stars = profileService != null ? profileService.TotalStars : 0;
            var coins = progression != null ? progression.Coins : 0;
            var progress = Mathf.Clamp01(completed / (float)TotalLevels);

            AddText(screen, "PROGRESS", new Vector2(0.5f, 0.57f), new Vector2(0.76f, 0.045f), 19, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);
            AddPanel(screen, new Color(0.08f, 0.10f, 0.19f, 1f), new Vector2(0.5f, 0.515f), new Vector2(0.72f, 0.028f));
            if (progress > 0f)
            {
                AddPanel(screen, new Color(0.10f, 0.82f, 0.78f, 1f), new Vector2(progress * 0.36f, 0.515f), new Vector2(0.72f * progress, 0.028f));
            }
            AddText(screen, "COMPLETED   " + completed + " / 30", new Vector2(0.5f, 0.45f), new Vector2(0.75f, 0.055f), 25, Color.white, TextAlignmentOptions.Center);
            AddImagePanel(screen, ArtAssetLibrary.GetGameplaySprite("star_filled"), Color.white, new Vector2(0.30f, 0.36f), new Vector2(0.065f, 0.052f), true);
            AddText(screen, stars + " / 90", new Vector2(0.45f, 0.36f), new Vector2(0.26f, 0.055f), 25, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Left);
            AddImagePanel(screen, ArtAssetLibrary.GetGameplaySprite("coin"), Color.white, new Vector2(0.58f, 0.36f), new Vector2(0.065f, 0.052f), true);
            AddText(screen, coins.ToString(), new Vector2(0.72f, 0.36f), new Vector2(0.20f, 0.055f), 25, new Color(1f, 0.63f, 0.08f), TextAlignmentOptions.Left);
            AddText(screen, "Keep solving. The board remembers your best stars.", new Vector2(0.5f, 0.26f), new Vector2(0.86f, 0.05f), 18, new Color(0.72f, 0.78f, 0.92f), TextAlignmentOptions.Center);
            AddButton(screen, "BACK", new Vector2(0.5f, 0.10f), new Vector2(0.32f, 0.07f), new Color(0.13f, 0.18f, 0.34f), () => ShowScreen(home, map, profile, settings));
        }

        private void BuildSettings(RectTransform screen, GameObject home, GameObject map, GameObject profile, GameObject settingsScreen)
        {
            AddPanel(screen, new Color(0.035f, 0.055f, 0.14f, 0.98f), Vector2.zero, Vector2.one);
            AddText(screen, "SETTINGS", new Vector2(0.5f, 0.90f), new Vector2(0.8f, 0.08f), 46, Color.white, TextAlignmentOptions.Center);
            AddImagePanel(screen, ArtAssetLibrary.GetUiIcon("settings"), Color.white, new Vector2(0.5f, 0.72f), new Vector2(0.20f, 0.12f), true);

            var controller = gameObject.AddComponent<SettingsController>();
            var profileService = FindFirstObjectByType<PlayerProfileService>();
            var musicEnabled = profileService == null || profileService.MusicEnabled;
            var sfxEnabled = profileService == null || profileService.SfxEnabled;
            var hapticsEnabled = profileService == null || profileService.HapticsEnabled;
            var activeColor = new Color(0.10f, 0.82f, 0.78f);
            var inactiveColor = new Color(0.13f, 0.18f, 0.34f);

            AddText(screen, "AUDIO & FEEDBACK", new Vector2(0.5f, 0.56f), new Vector2(0.82f, 0.05f), 22, activeColor, TextAlignmentOptions.Center);
            var musicState = AddText(screen, "MUSIC  " + (musicEnabled ? "ON" : "OFF"), new Vector2(0.16f, 0.47f), new Vector2(0.20f, 0.05f), 18, Color.white, TextAlignmentOptions.Left);
            AddButton(screen, "ON", new Vector2(0.43f, 0.47f), new Vector2(0.20f, 0.08f), musicEnabled ? activeColor : inactiveColor, () => { controller.SetMusic(true); musicState.text = "MUSIC  ON"; });
            AddButton(screen, "OFF", new Vector2(0.70f, 0.47f), new Vector2(0.20f, 0.08f), musicEnabled ? inactiveColor : new Color(0.55f, 0.22f, 1f), () => { controller.SetMusic(false); musicState.text = "MUSIC  OFF"; });
            var sfxState = AddText(screen, "SFX  " + (sfxEnabled ? "ON" : "OFF"), new Vector2(0.16f, 0.37f), new Vector2(0.20f, 0.05f), 18, Color.white, TextAlignmentOptions.Left);
            AddButton(screen, "ON", new Vector2(0.43f, 0.37f), new Vector2(0.20f, 0.08f), sfxEnabled ? activeColor : inactiveColor, () => { controller.SetSfx(true); sfxState.text = "SFX  ON"; });
            AddButton(screen, "OFF", new Vector2(0.70f, 0.37f), new Vector2(0.20f, 0.08f), sfxEnabled ? inactiveColor : new Color(0.55f, 0.22f, 1f), () => { controller.SetSfx(false); sfxState.text = "SFX  OFF"; });
            var hapticsState = AddText(screen, "HAPTICS  " + (hapticsEnabled ? "ON" : "OFF"), new Vector2(0.16f, 0.27f), new Vector2(0.22f, 0.05f), 18, Color.white, TextAlignmentOptions.Left);
            AddButton(screen, "ON", new Vector2(0.43f, 0.27f), new Vector2(0.20f, 0.08f), hapticsEnabled ? activeColor : inactiveColor, () => { controller.SetHaptics(true); hapticsState.text = "HAPTICS  ON"; });
            AddButton(screen, "OFF", new Vector2(0.70f, 0.27f), new Vector2(0.20f, 0.08f), hapticsEnabled ? inactiveColor : new Color(0.55f, 0.22f, 1f), () => { controller.SetHaptics(false); hapticsState.text = "HAPTICS  OFF"; });
            AddText(screen, "RESET LOCAL PROGRESS", new Vector2(0.5f, 0.18f), new Vector2(0.8f, 0.04f), 18, new Color(0.78f, 0.82f, 0.94f), TextAlignmentOptions.Center);
            AddButton(screen, "RESET", new Vector2(0.5f, 0.125f), new Vector2(0.30f, 0.065f), new Color(0.55f, 0.22f, 1f), controller.ResetProgress);
            AddButton(screen, "BACK", new Vector2(0.5f, 0.055f), new Vector2(0.32f, 0.06f), inactiveColor, () => ShowScreen(home, map, profile, settingsScreen));
        }

        private void BuildGameplay()
        {
            var runtime = gameObject.AddComponent<LevelRuntime>();
            var levelRootObject = new GameObject("LevelRoot");
            levelRootObject.transform.SetParent(transform, false);
            var board = gameObject.AddComponent<ProceduralPuzzleBoard>();
            board.Configure(runtime, levelRootObject.transform, gameplayCamera);

            canvas = CreateCanvas("GameplayCanvas", true);
            var root = canvas.transform as RectTransform;
            var selectedLevel = Mathf.Clamp(PlayerPrefs.GetInt("il.selected_level", 1), 1, TotalLevels);

            var hudBar = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.96f), new Vector2(0.5f, 0.925f), new Vector2(1f, 0.15f));
            var levelLabel = AddTextRelative(hudBar, "LEVEL", new Vector2(0.13f, 0.50f), new Vector2(0.22f, 0.62f), 26, Color.white, TextAlignmentOptions.Left);
            var starVisuals = new Image[3];
            for (var i = 0; i < starVisuals.Length; i++)
            {
                starVisuals[i] = AddImagePanelRelative(hudBar, ArtAssetLibrary.GetGameplaySprite("star_empty"), Color.white,
                    new Vector2(0.40f + i * 0.055f, 0.50f), new Vector2(0.045f, 0.54f), true);
            }
            var starsFallback = AddTextRelative(hudBar, "☆☆☆", new Vector2(0.49f, 0.50f), new Vector2(0.18f, 0.55f), 24, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Center);
            AddImagePanelRelative(hudBar, ArtAssetLibrary.GetGameplaySprite("coin"), Color.white,
                new Vector2(0.68f, 0.50f), new Vector2(0.055f, 0.60f), true);
            var coinLabel = AddTextRelative(hudBar, "0", new Vector2(0.77f, 0.50f), new Vector2(0.15f, 0.58f), 25, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Left);
            var pauseButton = AddGameplayIconButton(hudBar, "PAUSE", ArtAssetLibrary.GetGameplaySprite("pause"),
                new Vector2(0.92f, 0.50f), new Vector2(0.12f, 0.82f), new Color(0.13f, 0.18f, 0.34f), null);

            var objectivePanel = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.78f), new Vector2(0.5f, 0.795f), new Vector2(0.90f, 0.075f));
            var objectiveLabel = AddTextRelative(objectivePanel, "Find the key and open the door.", new Vector2(0.5f, 0.50f), new Vector2(0.94f, 0.70f), 20, Color.white, TextAlignmentOptions.Center);
            var hintLabel = AddText(root, "", new Vector2(0.5f, 0.18f), new Vector2(0.88f, 0.055f), 19, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);

            var pausePanel = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.98f), new Vector2(0.5f, 0.50f), new Vector2(0.72f, 0.42f));
            AddTextRelative(pausePanel, "PAUSED", new Vector2(0.5f, 0.68f), new Vector2(0.90f, 0.18f), 42, Color.white, TextAlignmentOptions.Center);
            var successPanel = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.98f), new Vector2(0.5f, 0.50f), new Vector2(0.82f, 0.52f));
            AddTextRelative(successPanel, "LEVEL COMPLETE", new Vector2(0.5f, 0.78f), new Vector2(0.90f, 0.14f), 34, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);
            var completionStatsLabel = AddTextRelative(successPanel, "STARS 0 / 3", new Vector2(0.5f, 0.60f), new Vector2(0.82f, 0.08f), 23, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Center);
            var completionCoinsLabel = AddTextRelative(successPanel, "COINS +0", new Vector2(0.5f, 0.50f), new Vector2(0.82f, 0.08f), 22, new Color(1f, 0.63f, 0.08f), TextAlignmentOptions.Center);
            var failPanel = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.98f), new Vector2(0.5f, 0.50f), new Vector2(0.78f, 0.42f));
            AddTextRelative(failPanel, "TRY AGAIN", new Vector2(0.5f, 0.72f), new Vector2(0.90f, 0.16f), 34, new Color(1f, 0.63f, 0.08f), TextAlignmentOptions.Center);

            var retryButton = AddButtonRelative(failPanel, "RETRY", new Vector2(0.5f, 0.27f), new Vector2(0.58f, 0.18f), new Color(1f, 0.63f, 0.08f), null);
            var continueButton = AddButtonRelative(failPanel, "CONTINUE", new Vector2(0.5f, 0.06f), new Vector2(0.58f, 0.12f), new Color(0.10f, 0.82f, 0.78f), null);
            var hintButton = AddIconButton(root, "HINT  -5", "hint", new Vector2(0.5f, 0.085f), new Vector2(0.32f, 0.095f), new Color(0.55f, 0.22f, 1f), board.UseHint);

            var router = gameObject.AddComponent<LevelCompletionRouter>();
            AddButtonRelative(successPanel, "NEXT LEVEL", new Vector2(0.5f, 0.30f), new Vector2(0.68f, 0.14f), new Color(1f, 0.63f, 0.08f), router.LoadNextLevel);
            AddButtonRelative(successPanel, "MENU", new Vector2(0.5f, 0.09f), new Vector2(0.45f, 0.10f), new Color(0.13f, 0.18f, 0.34f), router.ReturnToMenu);
            AddButtonRelative(pausePanel, "RESUME", new Vector2(0.5f, 0.30f), new Vector2(0.60f, 0.16f), new Color(0.10f, 0.82f, 0.78f), runtime.TogglePause);
            AddButtonRelative(pausePanel, "MENU", new Vector2(0.5f, 0.10f), new Vector2(0.45f, 0.12f), new Color(0.13f, 0.18f, 0.34f), router.ReturnToMenu);

            var hud = gameObject.AddComponent<GameHudController>();
            hud.Configure(runtime, pausePanel.gameObject, successPanel.gameObject, failPanel.gameObject, objectiveLabel, levelLabel, hintLabel, coinLabel,
                pauseButton, retryButton, continueButton, hintButton, starVisuals, starsFallback, completionStatsLabel, completionCoinsLabel);
            var entry = LevelCatalogRuntime.All[selectedLevel - 1];
            hud.SetObjective(entry.objective, selectedLevel);
            var progression = FindFirstObjectByType<ProgressionService>();
            if (progression != null) hud.SetCoins(progression.Coins);
            pausePanel.gameObject.SetActive(false);
            successPanel.gameObject.SetActive(false);
            failPanel.gameObject.SetActive(false);
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
            SetNormalizedRect(rect, anchor, size);
            var image = obj.AddComponent<Image>();
            image.color = color;
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
            SetNormalizedRect(rect, anchor, size);
            var image = obj.AddComponent<Image>();
            image.color = color;
            var button = obj.AddComponent<Button>();
            button.targetGraphic = image;
            ConfigureButton(button, color);
            if (action != null) button.onClick.AddListener(action);
            var motion = obj.AddComponent<MotionFeedback>();
            button.onClick.AddListener(motion.Press);
            AddTextRelative(rect, label, new Vector2(0.5f, 0.5f), Vector2.one, 27, Color.white, TextAlignmentOptions.Center);
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
                labelText.rectTransform.anchorMin = new Vector2(0.06f, 0.04f);
                labelText.rectTransform.anchorMax = new Vector2(0.94f, 0.25f);
                labelText.rectTransform.sizeDelta = Vector2.zero;
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
                labelText.rectTransform.anchorMin = new Vector2(0.04f, 0.04f);
                labelText.rectTransform.anchorMax = new Vector2(0.96f, 0.25f);
                labelText.rectTransform.sizeDelta = Vector2.zero;
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

        private static void AddLevelCard(RectTransform content, RuntimeLevelEntry entry, LevelMapController mapController, UnityEngine.Events.UnityAction action)
        {
            var cardObject = new GameObject("LevelCard_" + entry.index.ToString("00"));
            cardObject.transform.SetParent(content, false);
            var card = cardObject.AddComponent<RectTransform>();
            card.sizeDelta = new Vector2(456f, 300f);
            var unlocked = mapController.IsLevelUnlocked(entry.index);
            var selectedLevel = Mathf.Clamp(PlayerPrefs.GetInt("il.selected_level", 1), 1, TotalLevels);
            var isCurrent = entry.index == selectedLevel;
            var cardImage = cardObject.AddComponent<Image>();
            cardImage.color = isCurrent
                ? new Color(0.18f, 0.25f, 0.46f, 0.99f)
                : unlocked ? new Color(0.13f, 0.18f, 0.34f, 0.98f) : new Color(0.08f, 0.10f, 0.19f, 0.98f);
            if (isCurrent)
            {
                var outline = cardObject.AddComponent<Outline>();
                outline.effectColor = new Color(1f, 0.78f, 0.24f, 0.95f);
                outline.effectDistance = new Vector2(5f, 5f);
            }

            var button = cardObject.AddComponent<Button>();
            button.targetGraphic = cardImage;
            ConfigureButton(button, cardImage.color);
            button.interactable = unlocked;
            if (unlocked) button.onClick.AddListener(action);
            var motion = cardObject.AddComponent<MotionFeedback>();
            if (unlocked) button.onClick.AddListener(motion.Press);

            var thumb = AddImagePanelRelative(card, ArtAssetLibrary.GetLevelThumbnail(entry.index), unlocked ? Color.white : new Color(0.48f, 0.50f, 0.60f, 1f), new Vector2(0.5f, 0.62f), new Vector2(0.90f, 0.58f), true);
            thumb.raycastTarget = false;
            AddTextRelative(card, entry.index.ToString("00"), new Vector2(0.13f, 0.92f), new Vector2(0.20f, 0.12f), 25, Color.white, TextAlignmentOptions.Center);
            AddLevelStars(card, mapController.GetLevelStars(entry.index));
            AddTextRelative(card, entry.title, new Vector2(0.5f, 0.15f), new Vector2(0.90f, 0.16f), 16, Color.white, TextAlignmentOptions.Center);
            if (isCurrent)
            {
                AddPanelRelative(card, new Color(1f, 0.78f, 0.24f, 0.92f), new Vector2(0.77f, 0.27f), new Vector2(0.28f, 0.08f));
                AddTextRelative(card, "CURRENT", new Vector2(0.77f, 0.27f), new Vector2(0.26f, 0.07f), 13, new Color(0.035f, 0.055f, 0.14f), TextAlignmentOptions.Center);
            }
            if (!unlocked)
            {
                AddPanelRelative(card, new Color(0.01f, 0.02f, 0.07f, 0.58f), new Vector2(0.5f, 0.62f), new Vector2(0.90f, 0.58f));
                AddTextRelative(card, "LOCKED", new Vector2(0.5f, 0.60f), new Vector2(0.8f, 0.14f), 22, Color.white, TextAlignmentOptions.Center);
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

        private static string StarString(int stars)
        {
            var value = string.Empty;
            for (var i = 0; i < 3; i++) value += i < stars ? "★" : "☆";
            return value;
        }
    }
}
