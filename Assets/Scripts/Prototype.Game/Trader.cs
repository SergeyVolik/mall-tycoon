using System;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    [System.Serializable]
    public class CostUpgradeData
    {
        public CostUpgradeItem[] upgrades;
        public int currentUpgradeIndex;
        public float producCost = 10;
        public int currentLevel = 1;
        public float currentBuyCost = 10;

        public event Action onUpgraded = delegate { };

        public void LevelUpCost()
        {
            currentLevel++;
            var currentUpgrade = upgrades[currentUpgradeIndex];
            PlayerData.GetInstance().DecreaseMoney(currentBuyCost);
            producCost += currentUpgrade.producCostIncrease;
            currentBuyCost += currentUpgrade.buyUpgradeIncreaseValue;
         
            if (currentLevel >= currentUpgrade.maxLevel && upgrades.Length - 1 > currentUpgradeIndex)
            {
                foreach (var item in currentUpgrade.itemsToActivate)
                {
                    item.gameObject.SetActive(true);
                }

                producCost *= currentUpgrade.maxLevelMult;
                currentUpgradeIndex++;             
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

        public bool IsCostUpgradesFinished() => upgrades.Length - 1 == currentUpgradeIndex &&
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

    public class Trader : MonoBehaviour, IActivateableFromRaycast
    {
        public string traderName;
        public ResourceTypeSO resourceCost;
        public QueueBehaviour queue;
        private Transform m_CurrentCustomer;
        private Camera m_Camera;
        public Cooldown cooldown;
        public CircularCooldownView cooldownView;
        public CostUpgradeData costUpgrade;
        public TraderUpgradeUI traderUI;   

        private void Awake()
        {
            m_Camera = Camera.main;
            cooldownView.Bind(cooldown);
            traderUI.Bind(this);
        }

        public void Update()
        {
            if (m_CurrentCustomer == null)
            {
                if (queue.TryPeek(out var peek))
                {
                    m_CurrentCustomer = peek;
                    cooldown.Restart();
                }
            }
            else if (cooldown.IsFinished)
            {
                var customerAI = m_CurrentCustomer.GetComponent<CustomerAI>();
                customerAI.buyedProducCost = costUpgrade.producCost;
                customerAI.holdedResource = resourceCost;
                m_CurrentCustomer = null;
                queue.Dequeue();
            }

            cooldownView.cooldownRoot.transform.forward = m_Camera.transform.forward;
            cooldown.Tick(Time.deltaTime);
            cooldownView.Tick();
        } 

        public void ActivateFromRaycast()
        {
            traderUI.Navigate();
        }
    }
}
