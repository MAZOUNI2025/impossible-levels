using System.Collections.Generic;
using UnityEngine;

namespace ImpossibleLevels.UI
{
    public static class ArtAssetLibrary
    {
        private static readonly Dictionary<string, Sprite> cache = new();
        private static readonly HashSet<string> missingArt = new();

        public static Sprite GetUiIcon(string iconName)
        {
            return LoadSprite("Art/UI/ui_" + iconName.ToLowerInvariant());
        }

        public static Sprite GetLevelThumbnail(int levelIndex)
        {
            var clamped = Mathf.Clamp(levelIndex, 1, 30);
            return LoadSprite("Art/Levels/level_" + clamped.ToString("00"));
        }

        public static Sprite GetGameplaySprite(string gameplayKind)
        {
            var normalized = gameplayKind.ToLowerInvariant();
            var sprite = LoadSprite("Art/Gameplay/" + normalized, false);
            if (sprite != null) return sprite;

            var key = "Gameplay/" + gameplayKind;
            if (missingArt.Add(key))
            {
                Debug.LogWarning("IMPOSSIBLE LEVELS Missing Art: " + key + ". Procedural fallback remains active.");
            }

            return null;
        }

        private static Sprite LoadSprite(string resourcesPath, bool logMissing = true)
        {
            if (cache.TryGetValue(resourcesPath, out var cached)) return cached;

            var sprite = Resources.Load<Sprite>(resourcesPath);
            if (sprite == null)
            {
                var texture = Resources.Load<Texture2D>(resourcesPath);
                if (texture != null)
                {
                    sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                }
            }

            cache[resourcesPath] = sprite;
            if (sprite == null && logMissing && missingArt.Add("Resource/" + resourcesPath))
            {
                Debug.LogWarning("IMPOSSIBLE LEVELS missing art resource: Resources/" + resourcesPath);
            }

            return sprite;
        }
    }
}
