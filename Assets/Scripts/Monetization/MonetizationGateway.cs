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
        bool IsConfigured { get; }
        bool IsAdsRemoved { get; }
        bool IsRewardedAdReady { get; }
        void ShowRewarded(RewardType rewardType, Action<bool> completed);
        void PurchaseRemoveAds(Action<bool> completed);
    }

    public sealed class OfflineMonetizationGateway : MonoBehaviour, IMonetizationGateway
    {
        // This fallback is deliberately fail-closed until a verified provider adapter is installed.
        public bool IsConfigured => false;
        public bool IsAdsRemoved => false;
        public bool IsRewardedAdReady => false;

        public void ShowRewarded(RewardType rewardType, Action<bool> completed)
        {
            // No ad SDK is present. Never grant a reward without a verified ad callback.
            completed?.Invoke(false);
        }

        public void PurchaseRemoveAds(Action<bool> completed)
        {
            // No Play Billing provider is present. Never mark a purchase locally.
            completed?.Invoke(false);
        }
    }
}
