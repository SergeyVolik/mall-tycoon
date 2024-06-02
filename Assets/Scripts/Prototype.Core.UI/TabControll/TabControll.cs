using System.Collections.Generic;
using UnityEngine;

namespace Prototype.UI
{
    [System.Serializable]
    public class TabControllData
    {
        public string Title;
        public TabWindow Window;
        public TabButton Button;
    }

    public class TabControll : MonoBehaviour
    {
        public List<TabControllData> Tabs;

        [SerializeField]
        private int m_StartTabIndex;

        [SerializeField]
        private TabButton buttonPrefab;

        [SerializeField]
        private RectTransform m_ButtonsHolder;

        [SerializeField]
        private RectTransform m_WindowHolder;


        public int MaxTabIndex() => Tabs.Count - 1;
        private void Awake()
        {
            if (Tabs.Count == 0)
                return;

            for (int i = 0; i < Tabs.Count; i++)
            {
                var item = Tabs[i];
                SetupTabWindow(i, item);
            }

            ActivateTabByIndex(m_StartTabIndex);
        }

        private void SetupTabWindow(int i, TabControllData item)
        {
            item.Button.SetText(item.Title);
            item.Window.SetTitle(item.Title);

            item.Button.button.onClick.AddListener(() =>
            {
                ActivateTabByIndex(i);
            });
        }

        public void ActivateTabByIndex(int index)
        {
            foreach (var item in Tabs)
            {
                item.Window.Deactivate();
            }

            Tabs[index].Window.Activate();
        }

        public void ClearContent()
        {
            foreach (var item in Tabs)
            {
                Destroy(item.Window.gameObject);
                Destroy(item.Button.gameObject);
            }

            Tabs.Clear();
        }

        public TabWindow CreateWindow(TabWindow windowPrefab, string title)
        {
            var window = Instantiate(windowPrefab, m_WindowHolder);
            var tabButton = Instantiate(buttonPrefab, m_ButtonsHolder);
            window.transform.parent = m_WindowHolder;
            tabButton.SetText(title);
            windowPrefab.SetTitle(title);
            var item = new TabControllData
            {
                Button = tabButton,
                Window = window,
                Title = title
            };

            Tabs.Add(item);
            SetupTabWindow(MaxTabIndex(), item);
          
            if (Tabs.Count == 1)
            {
                ActivateTabByIndex(0);
            }

            return window;
        }
    }
}
