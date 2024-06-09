using Prototype.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prototype
{
    public interface IHoldButton : IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public event Action onClick;
        public event Action onFinished;
        public event Action onStarted;
    }

    public class HoldButtonTickManager : Singleton<HoldButtonTickManager>
    {
        private HoldedButton m_Button;

        public void Force(HoldedButton button)
        {
            m_Button = button;
        }

        public void StopButton()
        {
            m_Button = null;
        }


        public void Update()
        {
            if(m_Button)
                m_Button.Tick();
        }
    }

    public class HoldedButton : MonoBehaviour, IHoldButton,IPageHidedListener
    {
        private void Awake()
        {
            m_Button = GetComponent<Button>();
        }
        private float currentSpeed = 1f;
        private const float speedAcceleration = 1.5f;
        private const float duration = 1f;
        private const float maxSpeed = 20f;
        private float t;

        public event Action onClick = delegate { };
        public event Action onFinished = delegate { };
        public event Action onStarted = delegate { };

        public UnityEvent onClickUE;
        private Button m_Button;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!m_Button.interactable)
                return;

            currentSpeed = 1f;
            t = 0;

            onStarted.Invoke();
            onClick.Invoke();
          
            HoldButtonTickManager.GetOrCreateInstance().Force(this);
        }

        private void Finish()
        {
            onFinished.Invoke();
            HoldButtonTickManager.GetOrCreateInstance().StopButton();
        }

        private void OnDisable()
        {
            Finish();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Finish();
        }

        public void Tick()
        {
            if (!m_Button.interactable)
            {
                Finish();
            }

            t += Time.deltaTime * currentSpeed;

            if (t > duration)
            {
                t = 0;
                onClick.Invoke();
                currentSpeed *= speedAcceleration;
                currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Finish();
        }

        public void OnHided()
        {
            Finish();
        }
    }
}
