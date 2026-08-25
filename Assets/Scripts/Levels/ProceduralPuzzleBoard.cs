using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImpossibleLevels.Audio;
using ImpossibleLevels.Core;
using ImpossibleLevels.UI;

namespace ImpossibleLevels.Levels
{
    public sealed class ProceduralPuzzleBoard : MonoBehaviour
    {
        [SerializeField] private LevelRuntime runtime;
        [SerializeField] private Transform levelRoot;
        [SerializeField] private Camera gameplayCamera;
        [SerializeField] private int levelIndex = 1;
        [SerializeField] private float boardWidth = 8.2f;
        [SerializeField] private float boardHeight = 13.0f;

        private readonly List<PuzzleNode> nodes = new();
        private Sprite squareSprite;
        private TouchInputRouter inputRouter;
        private LevelMechanicConfig mechanicConfig;
        private PuzzleNode pressedNode;
        private PuzzleNode draggedNode;
        private Vector2 pointerStart;
        private int activePointerId = int.MinValue;
        private bool hasKey;
        private bool blockPlaced;
        private bool switchOn;
        private bool revealActive;
        private bool solved;
        private int sequenceProgress;
        private float startedAt;
        private int hintCount;
        private Vector2 blockTargetPosition;
        private PuzzleNode hiddenKeyNode;
        private PuzzleNode doorNode;
        private PuzzleNode switchNode;
        private DoorState doorState = DoorState.Locked;
        private readonly List<PuzzleNode> sequenceNodes = new();

        // Presentation-only notifications; gameplay and progression remain authoritative below.
        public event Action<int, int, int> CompletionSummaryReady;
        public event Action<int> HintUnavailable;

        public const int HintCost = 5;

        private static readonly Color Navy = new(0.035f, 0.055f, 0.14f);
        private static readonly Color Amber = new(1f, 0.63f, 0.08f);
        private static readonly Color Purple = new(0.55f, 0.22f, 1f);
        private static readonly Color Teal = new(0.10f, 0.82f, 0.78f);
        private static readonly Color Slate = new(0.13f, 0.18f, 0.34f);
        private static readonly Color Disabled = new(0.35f, 0.40f, 0.55f, 0.55f);

        public void Configure(LevelRuntime levelRuntime, Transform root, Camera camera)
        {
            runtime = levelRuntime;
            levelRoot = root;
            gameplayCamera = camera;
        }

        private void Awake()
        {
            inputRouter = FindFirstObjectByType<TouchInputRouter>();
        }

        private void OnEnable()
        {
            if (inputRouter == null) inputRouter = FindFirstObjectByType<TouchInputRouter>();
            if (inputRouter != null)
            {
                inputRouter.PointerPressed += OnPointerPressed;
                inputRouter.PointerMoved += OnPointerMoved;
                inputRouter.PointerReleased += OnPointerReleased;
            }

            if (runtime != null) runtime.StateChanged += OnRuntimeStateChanged;
        }

        private void OnDisable()
        {
            if (inputRouter != null)
            {
                inputRouter.PointerPressed -= OnPointerPressed;
                inputRouter.PointerMoved -= OnPointerMoved;
                inputRouter.PointerReleased -= OnPointerReleased;
            }

            if (runtime != null) runtime.StateChanged -= OnRuntimeStateChanged;
            ResetPointerState(true);
        }

        private void Start()
        {
            runtime = runtime != null ? runtime : FindFirstObjectByType<LevelRuntime>();
            gameplayCamera = gameplayCamera != null ? gameplayCamera : Camera.main;
            if (inputRouter == null) inputRouter = FindFirstObjectByType<TouchInputRouter>();
            if (inputRouter != null)
            {
                inputRouter.PointerPressed -= OnPointerPressed;
                inputRouter.PointerMoved -= OnPointerMoved;
                inputRouter.PointerReleased -= OnPointerReleased;
                inputRouter.PointerPressed += OnPointerPressed;
                inputRouter.PointerMoved += OnPointerMoved;
                inputRouter.PointerReleased += OnPointerReleased;
            }

            if (runtime != null)
            {
                runtime.StateChanged -= OnRuntimeStateChanged;
                runtime.StateChanged += OnRuntimeStateChanged;
            }

            levelIndex = Mathf.Clamp(PlayerPrefs.GetInt("il.selected_level", levelIndex), 1, 30);
            if (runtime != null) runtime.SetLevelIndex(levelIndex);
            squareSprite = CreateSquareSprite();
            BuildLevel(levelIndex);
        }

