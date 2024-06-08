using Prototype.UI;
using System;
using TMPro;
using UnityEngine.UI;

namespace Prototype
{
    public class IdleIncomeUIPage : UIPage
    {
        protected override void Awake()
        {
            base.Awake();    
        }

        public TextMeshProUGUI incomeText;
        public Button getIncomeButton;
        private float idleIncome;

        protected override void Start()
        {
            base.Start();

            getIncomeButton.onClick.AddListener(() =>
            {
                PlayerData.GetInstance().IncreaseMoney(idleIncome);
                UINavigationManager.GetInstance().Pop();
            });

            GlobalDataSaveManager.Instance.OnLoaded += Instance_OnLoaded;
            Instance_OnLoaded(GlobalDataSaveManager.Instance.LastLoad);
        }

        private void Instance_OnLoaded(GlobalSave obj)
        {
            if (obj == null)
                return;

            Navigate();

            GlobalDataSaveManager.Instance.OnLoaded -= Instance_OnLoaded;

            var incomePerCustomer = Market.GetInstance().GetTotalIncomePerCustomer();
            var spawnPerMinute = CustomerSpawnSystem.GetInstance().SpawnsPerMinute();
            var diff = DateTime.Now - obj.exitTime;
           
            idleIncome = (float)diff.TotalMinutes * spawnPerMinute * incomePerCustomer;
            incomeText.text = TextUtils.ValueToShortString(idleIncome);
        }
    }
}