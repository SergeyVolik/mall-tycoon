using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class PlayerResourceView : MonoBehaviour
    {
        public ResourceUIItem uiItem;
        public ResourceTypeSO resourceType;

        private void Start()
        {        
            Bind(PlayerData.GetInstance().Resources.resources);       
        }

        private ResourceContainer m_Resources;

        public void Bind(ResourceContainer resources)
        {
            m_Resources = resources;
            m_Resources.onResourceChanged += UpdateResourceUI;
            UpdateUI();
        }

        private void OnDestroy()
        {
            m_Resources.onResourceChanged -= UpdateResourceUI;
        }

        private void UpdateResourceUI(ResourceTypeSO arg1, float arg2)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            var count = m_Resources.GetResource(resourceType);
            uiItem.SetText(TextUtils.SplitBy3Number(count));
            uiItem.SetSprite(resourceType.resourceIcon, resourceType.resourceColor);
        }
    }
}
