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
        private Trader m_Tarder;

        protected override void Awake()
        {
            base.Awake();

            costLevelUp.buyButton.onClick.AddListener(() =>
            {
                m_CostUpgrade.LevelUpCost();
            });

            workerLevelUp.buyButton.onClick.AddListener(() =>
            {

            });

            PlayerData.GetInstance().onMoneyChanged += TraderUpgradeUI_onMoneyChanged;
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            PlayerData.GetInstance().onMoneyChanged -= TraderUpgradeUI_onMoneyChanged;
        }
        private void TraderUpgradeUI_onMoneyChanged(float obj)
        {
            UpdateUI();
        }

        public void Bind(Trader tarder)
        {
            m_CostUpgrade = tarder.costUpgrade;
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
            timeText.text = m_Tarder.cooldown.Duration.ToString("0.0");
            costLevelUp.cost.text = m_CostUpgrade.currentBuyCost.ToString("0.0");
            costLevelUp.buyButton.interactable = PlayerData.GetInstance().GetMoney() >= m_CostUpgrade.currentBuyCost;
        }
    }
}
