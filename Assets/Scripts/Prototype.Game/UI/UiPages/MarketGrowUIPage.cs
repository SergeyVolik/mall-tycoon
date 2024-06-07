using Prototype.UI;
using UnityEngine;

namespace Prototype
{
    public class MarketGrowUIPage : UIPage
    {
        public static MarketGrowUIPage Instance { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        private bool m_Binded;
        public LevelUpUIItem[] buyVegetablesSpot;
        public LevelUpUIItem buyTradingSpotUIItemPrefab;
        public Transform spawnParent;

        public override void Show()
        {
            base.Show();
            Bind(Market.GetInstance());
        }
        public void Bind(Market market)
        { 
            if (m_Binded)
                return;

            m_Binded = true;
            buyVegetablesSpot = new LevelUpUIItem[market.TradingSpots.Length];

            for (int i = 0; i < market.TradingSpots.Length; i++)
            {
                var traider = market.TradingSpots[i];
                var traiderUI = GameObject.Instantiate(buyTradingSpotUIItemPrefab, spawnParent);
                buyVegetablesSpot[i] = traiderUI;

                traiderUI.buyButton.onClick.AddListener(() =>
                {
                    traider.buySpotUpgrade.LevelUp();
                });

                traider.buySpotUpgrade.onChanged += () => { BuySpotUpgrade_onChanged(traiderUI, traider); };

                traiderUI.title.text = traider.traderName;
                traiderUI.UpgradeItem(traider.buySpotUpgrade);
            }
        }

        private void BuySpotUpgrade_onChanged(LevelUpUIItem traiderUI, TradingSpot traider)
        {
            traiderUI.UpgradeItem(traider.buySpotUpgrade);
        }
    }
}