        public void UseHint()
        {
            if (solved) return;
            var progression = FindFirstObjectByType<ProgressionService>();
            if (progression != null && progression.Coins < HintCost)
            {
                HintUnavailable?.Invoke(HintCost);
                if (AudioDirector.Instance != null) AudioDirector.Instance.Invalid();
                return;
            }

            hintCount++;
            if (AudioDirector.Instance != null) AudioDirector.Instance.Hint();
            HapticsFeedback.TryPulse();
            var hintedNode = FindHintTarget();
            if (hintedNode != null) hintedNode.PulseHint();
            if (progression != null) progression.SpendCoins(HintCost);
        }

        private void OnRuntimeStateChanged(LevelState state)
        {
            if (state == LevelState.Playing && startedAt > 0f && (nodes.Count == 0 || AllNodesDestroyed()))
            {
                BuildLevel(levelIndex);
            }
        }

        private void OnPointerPressed(PointerSample sample)
        {
            if (solved || runtime == null || runtime.State != LevelState.Playing || activePointerId != int.MinValue)
            {
                return;
            }

            activePointerId = sample.PointerId;
            pointerStart = WorldPoint(sample.ScreenPosition);
            pressedNode = FindNode(pointerStart);
            draggedNode = pressedNode != null && pressedNode.CanDragFor(mechanicConfig) ? pressedNode : null;
            if (draggedNode != null) draggedNode.BeginDrag();
        }

        private void OnPointerMoved(PointerSample sample)
        {
            if (sample.PointerId != activePointerId || draggedNode == null || runtime == null || runtime.State != LevelState.Playing)
            {
                return;
            }

            draggedNode.transform.position = WorldPoint(sample.ScreenPosition);
        }

        private void OnPointerReleased(PointerSample sample)
        {
            if (sample.PointerId != activePointerId)
            {
                return;
            }

            if (runtime == null || runtime.State != LevelState.Playing)
            {
                ResetPointerState(true);
                return;
            }

            var releasePoint = WorldPoint(sample.ScreenPosition);
            var node = draggedNode;
            var distance = Vector2.Distance(pointerStart, releasePoint);
            if (sample.IsCanceled)
            {
                if (node != null) node.ResetToStart();
                ResetPointerState(true);
                return;
            }

            if (node != null)
            {
                if (distance < 0.22f) HandleTap(node);
                else HandleDrop(node, releasePoint);
                node.EndDrag();
            }
            else
            {
                var releasedNode = FindNode(releasePoint);
                if (releasedNode != null && releasedNode == pressedNode && distance < 0.45f)
                {
                    HandleTap(releasedNode);
                }
            }

            ResetPointerState(false);
        }

        private void ResetPointerState(bool cancelDrag)
        {
            if (cancelDrag && draggedNode != null) draggedNode.ResetToStart();
            draggedNode = null;
            pressedNode = null;
            activePointerId = int.MinValue;
        }

