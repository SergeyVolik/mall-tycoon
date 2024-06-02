using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class RequiredResourceView : MonoBehaviour
    {
        public TextMeshProUGUI title;

        public Transform itemParent;
        private ResourceContainer m_CurrentResources;
        private ResourceContainer m_RequiredResources;

        public GameObject m_ResourceUIItemPrefab;

        public Dictionary<ResourceTypeSO, RquiredResourceUIItem> uiItems = new Dictionary<ResourceTypeSO, RquiredResourceUIItem>();

        public event Action<ResourceTypeSO, RquiredResourceUIItem> onItemAdded = delegate { };

        public Button finishButton;
        public GameObject finishButtonHolder;

        public void SetButtonText(string text)
        {
            finishButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }

        private void Awake()
        {
            finishButtonHolder.SetActive(false);
            finishButton.onClick.AddListener(() => onFinished?.Invoke());
        }

        public event Action onFinished;
        public void Bind(ResourceContainer required, ResourceContainer current)
        {
            m_CurrentResources = current;
            m_RequiredResources = required;
            Setup();
            m_CurrentResources.onResourceChanged += UpdateResourceUI;
            m_RequiredResources.onResourceChanged += UpdateResourceUI;

            CheckFinish();
        }

        private void OnEnable()
        {
            if (m_CurrentResources != null)
            {
                m_CurrentResources.onResourceChanged += UpdateResourceUI;
                m_RequiredResources.onResourceChanged += UpdateResourceUI;
            }
        }

        private void OnDisable()
        {
            m_CurrentResources.onResourceChanged -= UpdateResourceUI;
            m_RequiredResources.onResourceChanged -= UpdateResourceUI;
        }
        private void UpdateResourceUI(ResourceTypeSO arg1, int arg2)
        {
            uiItems.TryGetValue(arg1, out var item);

            var req = GetStringValue(m_RequiredResources.GetResource(arg1));
            var current = GetStringValue(m_CurrentResources.GetResource(arg1));
            item.SetText($"<size=100%>{current}<size=50%>/{req}");

            item.addResource.gameObject.SetActive(req != current);
            CheckFinish();
        }

        private void CheckFinish()
        {
            if (m_CurrentResources.Equals(m_RequiredResources))
            {
                finishButtonHolder.SetActive(true);
            }
        }

        private string GetStringValue(int numberOfIntems)
        {
            return TextUtils.IntToText(numberOfIntems);
        }

        private void Setup()
        {
            foreach (var item in m_RequiredResources.ResourceIterator())
            {
                SetupUIItem(item.Key, item.Value);
            }
        }

        private void SetupUIItem(ResourceTypeSO type, int count)
        {
            var uiItem = GameObject
                .Instantiate(m_ResourceUIItemPrefab, itemParent)
                .GetComponent<RquiredResourceUIItem>();
            finishButtonHolder.transform.SetAsLastSibling();
            uiItems.Add(type, uiItem);
            uiItem.SetSprite(type.resourceIcon, type.resourceColor);
            UpdateResourceUI(type, count);

            onItemAdded.Invoke(type, uiItem);
        }
    }
}
