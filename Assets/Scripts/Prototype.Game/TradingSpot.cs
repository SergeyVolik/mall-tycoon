using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Prototype
{
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
            PlayerData.GetInstance().DecreaseMoney(currentBuyCost);
            currentLevel++;
            var currentUpgrade = upgrades[currentUpgradeIndex];
            producCost += currentUpgrade.producCostIncrease;
            currentBuyCost += currentUpgrade.buyUpgradeIncreaseValue;
         
            if (currentLevel >= currentUpgrade.maxLevel)
            {
                currentUpgrade.ActivateVisual(true);
             
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
        [JsonIgnore]
        public GameObject[] itemsToActivate;
    
        public int maxLevel;       
        public float producCostIncrease;      
        public string name;      
        public float maxLevelMult;       
        public float buyUpgradeIncreaseValue;

        [JsonIgnore]
        public GameObject customerItemPrefab;
        public void ActivateVisual(bool activate)
        {
            foreach (var item in itemsToActivate)
            {
                item.gameObject.SetActive(activate);
            }
        }
    }

    [System.Serializable]
    public class TradingSpotSaveData : ISaveComponentData
    {
        public SerializableGuid SaveId { get; set; }
        public UpgradeData workerSpeedUpgrade;
        public UpgradeData addWorkerUpgrade;
        public UpgradeData buySpotUpgrade;
        public CostUpgradeData costUpgrade;
    }

    public class TradingSpot : MonoBehaviour, IActivateableFromRaycast, ISceneSaveComponent<TradingSpotSaveData>
    {
        [field: SerializeField]
        public SerializableGuid SaveId { get; set; }

        public string traderName;
        public ResourceTypeSO resourceCost;
        public QueueBehaviour queue;
        public CostUpgradeData costUpgrade;
        public UpgradeData workerSpeedUpgrade;
        private bool m_Loaded;
        public UpgradeData addWorkerUpgrade;
        public UpgradeData buySpotUpgrade;

        public TraderAI[] traders;
        public GameObject defaultCustomerItemPrefab;
        private void Awake()
        {
            if (m_Loaded)
                return;

            Setup();
        }

        private void Setup()
        {
            BindCallbacks();
            BuyStateUpdate();
            UpdateNumberOfWorkers();
            UpdateCooldownSpeed();
            UpdateVisual();
        }

        private void BindCallbacks()
        {
            workerSpeedUpgrade.onChanged += UpdateCooldownSpeed;
            addWorkerUpgrade.onChanged += UpdateNumberOfWorkers;
            buySpotUpgrade.onChanged += BuyStateUpdate;
        }

        private void BuyStateUpdate()
        {
            gameObject.SetActive(buySpotUpgrade.IsMaxLevel());
        }

        private void UpdateNumberOfWorkers()
        {
            foreach (var item in traders)
            {
                item.gameObject.SetActive(false);
            }

            for (int i = 0; i < addWorkerUpgrade.GetValue(); i++)
            {
                traders[i].gameObject.SetActive(true);
            }
        }

        private int GetActiveWorker()
        {
            int active = 0;
            foreach (var item in traders)
            {
                if (item.IsActive())
                    active++;
            }
            return active;
        }

        public float GetIncomePerMinute()
        {
            float customersPerMinute = 60f / workerSpeedUpgrade.GetValue() / GetActiveWorker();
            return costUpgrade.GetProducCost() * customersPerMinute;
        }

        private void UpdateVisual()
        {
            foreach (var item in costUpgrade.upgrades)
            {
                item.ActivateVisual(costUpgrade.currentLevel >= item.maxLevel);
            }
        }

        private void UpdateCooldownSpeed()
        {
            foreach (var item in traders)
            {
                item.cooldown.Duration = workerSpeedUpgrade.GetValue();
            }
        }

        public event Action onCheckoutFinished = delegate { };
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
                    customerAI.buyedProducCost = Market.GetInstance().GetTotalIncomePerCustomer();
                    customerAI.SpawnCustomerItem(GetRandomItemPrefab());
                    customerAI.holdedResource = resourceCost;
                    traderAi.Clear();
                    onCheckoutFinished.Invoke();
                }
            }
        }
        public GameObject GetRandomItemPrefab()
        {
            GameObject prefab = defaultCustomerItemPrefab;

            int rndIndex = UnityEngine.Random.Range(0, costUpgrade.upgrades.Length);
            int i = 0;
            foreach (var item in costUpgrade.upgrades)
            {
                if (item.maxLevel <= costUpgrade.currentLevel && item.customerItemPrefab != null)
                {
                    prefab = item.customerItemPrefab;
                }

                if (rndIndex == i)
                    break;

                i++;
            }

            return prefab;
        }

        public void ActivateFromRaycast()
        {
            TraderUpgradeUI.Instance.Bind(this);
            TraderUpgradeUI.Instance.Navigate();
        }

        internal bool IsWorking()
        {
            return gameObject.activeSelf;
        }

        public TradingSpotSaveData SaveComponent()
        {
            return new TradingSpotSaveData()
            {
                addWorkerUpgrade = addWorkerUpgrade,
                buySpotUpgrade = buySpotUpgrade,
                workerSpeedUpgrade = workerSpeedUpgrade,
                SaveId = SaveId,
                costUpgrade = costUpgrade,
            };
        }

        public void LoadComponent(TradingSpotSaveData data)
        {
            addWorkerUpgrade = data.addWorkerUpgrade;
            buySpotUpgrade = data.buySpotUpgrade;
            workerSpeedUpgrade = data.workerSpeedUpgrade;

            for (int i = 0; i < costUpgrade.upgrades.Length; i++)
            {
                data.costUpgrade.upgrades[i].itemsToActivate = costUpgrade.upgrades[i].itemsToActivate;
                data.costUpgrade.upgrades[i].customerItemPrefab = costUpgrade.upgrades[i].customerItemPrefab;
            }

            costUpgrade = data.costUpgrade;

            Setup();
            m_Loaded = true;
        }
    }
}
