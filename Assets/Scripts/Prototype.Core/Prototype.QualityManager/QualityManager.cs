using UnityEngine;

namespace Prototype
{
    public enum GameQuality
    {
        Performance = 0,
        Balanced = 1,
        Max = 2,
    }

    public class QualityManager : Singleton<QualityManager>
    {
        public GameQuality currentQuality;

        const string SaveKey = "GameQualityLevel";
        public bool forceSetting = true;

        public GameQuality forceQuality;

        void Awake()
        {
            LoadSettings();

            if (forceSetting)
            {
                SetQuality(forceQuality);
            }
        }

        void LoadSettings()
        {
            var value = PlayerPrefs.GetInt(SaveKey, 2);

            SetQuality((GameQuality)value);
        }

        void OnDestroy()
        {
            SaveSettings();
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveSettings();
            }
        }

        void SaveSettings()
        {
            PlayerPrefs.SetInt(SaveKey, (int)currentQuality);
        }

        public void SetQuality(GameQuality quality)
        {
            currentQuality = quality;

            //Debug.Log($"Quality Chaged {currentQulity}");

            var targetFps = 30;
            var targetFixedRate = 30;

            QualitySettings.SetQualityLevel((int)quality);

            switch (quality)
            {
                case GameQuality.Performance:
                    targetFps = 60;
                    targetFixedRate = 30;

                    break;
                case GameQuality.Balanced:
                    targetFps = 60;
                    targetFixedRate = 30;

                    break;
                case GameQuality.Max:
                    targetFixedRate = 60;
                    targetFps = 60;
                    break;
            }

            Application.targetFrameRate = targetFps;

            var physicsFrameRate = 1.0f / targetFixedRate;
            //Time.fixedDeltaTime = physicsFrameRate;
        }
    }
}