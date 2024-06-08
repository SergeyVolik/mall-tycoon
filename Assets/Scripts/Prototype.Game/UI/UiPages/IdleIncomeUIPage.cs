using Prototype.UI;
using System;
using System.Diagnostics;
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

            var incomePerCustomer = obj.marketCustomerIncome;
            var spawnPerMinute = CustomerSpawnSystem.GetInstance().SpawnsPerMinute();
            var diff = DateTime.Now - obj.exitTime;
            UnityEngine.Debug.Log(diff.TotalMinutes);
            UnityEngine.Debug.Log(spawnPerMinute);
            UnityEngine.Debug.Log(incomePerCustomer);

            idleIncome = (float)diff.TotalMinutes * spawnPerMinute * incomePerCustomer;
            PlayerData.GetInstance().IncreaseMoney(idleIncome);
            incomeText.text = TextUtils.ValueToShortString(idleIncome);
        }
    }
}
