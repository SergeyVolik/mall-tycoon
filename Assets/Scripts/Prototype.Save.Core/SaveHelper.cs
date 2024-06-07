using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prototype
{
    public static class SaveHelper
    {
        public static void LoadComponent<T, T2>(T loadCompData) where T : ISaveComponentData where T2 : MonoBehaviour, ISceneSaveComponent<T>
        {
            IEnumerable<ISceneSaveComponent<T>> components = GameObject.FindObjectsByType<T2>(FindObjectsInactive.Include, FindObjectsSortMode.None);
           
            var comp = components.FirstOrDefault((item) => loadCompData.SaveId == item.SaveId);

            if (comp != null)
            {
                comp.LoadComponent(loadCompData);
            }           
        }

        public static void LoadComponents<T, T2>(IEnumerable<T> loadData) where T : ISaveComponentData where T2 : MonoBehaviour, ISceneSaveComponent<T>
        {
            IEnumerable<ISceneSaveComponent<T>> components = GameObject.FindObjectsByType<T2>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var loadCompData in loadData)
            {
                var comp = components.FirstOrDefault((item) => {
                    //Debug.Log($"{loadCompData.Id.ToString()} == {item.Id.ToString()}");
                    return loadCompData.SaveId == item.SaveId; 
                });

                if (comp != null)
                {
                    comp.LoadComponent(loadCompData);
                }
            }
        }

        public static List<T> SaveComponents<T, T2>() where T : ISaveComponentData where T2 : MonoBehaviour, ISceneSaveComponent<T>
        {
            IEnumerable<ISceneSaveComponent<T>> components = GameObject.FindObjectsByType<T2>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            List<T> saves = new List<T>();

            foreach (var item in components)
            {
                var save = item.SaveComponent();
                save.SaveId = item.SaveId;
                saves.Add(save);
            }

            return saves;
        }

        public static T SaveComponent<T, T2>() where T : ISaveComponentData where T2 : MonoBehaviour, ISceneSaveComponent<T>
        {
            ISceneSaveComponent<T> components = GameObject.FindObjectOfType<T2>(true);
            var save = components.SaveComponent();
            save.SaveId = components.SaveId;
            return save;
        }
    }
}