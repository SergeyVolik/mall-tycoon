using Prototype.UI;
using System;
using UnityEngine.UI;

namespace Prototype
{
    public class UpgradeUiData
    {
        public LevelUpUIItem buyUI;
        public LevelUpUIItem workerSpeeedUI;
        public UpgradeData workerSpeedUpgrade;
        public UpgradeData buyUpgrade;
        public Action buyAction;
        internal Action upgateBuyUI;
        internal Action updateUpgradeUI;
        internal Action upgradeAction;
    }

    public class CashiersUpgradeUI : UIPage
    {
        public LevelUpUIItem[] buyCashiers;
        public LevelUpUIItem[] upgradeCashiers;
        public LevelUpUIItem selfServiceUpgrade;
        public Button closeButton;

        public UpgradeUiData[] instancesUI;
        public static CashiersUpgradeUI Instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Instance = this;

            closeButton.onClick.AddListener(() =>
            {

            });
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void Show()
        {
            base.Show();
            Bind(Market.GetInstance());
        }

        public override void Hide(bool onlyDisableRaycast = false)
        {
            base.Hide(onlyDisableRaycast);
        }

        public void Bind(Market market)
        {
            Unbind();

            instancesUI = new UpgradeUiData[market.Cashiers.Length];

            PlayerData.GetInstance().onMoneyChanged += CashiersUpgradeUI_onMoneyChanged;
  
            selfServiceUpgrade.buyButton.onClick.AddListener(SelfServiceCashierBuy);

            for (int i = 0; i < market.Cashiers.Length; i++)
            {
                var cashier = market.Cashiers[i];
                var buyUI = buyCashiers[i];
                var upgradeCashierUI = upgradeCashiers[i];

                Action upgateBuyUI = () =>
                {
                    UpgradeBuyUI(cashier.buyUpgrade, buyUI, upgradeCashierUI);
                };

                Action buyAction = () =>
                {
                    if (cashier.buyUpgrade.IsMaxLevel())
                        return;

                    cashier.buyUpgrade.LevelUp();
                };

                Action upgradeAction = () =>
                {
                    cashier.workerSpeedUpgrade.LevelUp();
                };

                Action updateUpgradeUI = () =>
                {
                    UpgateCashierUpgradeUI(cashier.workerSpeedUpgrade, upgradeCashierUI);
                };

                cashier.buyUpgrade.onChanged += upgateBuyUI;
                buyUI.buyButton.GetComponent<HoldedButton>().onClick += buyAction;
                cashier.workerSpeedUpgrade.onChanged += updateUpgradeUI;
                upgradeCashierUI.buyButton.GetComponent<HoldedButton>().onClick += upgradeAction;

                UpgradeUiData upgradeData = new UpgradeUiData
                {
                    buyUpgrade = cashier.buyUpgrade,
                    workerSpeedUpgrade = cashier.workerSpeedUpgrade,
                    buyUI = buyUI,
                    workerSpeeedUI = upgradeCashierUI,
                    buyAction = buyAction,
                    upgateBuyUI = upgateBuyUI,
                    upgradeAction = upgradeAction,
                    updateUpgradeUI = updateUpgradeUI,
                };

                instancesUI[i] = upgradeData;
            }

            UpdateUI();
        }

        void UpdateUI()
        {
            selfServiceUpgrade.UpgradeItem(Market.GetInstance().CashierSelfService.buyData);

            foreach (var item in instancesUI)
            {
                item.buyUI.cost.text = TextUtils.ValueToShortString(item.buyUpgrade.GetCostValue());
                UpgradeBuyUI(item.buyUpgrade, item.buyUI, item.workerSpeeedUI);
                UpgateCashierUpgradeUI(item.workerSpeedUpgrade, item.workerSpeeedUI);
                UpgateCashierUpgradeUI(item.workerSpeedUpgrade, item.workerSpeeedUI);

            }
        }

        private void SelfServiceCashierBuy()
        {
            Market.GetInstance().CashierSelfService.buyData.LevelUp();
            selfServiceUpgrade.UpgradeItem(Market.GetInstance().CashierSelfService.buyData);
        }

        private void Unbind()
        {
            selfServiceUpgrade.buyButton.onClick.RemoveListener(SelfServiceCashierBuy);

            if (instancesUI != null)
            {
                foreach (var item in instancesUI)
                {
                    item.buyUpgrade.onChanged -= item.upgateBuyUI;
                    item.buyUI.buyButton.GetComponent<HoldedButton>().onClick -= item.buyAction;
                    item.workerSpeedUpgrade.onChanged -= item.updateUpgradeUI;
                    item.workerSpeeedUI.buyButton.GetComponent<HoldedButton>().onClick -= item.upgradeAction;
                }
            }
            PlayerData.GetInstance().onMoneyChanged -= CashiersUpgradeUI_onMoneyChanged;
            instancesUI = null;
        }

        private void CashiersUpgradeUI_onMoneyChanged(float obj)
        {
            UpdateUI();
        }

        void UpgateCashierUpgradeUI(UpgradeData workerSpeedUpgrade, LevelUpUIItem ui)
        {
            ui.UpgradeItem(workerSpeedUpgrade);
            ui.description.text = $"customers: {(60f / workerSpeedUpgrade.GetValue()).ToString("0.0")} p/m";
        }

        void UpgradeBuyUI(UpgradeData buyUpgrade, LevelUpUIItem buyUI, LevelUpUIItem upgradeCashier)
        {
            bool isMAx = buyUpgrade.IsMaxLevel();
            buyUI.gameObject.SetActive(!isMAx);
            upgradeCashier.gameObject.SetActive(isMAx);
            buyUI.UpgradeItem(buyUpgrade);

        }
    }
}
