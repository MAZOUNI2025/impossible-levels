using UnityEngine;

namespace ImpossibleLevels.Core
{
    public sealed class ProgressionService : MonoBehaviour
    {
        private const string HighestUnlockedKey = "il.highest_unlocked";
        private const string CoinsKey = "il.coins";
        private const string LevelStarsPrefix = "il.level_stars.";

        public int HighestUnlockedLevel { get; private set; }
        public int Coins { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Load();
        }

        public bool IsUnlocked(int levelIndex)
        {
            return levelIndex <= HighestUnlockedLevel;
        }

        public int CompleteLevel(int levelIndex, int stars, int coinReward)
        {
            var previousStars = PlayerPrefs.GetInt(LevelStarsPrefix + levelIndex, 0);
            var firstClear = previousStars <= 0;
            if (stars > previousStars)
            {
                PlayerPrefs.SetInt(LevelStarsPrefix + levelIndex, Mathf.Clamp(stars, 0, 3));
            }

            if (levelIndex >= HighestUnlockedLevel)
            {
                HighestUnlockedLevel = levelIndex + 1;
                PlayerPrefs.SetInt(HighestUnlockedKey, HighestUnlockedLevel);
            }

            var grantedReward = firstClear ? Mathf.Max(0, coinReward) : 0;
            Coins += grantedReward;
            PlayerPrefs.SetInt(CoinsKey, Coins);
            PlayerPrefs.Save();
            return grantedReward;
        }

        public int GetStars(int levelIndex)
        {
            return PlayerPrefs.GetInt(LevelStarsPrefix + levelIndex, 0);
        }

        public void SpendCoins(int amount)
        {
            if (amount <= 0 || Coins < amount)
            {
                return;
            }

            Coins -= amount;
            PlayerPrefs.SetInt(CoinsKey, Coins);
            PlayerPrefs.Save();
        }

        public void ResetAllProgress()
        {
            HighestUnlockedLevel = 1;
            Coins = 0;
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt(HighestUnlockedKey, HighestUnlockedLevel);
            PlayerPrefs.SetInt(CoinsKey, Coins);
            PlayerPrefs.Save();
        }

        private void Load()
        {
            HighestUnlockedLevel = Mathf.Max(1, PlayerPrefs.GetInt(HighestUnlockedKey, 1));
            Coins = Mathf.Max(0, PlayerPrefs.GetInt(CoinsKey, 0));
        }
    }
}
