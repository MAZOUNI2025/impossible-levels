using UnityEngine;
using UnityEngine.UI;
using ImpossibleLevels.Core;

namespace ImpossibleLevels.UI
{
    public sealed class PlayerProfilePanel : MonoBehaviour
    {
        [SerializeField] private Text completedLevelsLabel;
        [SerializeField] private Text starsLabel;
        [SerializeField] private Text coinsLabel;
        [SerializeField] private Text rankLabel;
        [SerializeField] private int totalLevels = 30;

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            var progression = FindFirstObjectByType<ProgressionService>();
            var profile = FindFirstObjectByType<PlayerProfileService>();
            if (profile != null) profile.RefreshTotals(totalLevels);

            var completed = profile != null ? profile.CompletedLevels : 0;
            var stars = profile != null ? profile.TotalStars : 0;
            var coins = progression != null ? progression.Coins : 0;
            var rank = GetRank(stars);

            if (completedLevelsLabel != null) completedLevelsLabel.text = $"{completed}/{totalLevels}";
            if (starsLabel != null) starsLabel.text = stars.ToString();
            if (coinsLabel != null) coinsLabel.text = coins.ToString();
            if (rankLabel != null) rankLabel.text = rank;
        }

        private static string GetRank(int stars)
        {
            if (stars >= 75) return "Master of Impossible";
            if (stars >= 45) return "Pattern Breaker";
            if (stars >= 20) return "Door Seeker";
            return "First Attempt";
        }
    }
}
