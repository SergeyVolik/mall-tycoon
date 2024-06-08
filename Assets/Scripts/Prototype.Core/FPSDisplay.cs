using System;
using TMPro;
using UnityEngine;

namespace CubeRunner
{
    public class FPSDisplay : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_FPSText;

        private int m_FrameCount;
        private int m_TotalFPS;

        void Update()
        {
            m_FrameCount++;

            m_TotalFPS += (int)Math.Round(1f / Time.unscaledDeltaTime);

            if (m_FrameCount % 60 == 0)
            {
                m_FPSText.text = "FPS : " + m_TotalFPS / m_FrameCount;
                m_TotalFPS = 0;
                m_FrameCount = 0;
            }
        }
    }
}