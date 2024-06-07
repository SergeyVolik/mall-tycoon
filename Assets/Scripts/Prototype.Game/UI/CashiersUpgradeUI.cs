using Prototype.UI;
using UnityEngine.UI;

namespace Prototype
{
    public class CashiersUpgradeUI : UIPage
    {
        public LevelUpUIItem[] buyCashiers;
        public LevelUpUIItem[] upgradeCashiers;
        public Button closeButton;

        public static CashiersUpgradeUI Instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Instance = this;

            closeButton.onClick.AddListener(() =>
            {
                RaycastInput.GetInstance().BlockRaycast = false;
            });
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void Show()
        {
            base.Show();
            RaycastInput.GetInstance().BlockRaycast = true;
        }

        public override void Hide(bool onlyDisableRaycast = false)
        {
            base.Hide(onlyDisableRaycast);
        }

        public void Bind(Market market)
        {
            for (int i = 0; i < market.Cashiers.Length; i++)
            {
                var cashier = market.Cashiers[i];
                var buyUI = buyCashiers[i];
                var upgradeCashierUI = upgradeCashiers[i];

                cashier.buyUpgrade.onChanged += () =>
                {
                    UpgradeBuyUI(cashier, buyUI, upgradeCashierUI);
                };

                UpgradeBuyUI(cashier, buyUI, upgradeCashierUI);

                buyUI.buyButton.GetComponent<HoldedButton>().onClick += () =>
                {
                    if (cashier.buyUpgrade.IsMaxLevel())
                        return;

                    cashier.buyUpgrade.LevelUp();
                };

                buyUI.cost.text = TextUtils.ValueToShortString(cashier.buyUpgrade.GetCostValue());

                cashier.workerSpeedUpgrade.onChanged += () =>
                {
                    UpgateCashierUpgradeUI(cashier, upgradeCashierUI);
                };

                UpgateCashierUpgradeUI(cashier, upgradeCashierUI);

                upgradeCashierUI.buyButton.GetComponent<HoldedButton>().onClick += () =>
                {
                    cashier.workerSpeedUpgrade.LevelUp();
                };
            }
        }

        void UpgateCashierUpgradeUI(CashierBehaviour cashier, LevelUpUIItem ui)
        {
            ui.UpgradeItem(cashier.workerSpeedUpgrade);          
            ui.description.text = $"customers: {(60f / cashier.workerSpeedUpgrade.GetValue()).ToString("0.0")} p/m";
        }

        void UpgradeBuyUI(CashierBehaviour cashier, LevelUpUIItem buyUI, LevelUpUIItem upgradeCashier)
        {
            bool isMAx = cashier.buyUpgrade.IsMaxLevel();
            buyUI.gameObject.SetActive(!isMAx);
            upgradeCashier.gameObject.SetActive(isMAx);
        }
    }
}
