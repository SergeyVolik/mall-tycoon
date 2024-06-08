using Prototype.UI;
using System;
using UnityEngine;

namespace Prototype
{
    public class StatsUIPage : UIPage
    {
        public StatsItem customerIncome;
        public StatsItem maxCustomers;
        public StatsItem averageQueue;
        public StatsItem averageQueueTime;

        public override void Show()
        {
            base.Show();
            UpdateUI();
        }

        private void UpdateUI()
        {
            customerIncome.valueText.text = TextUtils.ValueToShortString(Market.GetInstance().GetTotalIncomePerCustomer());
            var inMarketAver = Market.GetInstance().GetCustomersPerMinute();

            maxCustomers.valueText.text = $"{inMarketAver} / min";
            averageQueue.valueText.text = Market.GetInstance().GetAverageCheckoutCustomersInQueue().ToString("0.0");
            averageQueueTime.valueText.text = $"{Market.GetInstance().GetAverageCheckoutQueueTimeInSeconds().ToString("0.0")} sec";
        }
    }
}
