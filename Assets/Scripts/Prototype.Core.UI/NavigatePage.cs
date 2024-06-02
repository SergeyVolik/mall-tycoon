using UnityEngine;
using UnityEngine.UI;

namespace Prototype.UI
{
    [RequireComponent(typeof(Button))]
    public class NavigatePage : MonoBehaviour
    {
        public UIPage page;
        public bool addtive;
        public bool clearNavManager;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(Navigate);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(Navigate);
        }

        public void Navigate()
        {
            if (clearNavManager)
            {
                UINavigationManager.GetInstance().PopAll();
            }

            UINavigationManager.GetInstance().Navigate(page, addtive);
        }
    }
}