        private void BuildLevel(int index)
        {
            ClearBoard();
            hasKey = false;
            blockPlaced = false;
            switchOn = false;
            revealActive = false;
            solved = false;
            sequenceProgress = 0;
            hintCount = 0;
            hiddenKeyNode = null;
            doorNode = null;
            switchNode = null;
            doorState = DoorState.Locked;
            sequenceNodes.Clear();
            ResetPointerState(true);
            startedAt = Time.unscaledTime;

            var entries = LevelCatalogRuntime.All;
            mechanicConfig = entries != null && index >= 1 && index <= entries.Count
                ? entries[index - 1].mechanics
                : null;
            if (mechanicConfig == null)
            {
                mechanicConfig = LevelMechanicConfig.ForEntry(index, PuzzleType.Interaction, 1, "Open the door.", "Find the key.");
            }

            CreateBlock("Background", new Vector2(0f, 0f), new Vector2(boardWidth, boardHeight), Navy, -10);
            CreateBlock("Floor", new Vector2(0f, -5.35f), new Vector2(7.4f, 0.45f), Slate, -1);
            CreateBlock("TopRail", new Vector2(0f, 5.35f), new Vector2(7.4f, 0.25f), Slate, -1);
            CreatePlayerVisual(new Vector2(-2.65f, -3.85f), new Vector2(0.95f, 1.15f));

            var variation = (mechanicConfig.deterministicSeed % 8 + 8) % 8;
            var keyPosition = new Vector2(-2.4f + (variation % 4) * 1.2f, 1.35f - (variation / 4) * 1.35f);
            var doorPosition = new Vector2(2.45f, -3.7f + (variation % 2) * 0.9f);
            var keyIsVisible = mechanicConfig.rule != GameplayRule.RevealObservation;
            var key = CreateNode("Key", keyPosition, new Vector2(0.78f, 0.78f), Amber, NodeKind.Key, false);
            hiddenKeyNode = keyIsVisible ? null : key;
            if (!keyIsVisible) key.SetVisible(false);
            doorNode = CreateNode("Door", doorPosition, new Vector2(1.35f, 2.15f), Purple, NodeKind.Door, false);
            doorNode.SetDoorState(DoorState.Locked);

            switch (mechanicConfig.rule)
            {
                case GameplayRule.DragPlace:
                    var targetX = -1.4f + (mechanicConfig.variationIndex % 3) * 1.4f;
                    var targetY = -1.45f + (mechanicConfig.variationIndex % 2) * 1.1f;
                    blockTargetPosition = new Vector2(targetX, targetY);
                    CreateNode("BlockTarget", blockTargetPosition, new Vector2(1.35f, 0.95f), Disabled, NodeKind.BlockTarget, false, false);
                    var blockStart = new Vector2(targetX > 0f ? -2.5f : 2.0f, targetY + 1.35f);
                    CreateNode("MovableBlock", blockStart, new Vector2(1.2f, 0.8f), Slate, NodeKind.Block, true);
                    break;
                case GameplayRule.SwitchState:
                    var switchX = -1.8f + (mechanicConfig.variationIndex % 4) * 1.2f;
                    switchNode = CreateNode("Switch", new Vector2(switchX, 2.65f), new Vector2(1.1f, 0.35f), Teal, NodeKind.Switch, false);
                    break;
                case GameplayRule.RevealObservation:
                    var revealX = mechanicConfig.deterministicSeed % 2 == 0 ? -1.8f : 1.8f;
                    CreateNode("RevealTrigger", new Vector2(revealX, 2.65f), new Vector2(1.1f, 0.65f), Teal, NodeKind.RevealTrigger, false);
                    break;
                case GameplayRule.FairSequence:
                    CreateSequenceNodes();
                    break;
                case GameplayRule.KeyDoor:
                    CreateLegacyDecorations(index);
                    break;
            }
        }

        private void CreateSequenceNodes()
        {
            var sequenceLength = Mathf.Clamp(mechanicConfig.sequenceLength, 1, 5);
            var direction = mechanicConfig.deterministicSeed % 2 == 0 ? 1f : -1f;
            var startX = sequenceLength == 5 ? -2.6f : -1.9f;
            for (var i = 0; i < sequenceLength; i++)
            {
                var position = new Vector2(startX + direction * i * 1.3f, 3.0f - (mechanicConfig.variationIndex % 2) * 0.55f);
                sequenceNodes.Add(CreateNode("SequenceStep_" + (i + 1), position, new Vector2(0.72f, 0.72f), Teal, NodeKind.SequenceStep, false, true, i));
            }
        }

