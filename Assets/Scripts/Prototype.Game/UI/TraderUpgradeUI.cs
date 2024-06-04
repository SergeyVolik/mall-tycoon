using Prototype.UI;
using TMPro;
using UnityEngine.UI;

namespace Prototype
{
    public class TraderUpgradeUI : UIPage
    {
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI infoTitle;
        public Slider levelupProgress;
        public TextMeshProUGUI currentLevel;
        public TextMeshProUGUI maxLevel;
        public TextMeshProUGUI costText;
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI levelupMult;

        public LevelUpUIItem costLevelUp;
        public LevelUpUIItem workerLevelUp;
        private CostUpgradeData m_CostUpgrade;
        private WorkerSpeedUpgrade m_WorkerUpgrade;
        private TradingSpot m_Tarder;
        private PlayerData m_Playerdata;

        protected override void Awake()
        {
            base.Awake();

            costLevelUp.buyButton.onClick.AddListener(() =>
            {
                PlayerData.GetInstance().DecreaseMoney(m_CostUpgrade.currentBuyCost);
                m_CostUpgrade.LevelUp();
            });

            workerLevelUp.buyButton.onClick.AddListener(() =>
            {
                PlayerData.GetInstance().DecreaseMoney(m_WorkerUpgrade.currentBuyCost);
                m_WorkerUpgrade.LevelUp();
            });

            m_Playerdata = PlayerData.GetInstance();
            m_Playerdata.onMoneyChanged += TraderUpgradeUI_onMoneyChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(m_Playerdata)
                m_Playerdata.onMoneyChanged -= TraderUpgradeUI_onMoneyChanged;
        }
        public override void Show()
        {
            base.Show();
            RaycastInput.GetInstance().blockRaycast = true;
        }

        public override void Hide(bool onlyDisableRaycast = false)
        {
            base.Hide(onlyDisableRaycast);
            RaycastInput.GetInstance().blockRaycast = false;
        }

        private void TraderUpgradeUI_onMoneyChanged(float obj)
        {
            UpdateUI();
        }

        public void Bind(TradingSpot tarder)
        {
            m_CostUpgrade = tarder.costUpgrade;
            m_WorkerUpgrade = tarder.workerSpeedUpgrade;
            m_Tarder = tarder;
            UpdateUI();

            m_CostUpgrade.onUpgraded += M_Tarder_onUpgraded;
        }

        private void M_Tarder_onUpgraded()
        {
            UpdateUI();
        }

        void UpdateUI()
        {
            titleText.text = m_Tarder.traderName;
          
            int prevMax = m_CostUpgrade.GetPrevMax();
            int nextMax = m_CostUpgrade.GetNextMax();

            levelupProgress.minValue = prevMax;
            levelupProgress.maxValue = nextMax;
            int currentCostLevel = m_CostUpgrade.currentLevel;
            levelupProgress.value = currentCostLevel;
            infoTitle.text = m_CostUpgrade.GetUpgradeName();
            currentLevel.text = nextMax == currentCostLevel ? "MAX" : currentCostLevel.ToString();
            maxLevel.text = nextMax == currentCostLevel ? "MAX" : nextMax.ToString();
            levelupMult.text = m_CostUpgrade.GetNextUpgradeMult();
          
            costText.text = m_CostUpgrade.GetProducCost().ToString("0.0");
            timeText.text = m_WorkerUpgrade.workerTime.ToString("0.0");

            costLevelUp.cost.text = m_CostUpgrade.currentBuyCost.ToString("0.0");
            costLevelUp.buyButton.interactable = PlayerData.GetInstance().GetMoney() >= m_CostUpgrade.currentBuyCost && !m_CostUpgrade.IsFinished();

            workerLevelUp.cost.text = m_WorkerUpgrade.currentBuyCost.ToString("0.0");
            workerLevelUp.buyButton.interactable = PlayerData.GetInstance().GetMoney() >= m_WorkerUpgrade.currentBuyCost && !m_WorkerUpgrade.IsFinished();
        }
    }
}
