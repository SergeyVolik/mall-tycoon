using UnityEngine;

namespace Prototype
{
    public class CustomerSpawnSystem : MonoBehaviour
    {
        public GameObject customerPrefab;

        public float spawnIntervalMin;
        public float spawnIntervalMax;
        private float spawnInterval;

        private void Awake()
        {
            spawnInterval = GetNextSpawnInterval();
        }

        private float GetNextSpawnInterval()
        {
            return Random.Range(spawnIntervalMin, spawnIntervalMax);
        }

        public float t;
        public Transform[] customerSpawnPoints;
        private void Update()
        {
            t += Time.deltaTime;

            if (t > spawnInterval)
            {
                spawnInterval = GetNextSpawnInterval();
                t = 0;
                var spawnPoint = customerSpawnPoints[Random.Range(0, customerSpawnPoints.Length)];
                GameObject.Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }
}