        private void CreateLegacyDecorations(int index)
        {
            var decoyCount = mechanicConfig != null ? mechanicConfig.decoyCount : Mathf.Min(3, Mathf.Max(0, (index - 1) / 5));
            for (var i = 0; i < decoyCount; i++)
            {
                var x = -2.3f + (i % 3) * 2.0f;
                var y = -1.1f + (i / 3) * 1.45f;
                CreateNode("Decoy_" + i, new Vector2(x, y), new Vector2(0.75f, 0.75f), Teal, NodeKind.Decoy, i % 2 == 1);
            }
        }

        private void HandleTap(PuzzleNode node)
        {
            if (node == null || solved || runtime == null || runtime.State != LevelState.Playing) return;
            if (AudioDirector.Instance != null) AudioDirector.Instance.Tap();

            switch (node.Kind)
            {
                case NodeKind.Key:
                    if (!node.IsVisible) return;
                    if (hasKey) return;
                    hasKey = true;
                    node.CollectFeedbackAndHide();
                    if (AudioDirector.Instance != null) AudioDirector.Instance.KeyPickup();
                    HapticsFeedback.TryPulse();
                    UpdateDoorState();
                    break;
                case NodeKind.Door:
                    if (CanComplete())
                    {
                        doorState = DoorState.Open;
                        node.SetDoorState(doorState);
                        node.PulseSuccess();
                        Complete();
                    }
                    else
                    {
                        node.PulseInvalid();
                        if (AudioDirector.Instance != null) AudioDirector.Instance.Invalid();
                        HapticsFeedback.TryPulse();
                    }
                    break;
                case NodeKind.Switch:
                    if (mechanicConfig.rule != GameplayRule.SwitchState) return;
                    switchOn = !switchOn;
                    node.SetToggled(switchOn);
                    node.PulseSuccess();
                    UpdateDoorState();
                    break;
                case NodeKind.RevealTrigger:
                    if (mechanicConfig.rule != GameplayRule.RevealObservation || revealActive) return;
                    revealActive = true;
                    if (hiddenKeyNode != null) hiddenKeyNode.SetVisible(true);
                    node.SetToggled(true);
                    node.PulseSuccess();
                    UpdateDoorState();
                    break;
                case NodeKind.SequenceStep:
                    HandleSequenceStep(node);
                    break;
                case NodeKind.Decoy:
                    node.PulseInvalid();
                    if (AudioDirector.Instance != null) AudioDirector.Instance.Invalid();
                    break;
            }
        }

        private void HandleSequenceStep(PuzzleNode node)
        {
            if (!hasKey || mechanicConfig.rule != GameplayRule.FairSequence)
            {
                node.PulseInvalid();
                return;
            }

            if (node.SequenceIndex == sequenceProgress)
            {
                node.SetToggled(true);
                node.PulseSuccess();
                sequenceProgress++;
                UpdateDoorState();
            }
            else
            {
                sequenceProgress = 0;
                for (var i = 0; i < sequenceNodes.Count; i++) sequenceNodes[i].SetToggled(false);
                node.PulseInvalid();
                if (AudioDirector.Instance != null) AudioDirector.Instance.Invalid();
                HapticsFeedback.TryPulse();
            }
        }

        private void HandleDrop(PuzzleNode node, Vector2 position)
        {
            if (node == null || node.Kind != NodeKind.Block || mechanicConfig.rule != GameplayRule.DragPlace) return;
            if (Vector2.Distance(position, blockTargetPosition) < 1.0f)
            {
                node.transform.position = blockTargetPosition;
                node.SetDraggable(false);
                blockPlaced = true;
                if (AudioDirector.Instance != null) AudioDirector.Instance.Tap();
                node.PulseSuccess();
                UpdateDoorState();
            }
            else
            {
                node.ResetToStart();
                if (AudioDirector.Instance != null) AudioDirector.Instance.Invalid();
                node.PulseInvalid();
            }
        }

