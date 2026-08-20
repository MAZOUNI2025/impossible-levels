using UnityEngine;
using ImpossibleLevels.Audio;

namespace ImpossibleLevels.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private static GameBootstrap instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureServices();
        }

        private void EnsureServices()
        {
            if (FindFirstObjectByType<TouchInputRouter>() == null)
            {
                var inputObject = new GameObject("TouchInputRouter");
                inputObject.AddComponent<TouchInputRouter>();
                DontDestroyOnLoad(inputObject);
            }

            if (FindFirstObjectByType<ProgressionService>() == null)
            {
                var progressionObject = new GameObject("ProgressionService");
                progressionObject.AddComponent<ProgressionService>();
                DontDestroyOnLoad(progressionObject);
            }

            if (FindFirstObjectByType<PlayerProfileService>() == null)
            {
                var profileObject = new GameObject("PlayerProfileService");
                profileObject.AddComponent<PlayerProfileService>();
                DontDestroyOnLoad(profileObject);
            }

            if (FindFirstObjectByType<AudioDirector>() == null)
            {
                var audioObject = new GameObject("AudioDirector");
                audioObject.AddComponent<AudioDirector>();
                DontDestroyOnLoad(audioObject);
            }
        }
    }
}
