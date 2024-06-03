using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public class Cooldown : ICooldown
    {
        [field: SerializeField]
        public float Duration { get; set; }

        public float Progress { get; private set; }

        public bool IsFinished => m_Finished;

        public bool IsPaused => m_IsPaused;

        bool m_IsPaused;
        bool m_Finished = true;

        public void Play()
        {
            m_IsPaused = false;
        }

        public void Restart()
        {
            Progress = 0;
            m_Finished = false;
            m_IsPaused = false;
        }

        public void Stop()
        {
            m_IsPaused = true;
        }

        public void Tick(float deltaTime)
        {
            if (m_IsPaused)
                return;

            if (m_Finished)
                return;

            Progress += deltaTime;

            m_Finished = Progress >= Duration;
        }
    }
}