        private bool CanComplete()
        {
            if (!hasKey) return false;
            return mechanicConfig.rule switch
            {
                GameplayRule.DragPlace => blockPlaced,
                GameplayRule.SwitchState => switchOn,
                GameplayRule.RevealObservation => revealActive,
                GameplayRule.FairSequence => sequenceProgress >= sequenceNodes.Count && sequenceNodes.Count > 0,
                _ => true
            };
        }

        private void UpdateDoorState()
        {
            if (doorNode == null || solved) return;
            var nextState = CanComplete() ? DoorState.Ready : DoorState.Locked;
            if (nextState == doorState) return;
            doorState = nextState;
            doorNode.SetDoorState(doorState);
        }

        private void Complete()
        {
            if (solved || runtime == null || !CanComplete()) return;
            solved = true;
            var elapsed = Time.unscaledTime - startedAt;
            var stars = runtime.CalculateStars(elapsed, hintCount);
            var progression = FindFirstObjectByType<ProgressionService>();
            var calculatedReward = runtime.CalculateCoinReward(stars);
            var grantedReward = progression != null
                ? progression.CompleteLevel(levelIndex, stars, calculatedReward)
                : 0;
            CompletionSummaryReady?.Invoke(levelIndex, stars, grantedReward);
            if (AudioDirector.Instance != null)
            {
                AudioDirector.Instance.DoorUnlock();
                AudioDirector.Instance.Success();
            }
            HapticsFeedback.TryPulse();
            runtime.CompleteLevel();
        }

        private PuzzleNode FindNode(Vector2 worldPoint)
        {
            for (var i = nodes.Count - 1; i >= 0; i--)
            {
                var node = nodes[i];
                if (node != null && node.gameObject.activeInHierarchy && node.Contains(worldPoint)) return node;
            }
            return null;
        }

        private PuzzleNode FindHintTarget()
        {
            if (mechanicConfig == null) return null;
            switch (mechanicConfig.rule)
            {
                case GameplayRule.DragPlace:
                    return FindNodeByKind(NodeKind.Block) ?? FindNodeByKind(NodeKind.Door);
                case GameplayRule.SwitchState:
                    return switchOn ? FindNodeByKind(NodeKind.Door) : switchNode;
                case GameplayRule.RevealObservation:
                    return revealActive ? FindNodeByKind(NodeKind.Key) : FindNodeByKind(NodeKind.RevealTrigger);
                case GameplayRule.FairSequence:
                    return sequenceProgress < sequenceNodes.Count ? sequenceNodes[sequenceProgress] : FindNodeByKind(NodeKind.Door);
                default:
                    return FindNodeByKind(NodeKind.Key) ?? FindNodeByKind(NodeKind.Door);
            }
        }

        private PuzzleNode FindNodeByKind(NodeKind kind)
        {
            for (var i = nodes.Count - 1; i >= 0; i--)
            {
                var node = nodes[i];
                if (node != null && node.gameObject.activeInHierarchy && node.Kind == kind) return node;
            }
            return null;
        }

        private PuzzleNode CreateNode(string name, Vector2 position, Vector2 size, Color color, NodeKind kind, bool draggable, bool createCollider = true, int sequenceIndex = -1)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(levelRoot != null ? levelRoot : transform, false);
            obj.transform.position = position;
            obj.transform.localScale = new Vector3(size.x, size.y, 1f);
            var renderer = obj.AddComponent<SpriteRenderer>();
            var gameplaySprite = UsesGameplayArt(kind) ? ArtAssetLibrary.GetGameplaySprite(kind.ToString()) : null;
            renderer.sprite = gameplaySprite ?? squareSprite;
            renderer.color = gameplaySprite != null ? Color.white : color;
            renderer.sortingOrder = 2;
            var collider = createCollider ? obj.AddComponent<BoxCollider2D>() : null;
            if (collider != null) collider.size = Vector2.one;
            var node = obj.AddComponent<PuzzleNode>();
            node.Configure(kind, draggable, position, size, renderer, collider, sequenceIndex);
            nodes.Add(node);
            return node;
        }

