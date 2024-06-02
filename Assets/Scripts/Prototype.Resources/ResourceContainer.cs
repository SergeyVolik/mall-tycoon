using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Prototype
{
    [System.Serializable]
    public class ResourceItem : IEquatable<ResourceItem>
    {
        public int count;
        public ResourceTypeSO resourceType;

        public bool Equals(ResourceItem other)
        {
            return count == other.count && resourceType == other.resourceType;
        }
    }

    [System.Serializable]
    public class ResourceContainer : IEquatable<ResourceContainer>
    {
        [SerializeField]
        private ResourceItem[] m_InitResources;
        bool m_Inited = false;

        private Dictionary<ResourceTypeSO, int> m_ResourceDic = new Dictionary<ResourceTypeSO, int>();
        public IEnumerable<KeyValuePair<ResourceTypeSO, int>> ResourceIterator() => ResourceDic;

        public event Action<ResourceTypeSO, int> onResourceChanged = delegate { };

        private Dictionary<ResourceTypeSO, int> ResourceDic
        {
            get
            {
                if (!m_Inited)
                {
                    Init();
                }

                return m_ResourceDic;
            }
        }

        public void Init(ResourceItem[] initResources)
        {
            m_Inited = true;
            m_ResourceDic.Clear();
            if (initResources != null)
            {
                foreach (ResourceItem item in initResources)
                {
                    m_ResourceDic.Add(item.resourceType, item.count);
                }
            }
        }

        private void Init()
        {
            Init(m_InitResources);
        }

        public void SetResource(ResourceTypeSO resourceType, int count)
        {
            ResourceDic[resourceType] = count;
            onResourceChanged.Invoke(resourceType, count);
        }

        public void AddResource(ResourceTypeSO resourceType, int count)
        {
            ResourceDic.TryGetValue(resourceType, out var current);
            current += count;
            SetResource(resourceType, current);
        }

        public void RemoveResource(ResourceTypeSO resourceType, int count)
        {
            ResourceDic.TryGetValue(resourceType, out var current);

            Assert.IsTrue(current >= count, $"Can't Remove resource {count} > {current}");

            current -= count;
            SetResource(resourceType, current);
        }

        public bool IsEmpty()
        {
            foreach (var item in ResourceIterator())
            {
                if (item.Value != 0)
                    return false;
            }

            return true;
        }

        public void Clear()
        {
            ResourceDic.Clear();
        }

        public int GetResource(ResourceTypeSO resourceType)
        {
            ResourceDic.TryGetValue(resourceType, out int result);
            return result;
        }

        public ResourceContainer DeepClone()
        {
            var container = new ResourceContainer();

            foreach (var item in ResourceDic)
            {
                container.SetResource(item.Key, item.Value);
            }

            return container;
        }

        public bool Equals(ResourceContainer other)
        {
            if (other.ResourceDic.Count != ResourceDic.Count)
                return false;

            int equalItems = 0;

            foreach (var item in ResourceDic)
            {
               var count = other.GetResource(item.Key);

                if(count == item.Value)
                    equalItems++;
            }

            return equalItems == ResourceDic.Count;
        }
    }
}
