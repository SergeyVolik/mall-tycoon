using UnityEngine;
using UnityEngine.Events;

namespace Prototype
{
    public class CashRegister : MonoBehaviour, IActivateableFromRaycast
    {
        public int producCost = 10;
        public QueueBehaviour queue;

        public WorkerSpeedUpgrade workerSpeedUpgrade;
        public TraderAI traderAi;

        public FloatingText floatingText;
        public UnityEvent onBuyUE;

        private void Awake()
        {
            workerSpeedUpgrade.onUpgraded += UpdateCooldownSpeed;
            UpdateCooldownSpeed();
        }

        void UpdateCooldownSpeed()
        {
            traderAi.cooldown.Duration = workerSpeedUpgrade.workerTime;
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

                floatingText.Show(customerAI.buyedProducCost.ToString("0"));
                customerAI.buyedProducCost = 0;
                customerAI.holdedResource = null;
                traderAi.Clear();
                onBuyUE.Invoke();
            }
        }

        public void ActivateFromRaycast()
        { }
    }
}
