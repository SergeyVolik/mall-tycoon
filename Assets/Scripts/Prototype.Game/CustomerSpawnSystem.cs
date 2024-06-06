using System;
using UnityEngine;
using UnityEngine.Assertions;

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
            Assert.IsFalse(IsMaxLevel(), "error: level is max");
                

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
    public class CustomerSpawnSystem : MonoBehaviour
    {
        public GameObject customerPrefab;

        public UpgradeData customerSpawnSpeed;
        public UpgradeData customerMoveSpeed;

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
            var spawnPoint = customerSpawnPoints[UnityEngine.Random.Range(0, customerSpawnPoints.Length)];
            var customer = GameObject.Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
            var customerSkin = customer.GetComponent<SpawnRandomSkin>();
            customerSkin.SpawnSkin();
            var customerAI = customer.GetComponent<CustomerAI>();
            customerAI.SetMoveSpeed(customerMoveSpeed.GetValue());
        }
    }
}
