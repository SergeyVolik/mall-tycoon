using UnityEngine;
using UnityEngine.AI;

namespace Prototype
{
    public class CustomerAI : MonoBehaviour
    {
        public int buyedProducCost;
        public ResourceTypeSO holdedResource;
        public CustomerAIStates currentState = CustomerAIStates.MoveToTrader;
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
            MoveToTrader,
            IdleInTraderQueue,
            MoveToCashRegister,
            IdleInCashRegisterQueue,
            MoveToHome
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
                case CustomerAIStates.MoveToTrader:

                    m_Agent.destination = trader.queue.GetNextPosition();

                    if (IsDestinationReached())
                    {
                        currentState = CustomerAIStates.IdleInTraderQueue;
                        trader.queue.TakeQueue(m_Transform);
                    }

                    break;
                case CustomerAIStates.IdleInTraderQueue:

                    if (!trader.queue.IsInQueue(m_Transform))
                    {
                        currentState = CustomerAIStates.MoveToCashRegister;
                        m_Agent.destination = cashRegister.queue.GetNextPosition();
                    }
                    else
                    {
                        m_Agent.destination = trader.queue.GetPositionInQueue(m_Transform);
                    }

                    break;
                case CustomerAIStates.MoveToCashRegister:
                    m_Agent.destination = cashRegister.queue.GetNextPosition();

                    if (IsDestinationReached())
                    {
                        currentState = CustomerAIStates.IdleInCashRegisterQueue;
                        cashRegister.queue.TakeQueue(m_Transform);
                    }
                    break;
                case CustomerAIStates.IdleInCashRegisterQueue:

                    if (!cashRegister.queue.IsInQueue(m_Transform))
                    {
                        currentState = CustomerAIStates.MoveToHome;
                        m_Agent.destination = m_StartPos;
                    }
                    else
                    {
                        m_Agent.destination = cashRegister.queue.GetPositionInQueue(m_Transform);
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
