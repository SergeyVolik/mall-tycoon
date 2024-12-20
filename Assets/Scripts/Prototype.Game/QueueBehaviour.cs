using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class QueueBehaviour : MonoBehaviour
    {
        private Queue<CustomerAI> m_Customers = new Queue<CustomerAI>();
        public Transform[] queuePoints;
        public float offsetBetweenCustomers;

        public IEnumerable<CustomerAI> Customers => m_Customers;
        public void TakeQueue(CustomerAI customer)
        {
            m_Customers.Enqueue(customer);
            UpdateQueue();
        }

        public int MaxSize => queuePoints.Length;
        public bool HasFreePlace() => m_Customers.Count < queuePoints.Length && gameObject.activeSelf;
        public CustomerAI Dequeue()
        { 
            var item = m_Customers.Dequeue();
            UpdateQueue();

            return item;
        }

        public void Clear()
        {
            m_Customers.Clear();
            UpdateQueue();
        }

        public CustomerAI Peek()
        {
            return m_Customers.Peek();
        }

        void UpdateQueue()
        {
            int i = 0;
            foreach (var item in m_Customers)
            {
                item.ForceDestination(queuePoints[i].position);
                i++;
            }
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
