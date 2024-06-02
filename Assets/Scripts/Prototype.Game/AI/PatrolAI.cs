using UnityEngine;
using UnityEngine.AI;

namespace Prototype
{
    public class PatrolAI : MonoBehaviour
    {
        public Transform[] patrolPoints;
        public int m_CurrentIndex;
        private NavMeshAgent m_Agent;
        private Transform m_Transform;
        public float distanceToNextPoint = 0.5f;

        private void Awake()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_Transform = transform;
        }

        private void Start()
        {
            m_Agent.destination = patrolPoints[m_CurrentIndex].position;
        }

        private void Update()
        {
            var destination = patrolPoints[m_CurrentIndex].position;

            if (Vector3.Distance(destination, m_Transform.position) < distanceToNextPoint)
            {
                m_CurrentIndex++;
                m_CurrentIndex = m_CurrentIndex % patrolPoints.Length;
                destination = patrolPoints[m_CurrentIndex].position;
                m_Agent.destination = destination;             
            }
        }
    }
}
