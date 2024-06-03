using UnityEngine;

namespace Prototype
{
    public class CustomerSpawnSystem : MonoBehaviour
    {
        public GameObject customerPrefab;

        public float spawnInterval;
        public float t;
        public Transform[] customerSpawnPoints;
        private void Update()
        {
            t += Time.deltaTime;

            if (t > spawnInterval)
            {
                t = 0;
                var spawnPoint = customerSpawnPoints[Random.Range(0, customerSpawnPoints.Length)];
                GameObject.Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }
}
