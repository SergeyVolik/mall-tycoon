using UnityEngine;
using UnityEngine.EventSystems;

namespace Prototype
{
    public interface IHoldButton : IPointerDownHandler, IPointerUpHandler
    {
        public bool IsPressed { get; }
    }

    public class HoldedButton : MonoBehaviour, IHoldButton
    {
        public bool IsPressed { get; private set; }
        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
        }
    }
}
