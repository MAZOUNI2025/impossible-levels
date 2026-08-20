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
            var root = canvas.transform as RectTransform;
            AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 1f), Vector2.zero, Vector2.one);
            AddText(root, "IMPOSSIBLE LEVELS", new Vector2(0.5f, 0.78f), new Vector2(0.92f, 0.1f), 54, Color.white, TextAlignmentOptions.Center);
            AddText(root, "Looks easy. Think again.", new Vector2(0.5f, 0.70f), new Vector2(0.85f, 0.06f), 25, new Color(0.10f, 0.82f, 0.78f), TextAlignmentOptions.Center);

            var menu = gameObject.AddComponent<MainMenuController>();
            AddButton(root, "PLAY", new Vector2(0.5f, 0.51f), new Vector2(0.58f, 0.09f), new Color(1f, 0.63f, 0.08f), menu.StartFirstLevel);
            AddButton(root, "LEVEL MAP", new Vector2(0.5f, 0.39f), new Vector2(0.58f, 0.09f), new Color(0.10f, 0.82f, 0.78f), menu.StartFirstLevel);
            AddButton(root, "PLAYER", new Vector2(0.5f, 0.27f), new Vector2(0.28f, 0.075f), new Color(0.55f, 0.22f, 1f), ShowPlayerInfo);
            AddButton(root, "SETTINGS", new Vector2(0.5f, 0.17f), new Vector2(0.28f, 0.075f), new Color(0.13f, 0.18f, 0.34f), ShowSettingsInfo);
            AddText(root, "13+  |  30 deterministic puzzles", new Vector2(0.5f, 0.07f), new Vector2(0.9f, 0.04f), 18, new Color(0.7f, 0.75f, 0.88f), TextAlignmentOptions.Center);
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
            var pausePanel = AddPanel(root, new Color(0.035f, 0.055f, 0.14f, 0.96f), new Vector2(0.5f, 0.5f), new Vector2(0.72f, 0.42f));
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
            var hintButton = AddButton(root, "HINT  -5", new Vector2(0.5f, 0.08f), new Vector2(0.30f, 0.07f), new Color(0.55f, 0.22f, 1f), board.UseHint);

            var router = gameObject.AddComponent<LevelCompletionRouter>();
            var nextButton = AddButton(successPanel, "NEXT LEVEL", new Vector2(0.5f, 0.30f), new Vector2(0.68f, 0.16f), new Color(1f, 0.63f, 0.08f), router.LoadNextLevel);
            AddButton(successPanel, "MENU", new Vector2(0.5f, 0.09f), new Vector2(0.45f, 0.11f), new Color(0.13f, 0.18f, 0.34f), router.ReturnToMenu);
            AddButton(pausePanel, "RESUME", new Vector2(0.5f, 0.30f), new Vector2(0.60f, 0.16f), new Color(0.10f, 0.82f, 0.78f), runtime.TogglePause);
            AddButton(pausePanel, "MENU", new Vector2(0.5f, 0.10f), new Vector2(0.45f, 0.12f), new Color(0.13f, 0.18f, 0.34f), router.ReturnToMenu);

            var hud = gameObject.AddComponent<GameHudController>();
            hud.Configure(runtime, pausePanel.gameObject, successPanel.gameObject, failPanel.gameObject, objectiveLabel, levelLabel, hintLabel, coinLabel, pauseButton, retryButton, continueButton, hintButton);
            var selectedLevel = Mathf.Clamp(PlayerPrefs.GetInt("il.selected_level", 1), 1, TotalLevels);
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
            return rect;
        }

        private static TMP_Text AddText(Transform parent, string value, Vector2 anchor, Vector2 size, float fontSize, Color color, TextAlignmentOptions alignment)
        {
            var obj = new GameObject("Text");
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = new Vector2(size.x * 1080f, size.y * 1920f);
            rect.anchoredPosition = Vector2.zero;
            var text = obj.AddComponent<TextMeshProUGUI>();
            text.text = value;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.enableWordWrapping = true;
            if (text.font == null) text.font = TMP_Settings.defaultFontAsset;
            return text;
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
            if (action != null) button.onClick.AddListener(action);
            AddText(rect, label, new Vector2(0.5f, 0.5f), Vector2.one, 27, Color.white, TextAlignmentOptions.Center);
            return button;
        }

        private void ShowPlayerInfo() { Debug.Log("Player profile is available from the Player button."); }
        private void ShowSettingsInfo() { Debug.Log("Settings are available from the Settings button."); }
    }
}
