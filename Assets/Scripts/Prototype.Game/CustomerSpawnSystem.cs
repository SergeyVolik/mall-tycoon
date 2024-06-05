using System;
using UnityEngine;

namespace Prototype
{
    public class CustomerSpawnSystem : MonoBehaviour
    {
        public GameObject customerPrefab;

        public float spawnInterval;
        public float decreaseSpawnInterval;
        public int maxLevel;
        public int currentLevel;

        public float defaultCustomerMoveSpeed;
        public float customerMoveSpeedLevel;
        public float customerMoveSpeedIncreasePerLevel;

        public float customerMoveMaxSpeedLevel;
        public event Action OnChanged = delegate { };

        public void LevelUpSpawnSpeed()
        {
            currentLevel++;
            OnChanged.Invoke();
        }

        public void LevelUpMoveSpeed()
        {
            customerMoveSpeedLevel++;
            OnChanged.Invoke();
        }

        public float LevelUpSpawnSpeedCost() => 100;
        public float LevelUpMoveSpeedCost() => 100;

        public float GetCurrentMoveSpeed()
        {
            return defaultCustomerMoveSpeed + customerMoveSpeedLevel * customerMoveSpeedIncreasePerLevel;
        }

        public bool IsMaxSpawnLevel() => currentLevel == maxLevel;

        public float SpawnsPerMinute()
        {
            return GetNextSpawnInterval() / 60f;
        }

        private void Awake()
        {
            spawnInterval = GetNextSpawnInterval();
        }

        private float GetNextSpawnInterval()
        {
            return spawnInterval - currentLevel * decreaseSpawnInterval;
        }

        public float t;
        public Transform[] customerSpawnPoints;
        private void Update()
        {
            t += Time.deltaTime;

            if (t < spawnInterval)
            {
                return;
            }

            spawnInterval = GetNextSpawnInterval();
            t = 0;
            var spawnPoint = customerSpawnPoints[UnityEngine.Random.Range(0, customerSpawnPoints.Length)];
            var customer = GameObject.Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
            var customerSkin = customer.GetComponent<SpawnRandomSkin>();
            customerSkin.SpawnSkin();
            var customerAI = customer.GetComponent<CustomerAI>();
            customerAI.SetMoveSpeed(GetCurrentMoveSpeed());
        }
    }
}
