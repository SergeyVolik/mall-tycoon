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
        private UpgradeData m_WorkerUpgrade;
        private TradingSpot m_Tarder;
        private PlayerData m_Playerdata;

        public static TraderUpgradeUI Instance { get; private set; }

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
                PlayerData.GetInstance().DecreaseMoney(m_WorkerUpgrade.GetCostValue());
                m_WorkerUpgrade.LevelUp();
            });

            m_Playerdata = PlayerData.GetInstance();
            m_Playerdata.onMoneyChanged += TraderUpgradeUI_onMoneyChanged;
            Instance = this;
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
            RaycastInput.GetInstance().BlockRaycast = true;
        }

        public override void Hide(bool onlyDisableRaycast = false)
        {
            base.Hide(onlyDisableRaycast);
            RaycastInput.GetInstance().BlockRaycast = false;
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
            costText.text = m_CostUpgrade.GetProducCost().ToString("0.0");
            infoTitle.text = m_CostUpgrade.GetUpgradeName();
            currentLevel.text = nextMax == currentCostLevel ? "MAX" : currentCostLevel.ToString();
            maxLevel.text = nextMax == currentCostLevel ? "MAX" : nextMax.ToString();
            levelupMult.text = m_CostUpgrade.GetNextUpgradeMult();
          
         
            timeText.text = m_WorkerUpgrade.GetValue().ToString("0.0");
            costLevelUp.cost.text = TextUtils.ValueToShortString(m_CostUpgrade.currentBuyCost);
            costLevelUp.buyButton.interactable = PlayerData.GetInstance().GetMoney() >= m_CostUpgrade.currentBuyCost && !m_CostUpgrade.IsFinished();

            workerLevelUp.cost.text = TextUtils.ValueToShortString(m_WorkerUpgrade.GetCostValue());
            workerLevelUp.buyButton.interactable = PlayerData.GetInstance().GetMoney() >= m_WorkerUpgrade.GetCostValue() && !m_WorkerUpgrade.IsMaxLevel();
        }
    }
}
