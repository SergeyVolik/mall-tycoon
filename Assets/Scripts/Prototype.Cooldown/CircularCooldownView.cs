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
            cooldownRoot.gameObject.SetActive(!m_Coolodwn.IsFinished);
            progressImage.fillAmount = m_Coolodwn.Progress / m_Coolodwn.Duration;
        }
    }
}