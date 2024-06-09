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

        public Transform customerItemStartPoint;
        public Transform customerItemEndPoint;

        public AnimationCurve moveCustomerItemFromHands;
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

        private Transform m_CustomerItem;
        private Transform m_CustomerHandPoint;
        private CustomerAI m_Customer;

        private Vector3 m_StartMovePoint;
        private Quaternion m_StartRotation;
        private float time;

        private CustomerItemMoveState m_AnimState = CustomerItemMoveState.Finished;
        public enum CustomerItemMoveState
        {
            Finished,
            FromCustomerToCashier,
            CahsierMovement,
            FromCashierToCustomer,
        }

        void UpdateCustomerItemMoveAnimation()
        {
            switch (m_AnimState)
            {
                case CustomerItemMoveState.Finished:
                    break;
                case CustomerItemMoveState.FromCustomerToCashier:

                    FromCustomerToCashier();
                    break;
                case CustomerItemMoveState.CahsierMovement:
                    CahsierMovement();
                    break;
                case CustomerItemMoveState.FromCashierToCustomer:
                    FromCashierToCustomer();
                    break;
                default:
                    break;
            }
        }

         
        private void FromCashierToCustomer()
        {
            float duration3 = workerSpeedUpgrade.GetValue() *0.1f;

            time += Time.deltaTime;
            var t2 = time / duration3;

            var pos2 = Vector3.Lerp(customerItemEndPoint.position, m_CustomerHandPoint.position, t2);
            pos2.y += moveCustomerItemFromHands.Evaluate(t2);

            m_CustomerItem.position = pos2;
            m_CustomerItem.rotation = Quaternion.Lerp(customerItemEndPoint.rotation, m_CustomerHandPoint.rotation, t2);

            if (time > duration3)
            {
                m_Customer.BindCustomerItem(m_CustomerItem);
                time = 0;
                m_AnimState = CustomerItemMoveState.Finished;
            }
        }

        private void CahsierMovement()
        {
            float duration2 = workerSpeedUpgrade.GetValue() * 0.8f;

            time += Time.deltaTime;
            var t1 = time / duration2;
            var pos1 = Vector3.Lerp(customerItemStartPoint.position, customerItemEndPoint.position, t1);

            m_CustomerItem.position = pos1;

            if (time > duration2)
            {
                time = 0;
                m_AnimState = CustomerItemMoveState.FromCashierToCustomer;
            }
        }

        private void FromCustomerToCashier()
        {
            float duration = workerSpeedUpgrade.GetValue() * 0.1f;

            time += Time.deltaTime;
            var t = time / duration;
            
            var pos = Vector3.Lerp(m_StartMovePoint, customerItemStartPoint.position, t);
            pos.y += moveCustomerItemFromHands.Evaluate(t);

            m_CustomerItem.position = pos;
            m_CustomerItem.rotation = Quaternion.Lerp(m_StartRotation, customerItemStartPoint.rotation, t);

            if (time > duration)
            {
                time = 0;
                m_AnimState = CustomerItemMoveState.CahsierMovement;
            }
        }

        public void Update()
        {
            traderAi.Tick();

            if (traderAi.IsWorkFinished() && !traderAi.IsHasCustomer() && queue.Count != 0)
            {
                var customer = queue.Dequeue();
                traderAi.StartWorking(customer);
                customer.MoveToCashRegister(traderAi.customerMovePoint.position);
                StartAnimation(customer);
            }
            else if (traderAi.IsWorkFinished() && traderAi.IsHasCustomer() && m_AnimState == CustomerItemMoveState.Finished)
            {
                var customerAI = traderAi.CurrentCustomer;
                PlayerData.GetInstance().Resources.resources.AddResource(customerAI.holdedResource, customerAI.buyedProducCost);

                buyFeedback.Play(customerAI.buyedProducCost.ToString("0"));
                customerAI.buyedProducCost = 0;
                customerAI.holdedResource = null;
                traderAi.Clear();
                onBuyUE.Invoke();
            }

            UpdateCustomerItemMoveAnimation();
        }

        private void StartAnimation(CustomerAI customer)
        {
            m_CustomerHandPoint = customer.GetHandPoint();
            m_CustomerItem = customer.UnbindCustomerItem();
            m_CustomerItem.parent = null;
            m_AnimState = CustomerItemMoveState.FromCustomerToCashier;
            m_StartMovePoint = m_CustomerItem.transform.position;
            m_StartRotation = m_CustomerItem.transform.rotation;
            m_Customer = customer;
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
