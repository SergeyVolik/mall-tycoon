using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class QueueBehaviour : MonoBehaviour
    {
        private Queue<Transform> m_Customers = new Queue<Transform>();
        public Transform queueStartPoint;
        public float offsetBetweenCustomers;

        public void TakeQueue(Transform customer)
        {
            m_Customers.Enqueue(customer);
        }

        public void Dequeue()
        { 
            m_Customers.Dequeue();
        }

        public Transform Peek()
        {
            return m_Customers.Peek();
        }

        public bool TryPeek(out Transform peek)
        {
            return m_Customers.TryPeek(out peek);
        }

        public bool IsInQueue(Transform customer)
        {
            foreach (var item in m_Customers)
            {
                if (item == customer)
                    return true;
            }

            return false;
        }

        public Vector3 GetPositionInQueue(Transform customer)
        {
            int positionInQueue = 0;

            foreach (var item in m_Customers)
            {
                if (item == customer)
                    break;
                positionInQueue++;              
            }

            Vector3 customerPos = queueStartPoint.position + (queueStartPoint.forward * offsetBetweenCustomers * positionInQueue);

            return customerPos;
        }

        public Vector3 GetNextPosition()
        {
            int positionInQueue = m_Customers.Count;
            return queueStartPoint.position + (queueStartPoint.forward * offsetBetweenCustomers * positionInQueue);
        }
    }
}
