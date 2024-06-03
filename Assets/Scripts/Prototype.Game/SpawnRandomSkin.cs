using UnityEngine;

namespace Prototype
{
    public class SpawnRandomSkin : MonoBehaviour
    {
        public GameObject[] skins;

        private void Awake()
        {
            var skinPrefab = skins[Random.Range(0, skins.Length)];

            GameObject.Instantiate(skinPrefab, transform);
        }
    }
}
