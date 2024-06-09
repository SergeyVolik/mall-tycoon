using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Prototype.CustomerAI;

namespace Prototype
{
    public class CustomerAIBehaviour : Singleton<CustomerAIBehaviour>
    {
        private List<CustomerAI> m_Customers = new List<CustomerAI>();
        private List<CustomerAI> m_RemoveList = new List<CustomerAI>();
        public int ActiveCustomers => m_Customers.Count;
        public void Add(CustomerAI customer)
        {
            m_Customers.Add(customer);
        }

        public void Remove(CustomerAI customer)
        {
            m_RemoveList.Add(customer);
        }

        private void Update()
        {
            var m_Market = Market.GetInstance();
            var deltaTime = Time.deltaTime;

            foreach (var customer in m_Customers)
            {
                if (customer == null)
                {
                    m_RemoveList.Add(customer);
                    continue;
                }

                customer.tickT += deltaTime;

                if (CustomerAI.tickRate > customer.tickT)
                    continue;

                customer.tickT = 0;

                switch (customer.currentState)
                {
                    case CustomerAIStates.SelectMarketPosition:
                        customer.currentState = CustomerAIStates.MoveToMarket;
                        customer.ForceDestination(Market.GetInstance().GetRadnomInMarketPosition());

                        break;

                    case CustomerAIStates.MoveToMarket:
                        if (customer.IsDestinationReached())
                        {
                            customer.selectedTraider = m_Market.GetRandomTraider();

                            if (customer.selectedTraider == null)
                            {
                                customer.GoHome();
                            }
                            else
                            {
                                customer.currentState = CustomerAIStates.MoveToTraderQueue;
                            }
                        }
                        break;

                    case CustomerAIStates.MoveToTraderQueue:

                        if (!customer.selectedTraider.queue.HasFreePlace())
                        {
                            customer.GoHome();
                            return;
                        }

                        customer.ForceDestination(customer.selectedTraider.queue.GetNextPosition());

                        if (customer.IsDestinationReached())
                        {
                            customer.currentState = CustomerAIStates.WaitInTraderQueue;
                            customer.selectedTraider.queue.TakeQueue(customer);
                        }
                        break;
                    case CustomerAIStates.WaitInTraderQueue:

                        customer.ForceDestination(customer.selectedTraider.queue.GetPositionInQueue(customer));
                        break;
                    case CustomerAIStates.WaitTraderWork:
                        if (customer.buyedProducCost != 0)
                        {
                            customer.currentState = CustomerAIStates.MoveToCashRegisterQueue;
                            customer.selectedCashier = Market.GetInstance().GetOptimalCashier();

                            if (customer.selectedCashier == null)
                            {
                                Debug.LogWarning("cashier not found");
                                customer.GoHome();
                                return;
                            }
                            customer.ForceDestination(customer.selectedCashier.CustomerQueue.GetNextPosition());
                        }
                        break;
                    case CustomerAIStates.WaitCashRegister:
                        if (customer.buyedProducCost == 0)
                        {
                            customer.GoHome();
                        }
                        break;
                    case CustomerAIStates.MoveToCashRegisterQueue:

                        if (!customer.selectedCashier.CustomerQueue.HasFreePlace())
                        {
                            customer.GoHome();
                            return;
                        }

                        customer.ForceDestination(customer.selectedCashier.CustomerQueue.GetNextPosition());

                        if (customer.IsDestinationReached())
                        {
                            customer.currentState = CustomerAIStates.IdleInCashRegisterQueue;
                            customer.selectedCashier.CustomerQueue.TakeQueue(customer);
                        }
                        break;
                    case CustomerAIStates.IdleInCashRegisterQueue:

                        customer.ForceDestination(customer.selectedCashier.CustomerQueue.GetPositionInQueue(customer));
                        break;
                    case CustomerAIStates.MoveToHome:
                        if (customer.IsDestinationReached())
                        {
                            m_RemoveList.Add(customer);
                            GameManager.Destroy(customer.gameObject, 0.1f);
                        }
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in m_RemoveList)
            {
                m_Customers.Remove(item);
            }

            m_RemoveList.Clear();
        }
    }
}
