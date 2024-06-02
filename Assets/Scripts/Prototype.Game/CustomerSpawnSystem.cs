using UnityEngine;

namespace Prototype
{
    public class CustomerSpawnSystem : MonoBehaviour
    {
        public GameObject customerPrefab;

        public float spawnInterval;
        public float t;
        public Transform customerSpawnPoint;
        private void Update()
        {
            t += Time.deltaTime;

            if (t > spawnInterval)
            {
                t = 0;
                GameObject.Instantiate(customerPrefab, customerSpawnPoint.position, Quaternion.identity);
            }
        }
    }
}
