using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    [System.Serializable]
    public class CircularCooldownView : ICooldoownView
    {
        private ICooldown m_Coolodwn;
        public Image progressImage;
        public Transform cooldownRoot;

        public void Bind(ICooldown cooldown)
        {
            m_Coolodwn = cooldown;
        }

        public void Tick()
        {
            if (m_Coolodwn.IsFinished || m_Coolodwn.IsPaused && cooldownRoot.gameObject.activeSelf)
            {
                cooldownRoot.gameObject.SetActive(false);
            }
            else if (!m_Coolodwn.IsFinished && !m_Coolodwn.IsPaused && !cooldownRoot.gameObject.activeSelf)
            {
                cooldownRoot.gameObject.SetActive(true);
            }

            progressImage.fillAmount = m_Coolodwn.Progress / m_Coolodwn.Duration;
        }
    }
}
