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
        private Trader m_Tarder;

        protected override void Awake()
        {
            base.Awake();

            costLevelUp.buyButton.onClick.AddListener(() =>
            {
                m_Tarder.LevelUpCost();
            });

            workerLevelUp.buyButton.onClick.AddListener(() =>
            {

            });
        }

        public void Bind(Trader tarder)
        {
            m_Tarder = tarder;
            UpdateUI();

            m_Tarder.onUpgraded += M_Tarder_onUpgraded;
        }

        private void M_Tarder_onUpgraded()
        {
            UpdateUI();
        }

        void UpdateUI()
        {
            titleText.text = m_Tarder.traderName;
          
            int prevMax = m_Tarder.GetPrevMax();
            int nextMax = m_Tarder.GetNextMax();

            levelupProgress.minValue = prevMax;
            levelupProgress.minValue = nextMax;
            int currentCostLevel = m_Tarder.currentLevel;
            levelupProgress.value = currentCostLevel;

            currentLevel.text = nextMax == currentCostLevel ? "MAX" : currentCostLevel.ToString();
            maxLevel.text = nextMax == currentCostLevel ? "MAX" : nextMax.ToString();
            levelupMult.text = m_Tarder.GetNextUpgradeMult();
          
            costText.text = m_Tarder.GetProducCost().ToString("0.0");
            timeText.text = m_Tarder.cooldown.Duration.ToString("0.0");
        }
    }
}
