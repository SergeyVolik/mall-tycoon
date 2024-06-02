using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prototype.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UIPage : MonoBehaviour, INavigateable
    {
        private Canvas m_Canvas;
        private GraphicRaycaster m_GraphicRaycaster;
        private IPageHidedListener[] m_Hideale;
        private IPageShowedListener[] m_Showed;
        [SerializeField]
        private bool initPage;

        [SerializeField]
        private GameObject lastFocusedElement;
        bool firstHideExecuted = false;

        public GameObject[] ActivateIfShowed;
        public GameObject[] DeactivateIfHided;

        protected virtual void Awake()
        {
            m_Canvas = GetComponent<Canvas>();
            m_GraphicRaycaster = GetComponent<GraphicRaycaster>();

            m_Hideale = GetComponentsInChildren<IPageHidedListener>();
            m_Showed =  GetComponentsInChildren<IPageShowedListener>();

            Hide();
        }

        protected virtual void OnDestroy() { }

        protected virtual void Start()
        {
            if (initPage)
                UINavigationManager.GetInstance().Navigate(this);
        }
       
        public virtual void Hide(bool onlyDisableRaycast = false)
        {         
            if(EventSystem.current && firstHideExecuted && EventSystem.current.currentSelectedGameObject != null)
                lastFocusedElement = EventSystem.current.currentSelectedGameObject;
           
            if (m_Hideale != null)
            {
                foreach (var item in m_Hideale)
                {
                    item.OnHided();
                }
            }
            ActivateArray(DeactivateIfHided, false);

            firstHideExecuted = true;

            if (onlyDisableRaycast)
            {
                m_GraphicRaycaster.enabled = false;
            }
            else
            {
                if (m_Canvas)
                    m_Canvas.enabled = false;
                if (m_GraphicRaycaster)
                    m_GraphicRaycaster.enabled = false;
            }
        }

        private void ActivateArray(GameObject[] arrayData, bool activate)
        {
            foreach (var item in arrayData)
            {
                item?.SetActive(activate);
            }
        }

        public virtual void Show()
        {
            if(EventSystem.current)
                EventSystem.current.SetSelectedGameObject(lastFocusedElement);

            ActivateArray(ActivateIfShowed, true);
                    
            if (m_Showed != null)
            {
                foreach (var item in m_Showed)
                {
                    item.OnShowed();
                }
            }

            if (m_Canvas)
                m_Canvas.enabled = true;
            if (m_GraphicRaycaster)
                m_GraphicRaycaster.enabled = true;

            transform.SetAsLastSibling();           
        }

        public void Navigate(bool additive = false)
        {
            UINavigationManager.GetInstance().Navigate(this, additive);
        }
        public void PopPage()
        {
            UINavigationManager.GetInstance().Pop();
        }
    }
}
