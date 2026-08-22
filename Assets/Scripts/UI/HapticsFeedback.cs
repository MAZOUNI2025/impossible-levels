using UnityEngine;
using ImpossibleLevels.Core;

namespace ImpossibleLevels.UI
{
    public static class HapticsFeedback
    {
        public static void TryPulse()
        {
            if (!Application.isMobilePlatform || !SystemInfo.supportsVibration) return;

            var profile = Object.FindFirstObjectByType<PlayerProfileService>();
            if (profile != null && !profile.HapticsEnabled) return;

            Handheld.Vibrate();
        }
    }
}
