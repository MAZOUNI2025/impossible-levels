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
            canvas = CreateCanvas("MainMenuCanvas");
            var canvasRoot = canvas.transform as RectTransform;
            var mainScreen = CreateScreen("HomeScreen", canvasRoot);
            var mapScreen = CreateScreen("LevelMapScreen", canvasRoot);
            var profileScreen = CreateScreen("ProfileScreen", canvasRoot);
            var settingsScreen = CreateScreen("SettingsScreen", canvasRoot);

            AddImagePanel(mainScreen, ArtAssetLibrary.GetLevelThumbnail(1), new Color(1f, 1f, 1f, 0.20f), Vector2.zero, Vector2.one, false);
            AddPanel(mainScreen, new Color(0.035f, 0.055f, 0.14f, 0.72f), Vector2.zero, Vector2.one);
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
            AddText(screen, "LEVEL MAP", new Vector2(0.5f, 0.94f), new Vector2(0.8f, 0.06f), 42, Color.white, TextAlignmentOptions.Center);
            AddText(screen, "Complete a level to unlock the next challenge.", new Vector2(0.5f, 0.885f), new Vector2(0.88f, 0.04f), 17, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);

            var scroll = CreateScrollView(screen, new Vector2(0.06f, 0.13f), new Vector2(0.88f, 0.72f));
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
            AddText(screen, "PLAYER", new Vector2(0.5f, 0.90f), new Vector2(0.8f, 0.08f), 46, Color.white, TextAlignmentOptions.Center);
            AddImagePanel(screen, ArtAssetLibrary.GetUiIcon("player"), Color.white, new Vector2(0.5f, 0.72f), new Vector2(0.22f, 0.13f), true);

            var progression = FindFirstObjectByType<ProgressionService>();
            var profileService = FindFirstObjectByType<PlayerProfileService>();
            var completed = profileService != null ? profileService.CompletedLevels : 0;
            var stars = profileService != null ? profileService.TotalStars : 0;
            var coins = progression != null ? progression.Coins : 0;
            AddText(screen, "COMPLETED   " + completed + " / 30", new Vector2(0.5f, 0.55f), new Vector2(0.75f, 0.06f), 27, Color.white, TextAlignmentOptions.Center);
            AddText(screen, "STARS   " + stars + " / 90", new Vector2(0.5f, 0.46f), new Vector2(0.75f, 0.06f), 27, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Center);
            AddText(screen, "COINS   " + coins, new Vector2(0.5f, 0.37f), new Vector2(0.75f, 0.06f), 27, new Color(1f, 0.63f, 0.08f), TextAlignmentOptions.Center);
            AddText(screen, "Keep solving. The board remembers your best stars.", new Vector2(0.5f, 0.28f), new Vector2(0.86f, 0.05f), 18, new Color(0.72f, 0.78f, 0.92f), TextAlignmentOptions.Center);
            AddButton(screen, "BACK", new Vector2(0.5f, 0.10f), new Vector2(0.32f, 0.07f), new Color(0.13f, 0.18f, 0.34f), () => ShowScreen(home, map, profile, settings));
        }

        private void BuildSettings(RectTransform screen, GameObject home, GameObject map, GameObject profile, GameObject settingsScreen)
        {
            AddPanel(screen, new Color(0.035f, 0.055f, 0.14f, 0.98f), Vector2.zero, Vector2.one);
            AddText(screen, "SETTINGS", new Vector2(0.5f, 0.90f), new Vector2(0.8f, 0.08f), 46, Color.white, TextAlignmentOptions.Center);
            AddImagePanel(screen, ArtAssetLibrary.GetUiIcon("settings"), Color.white, new Vector2(0.5f, 0.72f), new Vector2(0.20f, 0.12f), true);

            var controller = gameObject.AddComponent<SettingsController>();
            AddText(screen, "AUDIO", new Vector2(0.5f, 0.56f), new Vector2(0.8f, 0.05f), 22, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);
            AddButton(screen, "MUSIC ON", new Vector2(0.30f, 0.47f), new Vector2(0.28f, 0.08f), new Color(0.10f, 0.82f, 0.78f), () => controller.SetMusic(true));
            AddButton(screen, "MUSIC OFF", new Vector2(0.70f, 0.47f), new Vector2(0.28f, 0.08f), new Color(0.13f, 0.18f, 0.34f), () => controller.SetMusic(false));
            AddButton(screen, "SFX ON", new Vector2(0.30f, 0.37f), new Vector2(0.28f, 0.08f), new Color(0.10f, 0.82f, 0.78f), () => controller.SetSfx(true));
            AddButton(screen, "SFX OFF", new Vector2(0.70f, 0.37f), new Vector2(0.28f, 0.08f), new Color(0.13f, 0.18f, 0.34f), () => controller.SetSfx(false));
            AddText(screen, "RESET LOCAL PROGRESS", new Vector2(0.5f, 0.28f), new Vector2(0.8f, 0.04f), 18, new Color(0.78f, 0.82f, 0.94f), TextAlignmentOptions.Center);
            AddButton(screen, "RESET", new Vector2(0.5f, 0.21f), new Vector2(0.30f, 0.07f), new Color(0.55f, 0.22f, 1f), controller.ResetProgress);
            AddButton(screen, "BACK", new Vector2(0.5f, 0.10f), new Vector2(0.32f, 0.07f), new Color(0.13f, 0.18f, 0.34f), () => ShowScreen(home, map, profile, settingsScreen));
        }

        private void BuildGameplay()
        {
            var runtime = gameObject.AddComponent<LevelRuntime>();
            var levelRootObject = new GameObject("LevelRoot");
            levelRootObject.transform.SetParent(transform, false);
            var board = gameObject.AddComponent<ProceduralPuzzleBoard>();
            board.Configure(runtime, levelRootObject.transform, gameplayCamera);

            canvas = CreateCanvas("GameplayCanvas");
            var root = canvas.transform as RectTransform;
            var selectedLevel = Mathf.Clamp(PlayerPrefs.GetInt("il.selected_level", 1), 1, TotalLevels);
            AddImagePanel(root, ArtAssetLibrary.GetLevelThumbnail(selectedLevel), new Color(1f, 1f, 1f, 0.10f), new Vector2(0.5f, 0.79f), new Vector2(0.68f, 0.24f), true);
            AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.93f), new Vector2(0.5f, 0.5f), new Vector2(1f, 0.17f));

            var pausePanel = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.97f), new Vector2(0.5f, 0.5f), new Vector2(0.72f, 0.42f));
            AddText(pausePanel, "PAUSED", new Vector2(0.5f, 0.68f), new Vector2(0.9f, 0.18f), 42, Color.white, TextAlignmentOptions.Center);
            var successPanel = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.97f), new Vector2(0.5f, 0.5f), new Vector2(0.78f, 0.48f));
            AddText(successPanel, "LEVEL COMPLETE", new Vector2(0.5f, 0.72f), new Vector2(0.9f, 0.16f), 34, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);
            var failPanel = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.97f), new Vector2(0.5f, 0.5f), new Vector2(0.78f, 0.42f));
            AddText(failPanel, "TRY AGAIN", new Vector2(0.5f, 0.72f), new Vector2(0.9f, 0.16f), 34, new Color(1f, 0.63f, 0.08f), TextAlignmentOptions.Center);

            var levelLabel = AddText(root, "LEVEL", new Vector2(0.18f, 0.94f), new Vector2(0.32f, 0.05f), 26, Color.white, TextAlignmentOptions.Left);
            var objectiveLabel = AddText(root, "Find the key and open the door.", new Vector2(0.5f, 0.875f), new Vector2(0.86f, 0.07f), 22, Color.white, TextAlignmentOptions.Center);
            var hintLabel = AddText(root, "", new Vector2(0.5f, 0.16f), new Vector2(0.86f, 0.06f), 20, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);
            var coinLabel = AddText(root, "0", new Vector2(0.82f, 0.94f), new Vector2(0.25f, 0.05f), 26, new Color(1f, 0.63f, 0.08f), TextAlignmentOptions.Right);

            var pauseButton = AddButton(root, "II", new Vector2(0.90f, 0.875f), new Vector2(0.12f, 0.07f), new Color(0.13f, 0.18f, 0.34f), null);
            var retryButton = AddButton(failPanel, "RETRY", new Vector2(0.5f, 0.27f), new Vector2(0.58f, 0.18f), new Color(1f, 0.63f, 0.08f), null);
            var continueButton = AddButton(failPanel, "CONTINUE", new Vector2(0.5f, 0.06f), new Vector2(0.58f, 0.12f), new Color(0.10f, 0.82f, 0.78f), null);
            var hintButton = AddIconButton(root, "HINT  -5", "hint", new Vector2(0.5f, 0.08f), new Vector2(0.30f, 0.09f), new Color(0.55f, 0.22f, 1f), board.UseHint);

            var router = gameObject.AddComponent<LevelCompletionRouter>();
            AddButton(successPanel, "NEXT LEVEL", new Vector2(0.5f, 0.30f), new Vector2(0.68f, 0.16f), new Color(1f, 0.63f, 0.08f), router.LoadNextLevel);
            AddButton(successPanel, "MENU", new Vector2(0.5f, 0.09f), new Vector2(0.45f, 0.11f), new Color(0.13f, 0.18f, 0.34f), router.ReturnToMenu);
            AddButton(pausePanel, "RESUME", new Vector2(0.5f, 0.30f), new Vector2(0.60f, 0.16f), new Color(0.10f, 0.82f, 0.78f), runtime.TogglePause);
            AddButton(pausePanel, "MENU", new Vector2(0.5f, 0.10f), new Vector2(0.45f, 0.12f), new Color(0.13f, 0.18f, 0.34f), router.ReturnToMenu);

            var hud = gameObject.AddComponent<GameHudController>();
            hud.Configure(runtime, pausePanel.gameObject, successPanel.gameObject, failPanel.gameObject, objectiveLabel, levelLabel, hintLabel, coinLabel, pauseButton, retryButton, continueButton, hintButton);
            var entry = LevelCatalogRuntime.All[selectedLevel - 1];
            hud.SetObjective(entry.objective, selectedLevel);
            var progression = FindFirstObjectByType<ProgressionService>();
            if (progression != null) hud.SetCoins(progression.Coins);
            pausePanel.gameObject.SetActive(false);
            successPanel.gameObject.SetActive(false);
            failPanel.gameObject.SetActive(false);
        }

        private Canvas CreateCanvas(string name)
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
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(size.x * 1080f, size.y * 1920f);
            rect.anchoredPosition = Vector2.zero;
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
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(size.x * 1080f, size.y * 1920f);
            rect.anchoredPosition = Vector2.zero;
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
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(size.x * parent.rect.width, size.y * parent.rect.height);
            rect.anchoredPosition = Vector2.zero;
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
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(size.x * parent.rect.width, size.y * parent.rect.height);
            rect.anchoredPosition = Vector2.zero;
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
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(size.x * 1080f, size.y * 1920f);
            rect.anchoredPosition = Vector2.zero;
            var text = obj.AddComponent<TextMeshProUGUI>();
            text.font = ResolveFontAsset();
            text.text = value;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.Normal;
            obj.SetActive(true);
            return text;
        }

        private static TMP_Text AddTextRelative(RectTransform parent, string value, Vector2 anchor, Vector2 size, float fontSize, Color color, TextAlignmentOptions alignment)
        {
            var obj = new GameObject("Text");
            obj.SetActive(false);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(size.x * parent.rect.width, size.y * parent.rect.height);
            rect.anchoredPosition = Vector2.zero;
            var text = obj.AddComponent<TextMeshProUGUI>();
            text.font = ResolveFontAsset();
            text.text = value;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.Normal;
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
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(size.x * 1080f, size.y * 1920f);
            rect.anchoredPosition = Vector2.zero;
            var image = obj.AddComponent<Image>();
            image.color = color;
            var button = obj.AddComponent<Button>();
            button.targetGraphic = image;
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

        private static ScrollRect CreateScrollView(RectTransform parent, Vector2 anchor, Vector2 size)
        {
            var scrollObject = new GameObject("LevelMapScroll");
            scrollObject.transform.SetParent(parent, false);
            var scrollRectTransform = scrollObject.AddComponent<RectTransform>();
            scrollRectTransform.anchorMin = anchor;
            scrollRectTransform.anchorMax = anchor;
            scrollRectTransform.sizeDelta = new Vector2(size.x * 1080f, size.y * 1920f);
            scrollRectTransform.anchoredPosition = Vector2.zero;

            var scroll = scrollObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 30f;

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
            layout.cellSize = new Vector2(300f, 275f);
            layout.spacing = new Vector2(18f, 18f);
            layout.padding = new RectOffset(12, 12, 18, 24);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 3;
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
            card.sizeDelta = new Vector2(300f, 275f);
            var unlocked = mapController.IsLevelUnlocked(entry.index);
            var cardImage = cardObject.AddComponent<Image>();
            cardImage.color = unlocked ? new Color(0.13f, 0.18f, 0.34f, 0.98f) : new Color(0.08f, 0.10f, 0.19f, 0.98f);
            var button = cardObject.AddComponent<Button>();
            button.targetGraphic = cardImage;
            button.interactable = unlocked;
            if (unlocked) button.onClick.AddListener(action);
            var motion = cardObject.AddComponent<MotionFeedback>();
            if (unlocked) button.onClick.AddListener(motion.Press);

            var thumb = AddImagePanelRelative(card, ArtAssetLibrary.GetLevelThumbnail(entry.index), unlocked ? Color.white : new Color(0.45f, 0.48f, 0.58f, 1f), new Vector2(0.5f, 0.62f), new Vector2(0.88f, 0.64f), true);
            thumb.raycastTarget = false;
            AddTextRelative(card, entry.index.ToString("00"), new Vector2(0.14f, 0.92f), new Vector2(0.22f, 0.12f), 25, Color.white, TextAlignmentOptions.Center);
            var stars = mapController.GetLevelStars(entry.index);
            AddTextRelative(card, StarString(stars), new Vector2(0.72f, 0.92f), new Vector2(0.48f, 0.12f), 22, new Color(1f, 0.78f, 0.24f), TextAlignmentOptions.Center);
            AddTextRelative(card, entry.title, new Vector2(0.5f, 0.15f), new Vector2(0.9f, 0.18f), 15, Color.white, TextAlignmentOptions.Center);
            if (!unlocked)
            {
                AddPanelRelative(card, new Color(0.01f, 0.02f, 0.07f, 0.58f), new Vector2(0.5f, 0.60f), new Vector2(0.88f, 0.64f));
                AddTextRelative(card, "LOCKED", new Vector2(0.5f, 0.60f), new Vector2(0.8f, 0.14f), 22, Color.white, TextAlignmentOptions.Center);
            }
        }

        private static string StarString(int stars)
        {
            var value = string.Empty;
            for (var i = 0; i < 3; i++) value += i < stars ? "★" : "☆";
            return value;
        }
    }
}
