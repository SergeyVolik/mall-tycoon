using System;
using UnityEngine;
using UnityEngine.AI;

namespace Prototype
{
    public class CustomerAI : MonoBehaviour
    {
        public const float tickRate = 0.4f;

        public CustomerAIStates currentState = CustomerAIStates.SelectMarketPosition;
        private Vector3 m_StartPos;
        private NavMeshAgent m_Agent;
        private Transform m_Transform;
     
        [System.NonSerialized]
        public float tickT;
        [System.NonSerialized]
        public float buyedProducCost;
        [System.NonSerialized]
        public ResourceTypeSO holdedResource;
        [System.NonSerialized]
        public TradingSpot selectedTraider;
        [System.NonSerialized]
        public ICashier selectedCashier = null;

        private NavAgentAnimationController m_AnimatorController;
        [SerializeField]
        private GameObject m_ItemSpawnPoint;
        [SerializeField]
        private ParticleSystem m_SpawnItemParticle;
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
            m_Agent = GetComponent<NavMeshAgent>();
            m_Transform = transform;
        }

        private void OnEnable()
        {
            CustomerAIBehaviour.GetInstance().Add(this);
        }

        private void OnDisable()
        {
            var aiBehav = CustomerAIBehaviour.GetInstance();
            if (this != null && aiBehav!= null)
            {
                aiBehav.Remove(this);
            }
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

        public void GoHome()
        {
            currentState = CustomerAIStates.MoveToHome;
            m_Agent.destination = m_StartPos;
        }

        public void SpawnCustomerItem(GameObject itemPrefab)
        {
            if (itemPrefab == null)
                return;

            if (m_SpawnItemParticle)
            {
                m_SpawnItemParticle.Play();
            }
         
            GameObject.Instantiate(itemPrefab, m_ItemSpawnPoint.transform);
            AnimatorController.EnableHasItemState(true);
        }

        public bool IsDestinationReached()
        {
            return Vector3.Distance(m_Transform.position, m_Agent.destination) < 0.5f;
        }

        internal void SetMoveSpeed(float speed)
        {
            m_Agent.speed = speed;
            AnimatorController.SetMoveSpeed(speed);
        }

        internal void ForceDestination(Vector3 position)
        {
            m_Agent.destination = position;
        }
    }
}
