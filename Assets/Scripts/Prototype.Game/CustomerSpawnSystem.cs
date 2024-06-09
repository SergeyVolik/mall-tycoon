using System;
using System.Linq;
using UnityEngine;

namespace Prototype
{
    public enum UpgradeOp
    {
        Increase,
        Decrease,
        Mult,
        Divide
    }

    [System.Serializable]
    public class UpgradeData
    {
        public UpgradeOp upgradeOp;
        public float defaultValue;
        public float changeValue;
        public int maxLevel;
        public int currentLevel;
        public float upgradeDefaultCost;
        public UpgradeOp upgradeCostOp;
        public float upgradeCostChangeValue;

        public event Action onChanged = delegate { };

        public void LevelUp()
        {
            if (currentLevel == maxLevel)
                return;

            PlayerData.GetInstance().DecreaseMoney(GetCostValue());
            currentLevel++;
            onChanged.Invoke();
        }

        public void ForceChangeEvent()
        {
            onChanged.Invoke();
        }

        public bool IsMaxLevel() => currentLevel == maxLevel;

        public float GetValue(UpgradeOp upgradeOp, float defaultValue, float changeValue, float currentLevel)
        {
            switch (upgradeOp)
            {
                case UpgradeOp.Increase:
                    return defaultValue + changeValue * currentLevel;

                case UpgradeOp.Decrease:
                    return defaultValue - changeValue * currentLevel;

                case UpgradeOp.Mult:
                    return defaultValue * changeValue * currentLevel;

                case UpgradeOp.Divide:
                    return defaultValue / changeValue * currentLevel;
                default:
                    break;
            }

            return 0;
        }

        public float GetValue()
        {
            return GetValue(upgradeOp, defaultValue, changeValue, currentLevel);
        }

        public float GetCostValue()
        {
            return GetValue(upgradeCostOp, upgradeDefaultCost, upgradeCostChangeValue, currentLevel);
        }
    }

    [System.Serializable]
    public class CustomerSpawnerSave : ISaveComponentData
    {
        public UpgradeData customerSpawnSpeed;
        public UpgradeData customerMoveSpeed;

        public SerializableGuid SaveId { get; set; }
    }

    public class CustomerSpawnSystem : Singleton<CustomerSpawnSystem>, ISceneSaveComponent<CustomerSpawnerSave>
    {
        [field: SerializeField]
        public SerializableGuid SaveId { get; set; }

        public GameObject customerPrefab;

        public UpgradeData customerSpawnSpeed;
        public UpgradeData customerMoveSpeed;
        public int maxActiveCustomes = 50;
        public float SpawnsPerMinute()
        {
            return 60f / customerSpawnSpeed.GetValue();
        }

        private float m_SpawnT;
        public Transform[] customerSpawnPoints;

        private void Update()
        {
            m_SpawnT += Time.deltaTime;

            if (m_SpawnT < customerSpawnSpeed.GetValue())
            {
                return;
            }

            m_SpawnT = 0;

            if (Market.GetInstance().GetReadyTraders().Count() > 0 && CustomerAIBehaviour.GetInstance().ActiveCustomers < maxActiveCustomes)
            {              
                var spawnPoint = customerSpawnPoints[UnityEngine.Random.Range(0, customerSpawnPoints.Length)];
                var customer = GameObject.Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
                var customerSkin = customer.GetComponent<SpawnRandomSkin>();
                customerSkin.SpawnSkin();
                var customerAI = customer.GetComponent<CustomerAI>();
                customerAI.SetMoveSpeed(customerMoveSpeed.GetValue() + UnityEngine.Random.Range(-0.1f, 0.1f));
            }
        }

        public CustomerSpawnerSave SaveComponent()
        {
            return new CustomerSpawnerSave
            {
                customerMoveSpeed = customerMoveSpeed,
                customerSpawnSpeed = customerSpawnSpeed
            };
        }

        public void LoadComponent(CustomerSpawnerSave data)
        {
            customerMoveSpeed = data.customerMoveSpeed;
            customerSpawnSpeed = data.customerSpawnSpeed;
        }
    }
}
