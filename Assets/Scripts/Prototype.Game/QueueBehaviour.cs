using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class QueueBehaviour : MonoBehaviour
    {
        private Queue<CustomerAI> m_Customers = new Queue<CustomerAI>();
        public Transform[] queuePoints;
        public float offsetBetweenCustomers;

        public void TakeQueue(CustomerAI customer)
        {
            m_Customers.Enqueue(customer);
        }

        public bool HasFreePlace() => m_Customers.Count < queuePoints.Length-1 && gameObject.activeSelf;
        public CustomerAI Dequeue()
        { 
            return m_Customers.Dequeue();
        }

        public CustomerAI Peek()
        {
            return m_Customers.Peek();
        }

        public int Count => m_Customers.Count;

        public bool TryPeek(out CustomerAI peek)
        {
            return m_Customers.TryPeek(out peek);
        }

        public bool IsInQueue(CustomerAI customer)
        {
            foreach (var item in m_Customers)
            {
                if (item == customer)
                    return true;
            }

            return false;
        }

        public Vector3 GetPositionInQueue(CustomerAI customer)
        {
            int positionInQueue = 0;

            foreach (var item in m_Customers)
            {
                if (item == customer)
                    break;
                positionInQueue++;              
            }

            Vector3 customerPos = queuePoints[positionInQueue].position;

            return customerPos;
        }

        public Vector3 GetNextPosition()
        {
            return queuePoints[m_Customers.Count].position;
        }
    }
}
