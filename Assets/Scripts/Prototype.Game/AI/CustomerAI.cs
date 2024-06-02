using UnityEngine;
using UnityEngine.AI;

namespace Prototype
{
    public class CustomerAI : MonoBehaviour
    {
        public int buyedProducCost;
        public CustomerAIStates currentState = CustomerAIStates.MoveToTrader;
        private Vector3 m_StartPos;
        private Market m_Market;
        private NavMeshAgent m_Agent;
        private Transform m_Transform;

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
            var trader = m_Market.Trader;

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
                        currentState = CustomerAIStates.MoveToHome;
                        m_Agent.destination = m_StartPos;
                    }
                    else
                    {
                        m_Agent.destination = trader.queue.GetPositionInQueue(m_Transform);
                    }

                    break;
                case CustomerAIStates.MoveToCashRegister:
                    break;
                case CustomerAIStates.IdleInCashRegisterQueue:
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

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(m_Agent.destination, 0.3f);
        }

        private bool IsDestinationReached()
        {
            return Vector3.Distance(m_Transform.position, m_Agent.destination) < 0.5f;
        }
    }
}
