using System;
using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public class WorkerSpeedUpgrade
    {
        public int maxLevel = 10;
        public float workerTime = 2;
        public float workerDecreaseValue = 0.1f;
        public int currentLevel = 1;
        public float currentBuyCost = 10;
        public float buyUpgradeIncreaseValue = 10;

        public event Action onUpgraded = delegate { };

        public bool IsFinished() => maxLevel == currentLevel;

        public void LevelUp()
        {
            if (currentLevel == maxLevel)
            {
                return;   
            }

            currentLevel++;          
            currentBuyCost += buyUpgradeIncreaseValue;
            workerTime -= workerDecreaseValue;

            onUpgraded.Invoke();
        }
        public bool IsMaxLevel() => currentLevel == maxLevel;
        public float GetWorkerTime() => workerTime;
    }

    [System.Serializable]
    public class CostUpgradeData
    {
        public string upgradeUiTitleName;
        public CostUpgradeItem[] upgrades;
        public int currentUpgradeIndex;
        public float producCost = 10;
        public int currentLevel = 1;
        public float currentBuyCost = 10;

        public event Action onUpgraded = delegate { };

        public void LevelUp()
        {
            currentLevel++;
            var currentUpgrade = upgrades[currentUpgradeIndex];
            producCost += currentUpgrade.producCostIncrease;
            currentBuyCost += currentUpgrade.buyUpgradeIncreaseValue;

            if (currentLevel >= currentUpgrade.maxLevel)
            {
                foreach (var item in currentUpgrade.itemsToActivate)
                {
                    item.gameObject.SetActive(true);
                }

                producCost *= currentUpgrade.maxLevelMult;

                if (upgrades.Length - 1 > currentUpgradeIndex)
                {                 
                    currentUpgradeIndex++;
                }
            }

            onUpgraded.Invoke();
        }
        public CostUpgradeItem GetCurrentUpgrade() => upgrades[currentUpgradeIndex];
        public int GetMaxLevel()
        {
            return upgrades[currentUpgradeIndex].maxLevel;
        }

        public float GetProducCost()
        {
            return producCost;
        }

        public bool IsFinished() => upgrades.Length - 1 == currentUpgradeIndex &&
            upgrades[currentUpgradeIndex].maxLevel == currentLevel;

        public int GetPrevMax()
        {
            if (currentUpgradeIndex == 0)
                return 1;

            return upgrades[currentUpgradeIndex - 1].maxLevel;
        }

        public int GetNextMax()
        {
            return upgrades[currentUpgradeIndex].maxLevel;
        }

        public string GetNextUpgradeMult()
        {
            return upgrades[currentUpgradeIndex].maxLevelMult.ToString();
        }

        internal string GetUpgradeName()
        {
            return upgrades[currentUpgradeIndex].name;
        }
    }

    [System.Serializable]
    public class CostUpgradeItem
    {
        public GameObject[] itemsToActivate;
        public int maxLevel;
        public float producCostIncrease;
        public string name;
        public float maxLevelMult;
        public float buyUpgradeIncreaseValue;
    }

    public class TradingSpot : MonoBehaviour, IActivateableFromRaycast
    {
        public string traderName;
        public ResourceTypeSO resourceCost;
        public QueueBehaviour queue;
        public CostUpgradeData costUpgrade;
        public UpgradeData workerSpeedUpgrade;
        public TraderAI[] traders;

        private void Awake()
        {
           
            workerSpeedUpgrade.onChanged += UpdateCooldownSpeed;
            UpdateCooldownSpeed();
        }

        private void UpdateCooldownSpeed()
        {
            foreach (var item in traders)
            {
                item.cooldown.Duration = workerSpeedUpgrade.GetValue();
            }          
        }

        public void Update()
        {
            foreach (var traderAi in traders)
            {
                traderAi.Tick();

                if (traderAi.IsWorkFinished() && !traderAi.IsHasCustomer() && queue.Count != 0)
                {
                    var customer = queue.Dequeue();
                    traderAi.StartWorking(customer);
                    customer.MoveToTrader(traderAi.customerMovePoint.position);
                }
                else if (traderAi.IsWorkFinished() && traderAi.IsHasCustomer())
                {
                    var customerAI = traderAi.CurrentCustomer;
                    customerAI.buyedProducCost = costUpgrade.producCost;
                    customerAI.holdedResource = resourceCost;
                    traderAi.Clear();
                }
            }        
        }

        public void ActivateFromRaycast()
        {
            TraderUpgradeUI.Instance.Bind(this);
            TraderUpgradeUI.Instance.Navigate();
        }
    }
}
