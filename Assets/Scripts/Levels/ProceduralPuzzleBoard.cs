using System.Collections.Generic;
using UnityEngine;
using ImpossibleLevels.Audio;
using ImpossibleLevels.Core;

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
        private bool hasKey;
        private bool solved;
        private Vector2 pointerStart;
        private PuzzleNode draggedNode;
        private float startedAt;
        private int hintCount;

        private static readonly Color Navy = new(0.035f, 0.055f, 0.14f);
        private static readonly Color Amber = new(1f, 0.63f, 0.08f);
        private static readonly Color Purple = new(0.55f, 0.22f, 1f);
        private static readonly Color Teal = new(0.10f, 0.82f, 0.78f);
        private static readonly Color Slate = new(0.13f, 0.18f, 0.34f);

        public void Configure(LevelRuntime levelRuntime, Transform root, Camera camera)
        {
            runtime = levelRuntime;
            levelRoot = root;
            gameplayCamera = camera;
        }

        private void Start()
        {
            runtime = runtime != null ? runtime : FindFirstObjectByType<LevelRuntime>();
            gameplayCamera = gameplayCamera != null ? gameplayCamera : Camera.main;
            levelIndex = Mathf.Clamp(PlayerPrefs.GetInt("il.selected_level", levelIndex), 1, 30);
            if (runtime != null) runtime.SetLevelIndex(levelIndex);
            squareSprite = CreateSquareSprite();
            startedAt = Time.unscaledTime;
            BuildLevel(levelIndex);
        }

        private void Update()
        {
            if (solved || runtime == null || runtime.State != LevelState.Playing) return;

            if (TryGetPointerDown(out var screenPosition))
            {
                pointerStart = WorldPoint(screenPosition);
                draggedNode = FindNode(pointerStart);
                if (draggedNode != null && draggedNode.draggable) draggedNode.BeginDrag();
            }

            if (draggedNode != null && TryGetPointerHeld(out screenPosition))
            {
                draggedNode.transform.position = WorldPoint(screenPosition);
            }

            if (draggedNode != null && TryGetPointerUp(out screenPosition))
            {
                var releasePoint = WorldPoint(screenPosition);
                var distance = Vector2.Distance(pointerStart, releasePoint);
                if (distance < 0.22f) HandleTap(draggedNode);
                else HandleDrop(draggedNode, releasePoint);
                draggedNode.EndDrag();
                draggedNode = null;
            }
            else if (draggedNode == null && TryGetPointerUp(out screenPosition))
            {
                var node = FindNode(WorldPoint(screenPosition));
                if (node != null) HandleTap(node);
            }
        }

        public void UseHint()
        {
            if (solved) return;
            hintCount++;
            if (AudioDirector.Instance != null) AudioDirector.Instance.Hint();
            var progression = FindFirstObjectByType<ProgressionService>();
            if (progression != null && progression.Coins >= 5) progression.SpendCoins(5);
        }

        private void BuildLevel(int index)
        {
            ClearBoard();
            CreateBlock("Background", new Vector2(0f, 0f), new Vector2(boardWidth, boardHeight), Navy, -10);
            CreateBlock("Floor", new Vector2(0f, -5.35f), new Vector2(7.4f, 0.45f), Slate, -1);
            CreateBlock("TopRail", new Vector2(0f, 5.35f), new Vector2(7.4f, 0.25f), Slate, -1);

            var variation = (index - 1) % 6;
            var keyPosition = new Vector2(-2.25f + (variation % 3) * 0.45f, 1.0f - (variation / 3) * 1.2f);
            var doorPosition = new Vector2(2.25f, -3.5f + (variation % 2) * 0.75f);
            CreateNode("Key", keyPosition, new Vector2(0.78f, 0.78f), Amber, NodeKind.Key, false);
            CreateNode("Door", doorPosition, new Vector2(1.35f, 2.15f), Purple, NodeKind.Door, false);

            var decoyCount = 1 + Mathf.Min(4, (index - 1) / 6);
            for (var i = 0; i < decoyCount; i++)
            {
                var x = -2.3f + (i % 3) * 2.0f;
                var y = -1.1f + (i / 3) * 1.45f;
                CreateNode("Decoy_" + i, new Vector2(x, y), new Vector2(0.75f, 0.75f), Teal, NodeKind.Decoy, i % 2 == 1);
            }

            if (index >= 7)
            {
                CreateNode("Switch", new Vector2(0f, 3.0f), new Vector2(1.1f, 0.35f), Teal, NodeKind.Switch, false);
            }

            if (index >= 13)
            {
                CreateNode("MovableBlock", new Vector2(-0.6f, -2.3f), new Vector2(1.2f, 0.8f), Slate, NodeKind.Block, true);
            }
        }

        private void HandleTap(PuzzleNode node)
        {
            if (node == null || solved) return;
            if (AudioDirector.Instance != null) AudioDirector.Instance.Tap();

            switch (node.kind)
            {
                case NodeKind.Key:
                    hasKey = true;
                    node.gameObject.SetActive(false);
                    if (AudioDirector.Instance != null) AudioDirector.Instance.KeyPickup();
                    break;
                case NodeKind.Door:
                    if (hasKey)
                    {
                        Complete();
                    }
                    else
                    {
                        node.PulseInvalid();
                        if (AudioDirector.Instance != null) AudioDirector.Instance.Invalid();
                    }
                    break;
                case NodeKind.Switch:
                    node.ToggleVisual();
                    break;
                case NodeKind.Decoy:
                    node.PulseInvalid();
                    if (AudioDirector.Instance != null) AudioDirector.Instance.Invalid();
                    break;
            }
        }

        private void HandleDrop(PuzzleNode node, Vector2 position)
        {
            if (node == null) return;
            if (node.kind == NodeKind.Block && Vector2.Distance(position, new Vector2(0f, -1f)) < 1.0f)
            {
                node.transform.position = new Vector2(0f, -1f);
                if (AudioDirector.Instance != null) AudioDirector.Instance.Tap();
            }
            else if (node.kind == NodeKind.Block)
            {
                node.transform.position = node.startPosition;
                if (AudioDirector.Instance != null) AudioDirector.Instance.Invalid();
            }
        }

        private void Complete()
        {
            if (solved) return;
            solved = true;
            var elapsed = Time.unscaledTime - startedAt;
            var stars = runtime != null ? runtime.CalculateStars(elapsed, hintCount) : 3;
            var progression = FindFirstObjectByType<ProgressionService>();
            var reward = runtime != null ? runtime.CalculateCoinReward(stars) : 10 + stars * 2;
            if (progression != null) progression.CompleteLevel(levelIndex, stars, reward);
            if (AudioDirector.Instance != null)
            {
                AudioDirector.Instance.DoorUnlock();
                AudioDirector.Instance.Success();
            }
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

        private PuzzleNode CreateNode(string name, Vector2 position, Vector2 size, Color color, NodeKind kind, bool draggable)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(levelRoot != null ? levelRoot : transform, false);
            obj.transform.position = position;
            obj.transform.localScale = new Vector3(size.x, size.y, 1f);
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = squareSprite;
            renderer.color = color;
            renderer.sortingOrder = 2;
            var collider = obj.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            var node = obj.AddComponent<PuzzleNode>();
            node.Configure(kind, draggable, position, size, renderer, collider);
            nodes.Add(node);
            return node;
        }

        private void CreateBlock(string name, Vector2 position, Vector2 size, Color color, int sortingOrder)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(levelRoot != null ? levelRoot : transform, false);
            obj.transform.position = position;
            obj.transform.localScale = new Vector3(size.x, size.y, 1f);
            var renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = squareSprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
        }

        private void ClearBoard()
        {
            nodes.Clear();
            var root = levelRoot != null ? levelRoot : transform;
            for (var i = root.childCount - 1; i >= 0; i--) Destroy(root.GetChild(i).gameObject);
        }

        private Vector2 WorldPoint(Vector2 screenPosition)
        {
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

        private static bool TryGetPointerDown(out Vector2 position)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                position = Input.GetTouch(0).position;
                return true;
            }
            position = Input.mousePosition;
            return Input.GetMouseButtonDown(0);
        }

        private static bool TryGetPointerHeld(out Vector2 position)
        {
            if (Input.touchCount > 0)
            {
                var phase = Input.GetTouch(0).phase;
                position = Input.GetTouch(0).position;
                return phase == TouchPhase.Moved || phase == TouchPhase.Stationary;
            }
            position = Input.mousePosition;
            return Input.GetMouseButton(0);
        }

        private static bool TryGetPointerUp(out Vector2 position)
        {
            if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled))
            {
                position = Input.GetTouch(0).position;
                return true;
            }
            position = Input.mousePosition;
            return Input.GetMouseButtonUp(0);
        }

        private enum NodeKind { Key, Door, Switch, Decoy, Block }

        private sealed class PuzzleNode : MonoBehaviour
        {
            public NodeKind kind;
            public bool draggable;
            public Vector2 startPosition;
            private Vector2 size;
            private SpriteRenderer nodeRenderer;
            private BoxCollider2D nodeCollider;
            private Color originalColor;

            public void Configure(NodeKind nodeKind, bool canDrag, Vector2 start, Vector2 nodeSize, SpriteRenderer nodeRenderer, BoxCollider2D nodeCollider)
            {
                kind = nodeKind;
                draggable = canDrag;
                startPosition = start;
                size = nodeSize;
                this.nodeRenderer = nodeRenderer;
                this.nodeCollider = nodeCollider;
                originalColor = this.nodeRenderer.color;
            }

            public bool Contains(Vector2 point)
            {
                return nodeCollider != null && nodeCollider.bounds.Contains(point);
            }

            public void BeginDrag()
            {
                nodeRenderer.color = Color.Lerp(originalColor, Color.white, 0.25f);
            }

            public void EndDrag()
            {
                nodeRenderer.color = originalColor;
            }

            public void ToggleVisual()
            {
                nodeRenderer.color = nodeRenderer.color == Color.white ? originalColor : Color.white;
            }

            public void PulseInvalid()
            {
                nodeRenderer.color = Color.Lerp(originalColor, Color.red, 0.35f);
                Invoke(nameof(ResetColor), 0.16f);
            }

            private void ResetColor()
            {
                if (nodeRenderer != null) nodeRenderer.color = originalColor;
            }
        }
    }
}
