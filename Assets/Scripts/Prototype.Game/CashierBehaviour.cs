using System;
using UnityEngine;
using UnityEngine.Events;

namespace Prototype
{
    public class CashierBehaviour : MonoBehaviour, IActivateableFromRaycast
    {
        public QueueBehaviour queue;
        public UpgradeData workerSpeedUpgrade;
        public UpgradeData buyUpgrade;

        public TraderAI traderAi;
        public UnityEvent onBuyUE;
        public BuyFeedback buyFeedback;

        private void Awake()
        {
            workerSpeedUpgrade.onChanged += UpdateCooldownSpeed;
            UpdateCooldownSpeed();
            buyUpgrade.onChanged += BuyUpgrade_onChanged;
            BuyUpgrade_onChanged();
        }

        private void BuyUpgrade_onChanged()
        {
            gameObject.SetActive(buyUpgrade.IsMaxLevel());
        }

        void UpdateCooldownSpeed()
        {
            traderAi.cooldown.Duration = workerSpeedUpgrade.GetValue();
        }

        public void Update()
        {
            traderAi.Tick();

            if (traderAi.IsWorkFinished() && !traderAi.IsHasCustomer() && queue.Count != 0)
            {
                var customer = queue.Dequeue();
                traderAi.StartWorking(customer);
                customer.MoveToCashRegister(traderAi.customerMovePoint.position);
            }
            else if (traderAi.IsWorkFinished() && traderAi.IsHasCustomer())
            {
                var customerAI = traderAi.CurrentCustomer;
                PlayerData.GetInstance().Resources.resources.AddResource(customerAI.holdedResource, customerAI.buyedProducCost);

                buyFeedback.Play(customerAI.buyedProducCost.ToString("0"));
                customerAI.buyedProducCost = 0;
                customerAI.holdedResource = null;
                traderAi.Clear();
                onBuyUE.Invoke();
            }
        }

        public void ActivateFromRaycast()
        {
            CashiersUpgradeUI.Instance.Navigate();
        }

        internal bool IsWorking()
        {
            return gameObject.activeSelf;
        }
    }
}
