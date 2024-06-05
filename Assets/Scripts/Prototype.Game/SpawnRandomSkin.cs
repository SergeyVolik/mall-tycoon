using UnityEngine;

namespace Prototype
{
    public class SpawnRandomSkin : MonoBehaviour
    {
        public bool spawnOnAwake = true;
        public GameObject[] skins;
       
        private void Awake()
        {
            if (spawnOnAwake)
            {
                SpawnSkin();
            }
        }

        public GameObject SpawnSkin()
        {
            var skinPrefab = skins[Random.Range(0, skins.Length)];
            return GameObject.Instantiate(skinPrefab, transform);
        }
    }
}
