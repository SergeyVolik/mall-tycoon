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

        private const int minsToApplyIdleIncome = 1;
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

            var diff = DateTime.Now - obj.exitTime;

            if (diff.TotalMinutes < minsToApplyIdleIncome)
            {
                return;
            }

            if (!IsShowed)
                Navigate();

            var incomePerCustomer = obj.marketCustomerIncome;
            var spawnPerMinute = CustomerSpawnSystem.GetInstance().SpawnsPerMinute();
       
            UnityEngine.Debug.Log(diff.TotalMinutes);
            UnityEngine.Debug.Log(spawnPerMinute);
            UnityEngine.Debug.Log(incomePerCustomer);

            idleIncome = (float)diff.TotalMinutes * spawnPerMinute * incomePerCustomer;
            PlayerData.GetInstance().IncreaseMoney(idleIncome);
            incomeText.text = TextUtils.ValueToShortString(idleIncome);
        }
    }
}