        private static bool UsesGameplayArt(NodeKind kind)
        {
            return kind == NodeKind.Key || kind == NodeKind.Door || kind == NodeKind.Switch || kind == NodeKind.Decoy || kind == NodeKind.Block;
        }

        private void CreateBlock(string name, Vector2 position, Vector2 size, Color color, int sortingOrder)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(levelRoot != null ? levelRoot : transform, false);
            obj.transform.position = position;
            obj.transform.localScale = new Vector3(size.x, size.y, 1f);
            var renderer = obj.AddComponent<SpriteRenderer>();
            var gameplaySprite = ArtAssetLibrary.GetGameplaySprite(name);
            renderer.sprite = gameplaySprite ?? squareSprite;
            renderer.color = gameplaySprite != null ? Color.white : color;
            renderer.sortingOrder = sortingOrder;
        }

        private void CreatePlayerVisual(Vector2 position, Vector2 size)
        {
            var obj = new GameObject("PlayerVisual");
            obj.transform.SetParent(levelRoot != null ? levelRoot : transform, false);
            obj.transform.position = position;
            obj.transform.localScale = new Vector3(size.x, size.y, 1f);

            var renderer = obj.AddComponent<SpriteRenderer>();
            var gameplaySprite = ArtAssetLibrary.GetGameplaySprite("Player");
            renderer.sprite = gameplaySprite ?? squareSprite;
            renderer.color = gameplaySprite != null ? Color.white : new Color(0.12f, 0.82f, 0.95f, 1f);
            renderer.sortingOrder = 3;
        }

        private void ClearBoard()
        {
            nodes.Clear();
            var root = levelRoot != null ? levelRoot : transform;
            for (var i = root.childCount - 1; i >= 0; i--) Destroy(root.GetChild(i).gameObject);
        }

        private bool AllNodesDestroyed()
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] != null) return false;
            }
            return true;
        }

        private Vector2 WorldPoint(Vector2 screenPosition)
        {
            if (gameplayCamera == null) return screenPosition;
            var world = gameplayCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -gameplayCamera.transform.position.z));
            return new Vector2(world.x, world.y);
        }

        private static Sprite CreateSquareSprite()
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        private enum DoorState
        {
            Locked,
            Ready,
            Open
        }

        private enum NodeKind
        {
            Key,
            Door,
            Switch,
            Decoy,
            Block,
            BlockTarget,
            RevealTrigger,
            SequenceStep
        }

        private sealed class PuzzleNode : MonoBehaviour
        {
            public NodeKind Kind { get; private set; }
            public int SequenceIndex { get; private set; }
            public bool IsVisible => gameObject.activeInHierarchy;
            private bool canDrag;
            private Vector2 startPosition;
            private Vector2 size;
            private SpriteRenderer nodeRenderer;
            private BoxCollider2D nodeCollider;
            private Color originalColor;
            private bool toggled;

            public void Configure(NodeKind nodeKind, bool draggable, Vector2 start, Vector2 nodeSize, SpriteRenderer renderer, BoxCollider2D collider, int sequenceIndex)
            {
                Kind = nodeKind;
                canDrag = draggable;
                startPosition = start;
                size = nodeSize;
                nodeRenderer = renderer;
                nodeCollider = collider;
                SequenceIndex = sequenceIndex;
                originalColor = nodeRenderer != null ? nodeRenderer.color : Color.white;
                toggled = false;
            }

            public bool CanDragFor(LevelMechanicConfig config)
            {
                return canDrag && config != null && config.rule == GameplayRule.DragPlace;
            }

            public bool Contains(Vector2 point)
            {
                return nodeCollider != null && nodeCollider.enabled && nodeCollider.bounds.Contains(point);
            }

            public void BeginDrag()
            {
                if (!canDrag) return;
                if (nodeRenderer != null) nodeRenderer.color = Color.Lerp(originalColor, Color.white, 0.25f);
                transform.localScale = transform.localScale * 1.04f;
            }

            public void EndDrag()
            {
                if (nodeRenderer != null) nodeRenderer.color = CurrentVisualColor();
                transform.localScale = new Vector3(size.x, size.y, 1f);
            }

            public void ResetToStart()
            {
                transform.position = startPosition;
                EndDrag();
            }

            public void SetDraggable(bool value)
            {
                canDrag = value;
            }

            public void SetVisible(bool visible)
            {
                gameObject.SetActive(visible);
                if (nodeCollider != null) nodeCollider.enabled = visible;
            }

            public void SetToggled(bool value)
            {
                toggled = value;
                if (nodeRenderer != null) nodeRenderer.color = CurrentVisualColor();
            }

            public void SetDoorState(DoorState state)
            {
                if (nodeRenderer == null) return;
                nodeRenderer.color = state switch
                {
                    DoorState.Ready => Color.Lerp(originalColor, Color.white, 0.35f),
                    DoorState.Open => Color.Lerp(originalColor, new Color(0.25f, 1f, 0.55f), 0.65f),
                    _ => originalColor
                };
            }

            public void PulseInvalid()
            {
                StopAllCoroutines();
                CancelInvoke(nameof(ResetColor));
                if (nodeRenderer != null) nodeRenderer.color = Color.Lerp(CurrentVisualColor(), Color.red, 0.35f);
                Invoke(nameof(ResetColor), 0.16f);
            }

            public void PulseSuccess()
            {
                StopAllCoroutines();
                CancelInvoke(nameof(ResetColor));
                StartCoroutine(SuccessRoutine());
            }

            public void PulseHint()
            {
                StopAllCoroutines();
                CancelInvoke(nameof(ResetColor));
                StartCoroutine(HintRoutine());
            }

            public void CollectFeedbackAndHide()
            {
                if (!gameObject.activeInHierarchy) return;
                if (nodeCollider != null) nodeCollider.enabled = false;
                StopAllCoroutines();
                CancelInvoke(nameof(ResetColor));
                StartCoroutine(CollectRoutine());
            }

            private IEnumerator SuccessRoutine()
            {
                var startScale = transform.localScale;
                if (nodeRenderer != null) nodeRenderer.color = Color.Lerp(CurrentVisualColor(), Color.white, 0.32f);
                transform.localScale = startScale * 1.10f;
                yield return new WaitForSecondsRealtime(0.12f);
                if (nodeRenderer != null) nodeRenderer.color = CurrentVisualColor();
                transform.localScale = startScale;
            }

            private IEnumerator HintRoutine()
            {
                var startScale = transform.localScale;
                if (nodeRenderer != null) nodeRenderer.color = Color.Lerp(CurrentVisualColor(), Color.white, 0.45f);
                transform.localScale = startScale * 1.08f;
                yield return new WaitForSecondsRealtime(0.18f);
                if (nodeRenderer != null) nodeRenderer.color = CurrentVisualColor();
                transform.localScale = startScale;
            }

            private IEnumerator CollectRoutine()
            {
                var startScale = transform.localScale;
                if (nodeRenderer != null) nodeRenderer.color = Color.Lerp(CurrentVisualColor(), Color.white, 0.45f);
                transform.localScale = startScale * 1.14f;
                yield return new WaitForSecondsRealtime(0.12f);
                gameObject.SetActive(false);
            }

            private Color CurrentVisualColor()
            {
                return toggled
                    ? Color.Lerp(originalColor, new Color(0.10f, 0.95f, 0.82f), 0.35f)
                    : originalColor;
            }

            private void ResetColor()
            {
                if (nodeRenderer != null) nodeRenderer.color = CurrentVisualColor();
            }
        }
    }
}
