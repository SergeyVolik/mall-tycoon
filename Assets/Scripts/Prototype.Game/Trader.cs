using System;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    [System.Serializable]
    public class CostUpgradeData
    {
        public GameObject[] itemsToActivate;
        public int levelsToFinish;
        public float levelUpCostIncrease;
        public string title;
        public int finishCostMult;
    }

    public class Trader : MonoBehaviour, IActivateableFromRaycast
    {
        public float producCost = 10;
        public int currentLevel = 1;
        public string traderName;
        public ResourceTypeSO resourceCost;
        public QueueBehaviour queue;
        private Transform m_CurrentCustomer;
        private Camera m_Camera;
        public Cooldown cooldown;
        public CircularCooldownView cooldownView;
        public CostUpgradeData[] upgrades;
        public int currentUpgradeIndex;
        public TraderUpgradeUI traderUI;
        public event Action onUpgraded = delegate { }; 

        public void LevelUpCost()
        {
            currentLevel++;
            var currentUpgrade = upgrades[currentUpgradeIndex];
            producCost += currentUpgrade.levelUpCostIncrease;

            if (currentLevel > currentUpgrade.levelsToFinish && upgrades.Length -1 > currentUpgradeIndex)
            {
                foreach (var item in currentUpgrade.itemsToActivate)
                {
                    item.gameObject.SetActive(true);
                }
                producCost *= currentUpgrade.finishCostMult;
                currentUpgradeIndex++;               
            }

            onUpgraded.Invoke();
        }

        public bool IsCostUpgradesFinished() => upgrades.Length - 1 == currentUpgradeIndex && upgrades[currentUpgradeIndex].levelsToFinish == currentLevel;
        public int GetMaxLevel()
        {
            return upgrades[currentUpgradeIndex].levelsToFinish;
        }

        public float GetProducCost()
        {
            return producCost;
        }

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
                customerAI.buyedProducCost = producCost;
                customerAI.holdedResource = resourceCost;
                m_CurrentCustomer = null;
                queue.Dequeue();
            }

            cooldownView.cooldownRoot.transform.forward = m_Camera.transform.forward;
            cooldown.Tick(Time.deltaTime);
            cooldownView.Tick();
        }

        internal int GetPrevMax()
        {
            if(currentUpgradeIndex == 0)
                return 1;

            return upgrades[currentUpgradeIndex - 1].levelsToFinish;
        }

        internal int GetNextMax()
        {
            return upgrades[currentUpgradeIndex].levelsToFinish;
        }

        internal string GetNextUpgradeMult()
        {
            return upgrades[currentUpgradeIndex].finishCostMult.ToString();
        }

        public void ActivateFromRaycast()
        {
            traderUI.Navigate();
        }
    }
}
