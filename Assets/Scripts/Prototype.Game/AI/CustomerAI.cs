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
        private const float tickRate = 0.2f;
        private float tickT;

        private void Awake()
        {
            m_StartPos = transform.position;
            m_Market = Market.GetInstance();
            m_Agent = GetComponent<NavMeshAgent>();
            m_Transform = transform;
        }
        public enum CustomerAIStates
        {
            SelectMarketPosition,
            MoveToMarket,
            MoveToTrader,
            IdleInTraderQueue,
            WaitTraderWork,
            MoveToCashRegister,
            IdleInCashRegisterQueue,
            MoveToHome
        }

        public void MoveToTrader(Vector3 traderPosition)
        {
            currentState = CustomerAIStates.WaitTraderWork;
            m_Agent.destination = traderPosition;
        }

        private void Update()
        {
            tickT += Time.deltaTime;

            if (tickRate > tickT)
                return;

            tickT = 0;
            
            var trader = m_Market.Trader;
            var cashRegister = m_Market.CashRegister;

            switch (currentState)
            {
                case CustomerAIStates.SelectMarketPosition:
                    currentState = CustomerAIStates.MoveToMarket;
                    m_Agent.destination = Market.GetInstance().GetRadnomInMarketPosition();

                    break;

                case CustomerAIStates.MoveToMarket:                  
                    if (IsDestinationReached())
                    {
                        currentState = CustomerAIStates.MoveToTrader;
                    }
                    break;

                case CustomerAIStates.MoveToTrader:
                    m_Agent.destination = trader.queue.GetNextPosition();

                    if (IsDestinationReached())
                    {
                        currentState = CustomerAIStates.IdleInTraderQueue;
                        trader.queue.TakeQueue(this);
                    }
                    break;
                case CustomerAIStates.IdleInTraderQueue:

                    m_Agent.destination = trader.queue.GetPositionInQueue(this);
                    break;
                case CustomerAIStates.WaitTraderWork:
                    if (buyedProducCost != 0)
                    {
                        currentState = CustomerAIStates.MoveToCashRegister;
                        m_Agent.destination = cashRegister.queue.GetNextPosition();
                    }
                    break;
                case CustomerAIStates.MoveToCashRegister:
                    m_Agent.destination = cashRegister.queue.GetNextPosition();

                    if (IsDestinationReached())
                    {
                        currentState = CustomerAIStates.IdleInCashRegisterQueue;
                        cashRegister.queue.TakeQueue(this);
                    }
                    break;
                case CustomerAIStates.IdleInCashRegisterQueue:

                    if (!cashRegister.queue.IsInQueue(this))
                    {
                        currentState = CustomerAIStates.MoveToHome;
                        m_Agent.destination = m_StartPos;
                    }
                    else
                    {
                        m_Agent.destination = cashRegister.queue.GetPositionInQueue(this);
                    }
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
    }
}
