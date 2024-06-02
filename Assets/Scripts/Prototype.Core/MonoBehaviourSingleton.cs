using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T GetInstance()
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>(includeInactive:true);

                if(instance)
                    DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }

        public static T GetOrCreateInstance()
        {
            if (instance == null)
            {
                GetInstance();

                if (instance == null)
                {
                    var go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();
                    DontDestroyOnLoad(instance.gameObject);
                }
            }

            return instance;
        }
    }
}
