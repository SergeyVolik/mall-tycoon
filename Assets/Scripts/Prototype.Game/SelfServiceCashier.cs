using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public class SelfServiceCashierSave : ISaveComponentData
    {
        public SerializableGuid SaveId { get; set; }
        public UpgradeData buyData;
    }

    public class SelfServiceCashier : MonoBehaviour, IActivateableFromRaycast, ICashier, ISceneSaveComponent<SelfServiceCashierSave>
    {
        public QueueBehaviour queue;
        public BuyFeedback buyFeedback;

        public QueueBehaviour CustomerQueue => queue;

        [field: SerializeField]
        public SerializableGuid SaveId { get; set; }

        public Transform cashierMovePoint;

        private CustomerAI m_CurrentCustomer;

        public UpgradeData buyData;

        private void Awake()
        {
            Setup();
        }

        public void ActivateFromRaycast()
        {
            if (m_CurrentCustomer == null)
                return;

            var customerAI = m_CurrentCustomer;
            customerAI.GoHome();
            buyFeedback.Play(customerAI.buyedProducCost.ToString("0"));

            PlayerData.GetInstance().Resources.resources.AddResource(customerAI.holdedResource, customerAI.buyedProducCost);

            customerAI.buyedProducCost = 0;
            customerAI.holdedResource = null;
            m_CurrentCustomer = null;
        }

        private void Update()
        {
            if (m_CurrentCustomer == null && queue.Count > 0)
            {
                m_CurrentCustomer = queue.Dequeue();
                m_CurrentCustomer.MoveToCashRegister(cashierMovePoint.position);
            }
        }

        private void Setup()
        {
            buyData.onChanged += BuyUpgrade_onChanged;
            BuyUpgrade_onChanged();
        }

        private void BuyUpgrade_onChanged()
        {
            gameObject.SetActive(buyData.IsMaxLevel());
        }

        public SelfServiceCashierSave SaveComponent()
        {
            return new SelfServiceCashierSave
            {
                SaveId = SaveId,
                buyData = buyData,
            };
        }

        public void LoadComponent(SelfServiceCashierSave data)
        {
            buyData = data.buyData;

            Setup();
        }
    }
}
