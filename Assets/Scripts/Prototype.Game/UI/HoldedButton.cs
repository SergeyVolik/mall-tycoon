using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype
{
    public interface IHoldButton : IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public bool IsPressed { get; }
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

    public class HoldedButton : MonoBehaviour, IHoldButton
    {
        public bool IsPressed { get; private set; }

        private float currentSpeed = 1f;
        private float speedAcceleration = 1.5f;
        private const float duration = 1f;

        private float t;
        private RectTransform m_Rect;

        public event Action onClick = delegate { };

        private void Awake()
        {
            m_Rect = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            currentSpeed = 1f;
            t = 0;
            IsPressed = true;
            onClick.Invoke();

            Debug.Log("OnPointerDown");
            HoldButtonTickManager.GetOrCreateInstance().Force(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("OnPointerUp");

            IsPressed = false;
            HoldButtonTickManager.GetOrCreateInstance().StopButton();
        }

        public void Tick()
        {

            t += Time.deltaTime * currentSpeed;

            if (t > duration)
            {
                t = 0;
                onClick.Invoke();
                currentSpeed *= speedAcceleration;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HoldButtonTickManager.GetOrCreateInstance().StopButton();
        }
    }
}
