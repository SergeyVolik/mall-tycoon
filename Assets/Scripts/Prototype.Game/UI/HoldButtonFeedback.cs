using Prototype.Audio;
using UnityEngine;

namespace Prototype
{
    public class HoldButtonFeedback : MonoBehaviour
    {
        public PlaySFXData sfxData;
        private AudioSource m_Source;

        private void Awake()
        {       
            var button = GetComponentInParent<HoldedButton>();
            button.onClick += HoldButtonFeedback_onClick;
            button.onStarted += Button_onStarted;
            button.onFinished += Button_onFinished;
        }

        private void Button_onFinished()
        {
            m_Source = null;          
        }

        private void Button_onStarted()
        {
            AudioSourcePool.GetInstance().Get(out m_Source);
        }

        private AudioSource GetSource()
        {
            if (m_Source == null)
            {
                AudioSourcePool.GetInstance().Get(out m_Source);
                m_Source.gameObject.SetActive(true);
            }
            if (m_Source.isPlaying)
            {
                m_Source.gameObject.SetActive(true);
                return m_Source;
            }

            return m_Source;
        }

        private void HoldButtonFeedback_onClick()
        {
            sfxData.Play(GetSource());
        }
    }
}
