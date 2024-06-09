using System;
using UnityEngine;
using UnityEngine.Events;

namespace Prototype
{
    [System.Serializable]
    public class CashierBehaviourSave : ISaveComponentData
    {
        public UpgradeData workerSpeedUpgrade;
        public UpgradeData buyUpgrade;

        public SerializableGuid SaveId { get; set; }
    }

    public interface ICashier
    {
        public QueueBehaviour CustomerQueue { get; }
    }

    public class CashierBehaviour : MonoBehaviour, IActivateableFromRaycast, ISceneSaveComponent<CashierBehaviourSave>, ICashier
    {
        [field: SerializeField]
        public SerializableGuid SaveId { get; set; }

        public QueueBehaviour CustomerQueue => queue;

        public QueueBehaviour queue;
        public UpgradeData workerSpeedUpgrade;
        private bool m_Loaded;
        public UpgradeData buyUpgrade;

        public TraderAI traderAi;
        public UnityEvent onBuyUE;
        public BuyFeedback buyFeedback;

        private void Awake()
        {
            if (m_Loaded)
                return;

            Setup();
        }

        private void Setup()
        {
            workerSpeedUpgrade.onChanged += UpdateCooldownSpeed;
            buyUpgrade.onChanged += BuyUpgrade_onChanged;

            UpdateCooldownSpeed();
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

        public CashierBehaviourSave SaveComponent()
        {
            return new CashierBehaviourSave
            {
                buyUpgrade = buyUpgrade,
                workerSpeedUpgrade = workerSpeedUpgrade
            };
        }

        public void LoadComponent(CashierBehaviourSave data)
        {
            buyUpgrade = data.buyUpgrade;
            workerSpeedUpgrade = data.workerSpeedUpgrade;
            m_Loaded = true;

            Setup();
        }
    }
}
