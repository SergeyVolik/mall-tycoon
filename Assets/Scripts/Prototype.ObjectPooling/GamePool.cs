using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class GameObjectPool
    {
        private static Dictionary<GameObject, GameObjectPool> Pools = new Dictionary<GameObject, GameObjectPool>();
        public GameObject prefab;
        private Stack<GameObject> poolItems = new Stack<GameObject>();

        private Transform m_PoolRoot;
        private Transform PoolRoot
        {
            get
            {
                if (m_PoolRoot == null)
                {
                    var go = new GameObject($"pool: {prefab.name}");

                    m_PoolRoot = go.GetComponent<Transform>();
                }

                return m_PoolRoot;
            }
        }

        public static T GetPoolObject<T>(T prefab) where T : Component
        {
            var obj = GetPoolObject(prefab.gameObject);

            return obj.GetComponent<T>();
        }

        public static GameObject GetPoolObject(GameObject prefab)
        {
            if (Pools.TryGetValue(prefab, out var pool))
            {
                return pool.GetItem();
            }

            pool = new GameObjectPool();

            pool.prefab = prefab;
            Pools.Add(prefab, pool);
            return pool.GetItem();
        }

        GameObject GetItem()
        {
            GameObject item = null;

            while (poolItems.Count > 0)
            {
                item = poolItems.Pop();
                if (item != null)
                {
                    break;
                }
            }
            if (item == null)
            {
                PrepareItems(1);
                item = poolItems.Pop();
            }

            item.GetComponent<PoolObject>().isReleased = false;

            return item;
        }

        public void PrepareItems(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var item = GameObject.Instantiate(prefab);
                item.SetActive(false);
                var poolRef = item.AddComponent<PoolObject>();

                poolRef.isReleased = true;
                poolRef.poolRef = this;
                poolRef.onDisabled += () =>
                {
                    Release(item);
                };
                item.transform.SetParent(PoolRoot);

                poolItems.Push(item);
            }         
        }

        public void Release(GameObject item)
        {
            poolItems.Push(item);
            item.GetComponent<PoolObject>().isReleased = true;
            //item.SetActive(true);
            //item.transform.SetParent(m_PoolRoot);
            item.SetActive(false);
        }
    }

    public class PoolObject : MonoBehaviour
    {
        public event Action onDisabled = delegate { };
        public GameObjectPool poolRef;

        [HideInInspector]
        public bool isReleased;

        private void OnDisable()
        {
            if (!isReleased)
            {
                onDisabled.Invoke();
            }
        }

        public void Release()
        {
            poolRef.Release(gameObject);
        }
    }
}

