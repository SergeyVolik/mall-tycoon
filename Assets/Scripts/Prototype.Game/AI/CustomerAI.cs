using System;
using UnityEngine;
using UnityEngine.AI;

namespace Prototype
{
    public class CustomerAI : MonoBehaviour
    {
        public float buyedProducCost;
        public ResourceTypeSO holdedResource;
        public CustomerAIStates currentState = CustomerAIStates.SelectMarketPosition;
        private Vector3 m_StartPos;
        private Market m_Market;
        private NavMeshAgent m_Agent;
        private Transform m_Transform;
        private const float tickRate = 0.4f;
        private float tickT;
        private TradingSpot m_SelectedTraider;
        private CashierBehaviour m_SelectedCashier = null;
        private NavAgentAnimationController m_AnimatorController;
        private NavAgentAnimationController AnimatorController
        {
            get
            {
                if (m_AnimatorController == null)
                {
                    m_AnimatorController = GetComponentInChildren<NavAgentAnimationController>();
                }
                return m_AnimatorController;
            }
        }

        private void Awake()
        {
            m_StartPos = transform.position;
            m_Market = Market.GetInstance();
            m_Agent = GetComponent<NavMeshAgent>();
            m_Transform = transform;
        }

        public bool IsMoveming => m_Agent.remainingDistance > 0.1f;
        public enum CustomerAIStates
        {
            Idle,
            SelectMarketPosition,
            MoveToMarket,
            MoveToTraderQueue,
            WaitInTraderQueue,
            WaitTraderWork,
            WaitCashRegister,
            MoveToCashRegisterQueue,
            IdleInCashRegisterQueue,
            MoveToHome
        }

        public void MoveToTrader(Vector3 traderPosition)
        {
            currentState = CustomerAIStates.WaitTraderWork;
            m_Agent.destination = traderPosition;
        }
        public void MoveToCashRegister(Vector3 traderPosition)
        {
            currentState = CustomerAIStates.WaitCashRegister;
            m_Agent.destination = traderPosition;
        }

        private void GoHome()
        {
            currentState = CustomerAIStates.MoveToHome;
            m_Agent.destination = m_StartPos;
        }

        private void Update()
        {
            tickT += Time.deltaTime;

            if (tickRate > tickT)
                return;

            tickT = 0; 

            switch (currentState)
            {
                case CustomerAIStates.SelectMarketPosition:
                    currentState = CustomerAIStates.MoveToMarket;
                    m_Agent.destination = Market.GetInstance().GetRadnomInMarketPosition();

                    break;

                case CustomerAIStates.MoveToMarket:
                    if (IsDestinationReached())
                    {
                        currentState = CustomerAIStates.MoveToTraderQueue;
                        m_SelectedTraider = m_Market.GetRandomTraider();
                    }
                    break;

                case CustomerAIStates.MoveToTraderQueue:

                    if (!m_SelectedTraider.queue.HasFreePlace())
                    {
                        GoHome();
                        return;
                    }

                    m_Agent.destination = m_SelectedTraider.queue.GetNextPosition();

                    if (IsDestinationReached())
                    {
                        currentState = CustomerAIStates.WaitInTraderQueue;
                        m_SelectedTraider.queue.TakeQueue(this);
                    }
                    break;
                case CustomerAIStates.WaitInTraderQueue:

                    m_Agent.destination = m_SelectedTraider.queue.GetPositionInQueue(this);
                    break;
                case CustomerAIStates.WaitTraderWork:
                    if (buyedProducCost != 0)
                    {
                        currentState = CustomerAIStates.MoveToCashRegisterQueue;
                        m_SelectedCashier = Market.GetInstance().GetOptimalCashRegister();

                        if (m_SelectedCashier == null)
                        {
                            Debug.LogWarning("cashier not found");
                            GoHome();
                            return;
                        }
                        m_Agent.destination = m_SelectedCashier.queue.GetNextPosition();
                    }
                    break;
                case CustomerAIStates.WaitCashRegister:
                    if (buyedProducCost == 0)
                    {
                        GoHome();
                    }
                    break;
                case CustomerAIStates.MoveToCashRegisterQueue:

                    if (!m_SelectedCashier.queue.HasFreePlace())
                    {
                        GoHome();
                        return;
                    }

                    m_Agent.destination = m_SelectedCashier.queue.GetNextPosition();

                    if (IsDestinationReached())
                    {
                        currentState = CustomerAIStates.IdleInCashRegisterQueue;
                        m_SelectedCashier.queue.TakeQueue(this);
                    }
                    break;
                case CustomerAIStates.IdleInCashRegisterQueue:

                    m_Agent.destination = m_SelectedCashier.queue.GetPositionInQueue(this);
                    break;
                case CustomerAIStates.MoveToHome:
                    if (IsDestinationReached())
                    {
                        GameObject.Destroy(gameObject);
                    }
                    break;
                default:
                    break;
            }
        }

        private bool IsDestinationReached()
        {
            return Vector3.Distance(m_Transform.position, m_Agent.destination) < 0.5f;
        }

        internal void SetMoveSpeed(float speed)
        {
            m_Agent.speed = speed;
            AnimatorController.SetMoveSpeed(speed);
        }
    }
}
