using System;
using UnityEngine;

namespace ImpossibleLevels.Monetization
{
    public enum RewardType
    {
        Hint,
        Continue,
        CoinMultiplier
    }

    public interface IMonetizationGateway
    {
        bool IsAdsRemoved { get; }
        bool IsRewardedAdReady { get; }
        void ShowRewarded(RewardType rewardType, Action<bool> completed);
        void PurchaseRemoveAds(Action<bool> completed);
    }

    public sealed class OfflineMonetizationGateway : MonoBehaviour, IMonetizationGateway
    {
        public bool IsAdsRemoved => PlayerPrefs.GetInt("il.ads_removed", 0) == 1;
        public bool IsRewardedAdReady => false;

        public void ShowRewarded(RewardType rewardType, Action<bool> completed)
        {
            // Development-safe fallback. A production adapter can be added later
            // without changing level or UI code.
            completed?.Invoke(false);
        }

        public void PurchaseRemoveAds(Action<bool> completed)
        {
            completed?.Invoke(false);
        }
    }
}
