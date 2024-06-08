using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prototype
{
    public class Market : Singleton<Market>
    {
        private TradingSpot[] m_TradingSpots;
        private CashierBehaviour[] m_Cashiers;
        public CashierBehaviour[] Cashiers => m_Cashiers;
        public TradingSpot[] TradingSpots => m_TradingSpots;

        [SerializeField] private AudioSource source;
        [SerializeField] private PhysicsCallbacks roomTrigger;

        private int m_UnitsInside;
        public int maxUnitsInMarket;

        [SerializeField]
        private Transform[] m_CustomerEnterPositions;

        public Vector3 GetRadnomInMarketPosition()
        {
            return m_CustomerEnterPositions[UnityEngine.Random.Range(0, m_CustomerEnterPositions.Length)].position;
        }

        private void Awake()
        {
            roomTrigger.onTriggerEnter += RoomTrigger_onTriggerEnter;
            roomTrigger.onTriggerExit += RoomTrigger_onTriggerExit;

            UpdateCrowdVolume();
            m_Cashiers = GetComponentsInChildren<CashierBehaviour>(true);
            m_TradingSpots = GetComponentsInChildren<TradingSpot>(true);

            foreach (var item in m_TradingSpots)
            {
                item.onCheckoutFinished += Item_onCheckoutFinished;
            }
        }

        private void Item_onCheckoutFinished()
        {
            currentMinuteCustomes++;
        }

        private void RoomTrigger_onTriggerExit(Collider obj)
        {
            m_UnitsInside--;
            UpdateCrowdVolume();
        }

        private IEnumerable<TradingSpot> WorkedSpots()
        {
            foreach (var item in m_TradingSpots)
            {
                if (item.IsWorking())
                {
                    yield return item;
                }
            }
        }

        public TradingSpot GetRandomTraider()
        {
            var tratingSpots = WorkedSpots();

            var len = tratingSpots.Count();
            if (len == 0)
                return null;

            var rndIndex = UnityEngine.Random.Range(0, len);
            int i = 0;

            foreach (var item in tratingSpots)
            {
                if (i == rndIndex)
                    return item;
                i++;
            }

            return null;
        }

        public CashierBehaviour GetOptimalCashRegister()
        {
            int minQueueLen = int.MaxValue;
            CashierBehaviour result = null;

            foreach (var item in m_Cashiers)
            {
                if (!item.IsWorking())
                {
                    continue;
                }

                if (!item.queue.HasFreePlace())
                {
                    continue;
                }

                if (minQueueLen > item.queue.Count)
                {
                    minQueueLen = item.queue.Count;
                    result = item;
                }
            }
            return result;
        }

        private void UpdateCrowdVolume()
        {
            source.volume = Mathf.Clamp01(m_UnitsInside / (float)maxUnitsInMarket);
        }

        private void RoomTrigger_onTriggerEnter(Collider obj)
        {
            m_UnitsInside++;
            UpdateCrowdVolume();
        }


        internal float GetTotalIncomePerCustomer()
        {
            float cost = 0;

            foreach (var item in m_TradingSpots)
            {
                if (!item.IsWorking())
                    continue;

                cost += item.costUpgrade.GetProducCost();
            }

            return cost;
        }

        internal float GetAverageCheckoutQueueTimeInSeconds()
        {
            float sum = 0;
            int len = 0;
            foreach (var item in Cashiers)
            {
                if (!item.IsWorking())
                    continue;

                var seconds = item.workerSpeedUpgrade.GetValue();
                len++;
                sum += seconds * item.queue.Count;
            }

            return sum / len;
        }

        internal float GetAverageTraderTimeInSeconds()
        {
            float sum = 0;
            int len = 0;

            foreach (var item in m_TradingSpots)
            {
                if (!item.IsWorking())
                    continue;

                var seconds = item.workerSpeedUpgrade.GetValue();
                len++;

                sum += seconds * item.queue.Count;
            }

            return sum / len;
        }

        internal float GetAverageCheckoutCustomersInQueue()
        {
            float sum = 0;
            int len = 0;
            foreach (var item in Cashiers)
            {
                if (!item.IsWorking())
                    continue;

                sum += item.queue.Count;
                len++;
            }

            sum /= len;

            return sum;
        }

        internal float GetCustomersPerMinute()
        {          
            return (prevMinuteCustomers/60f + (currentMinuteCustomes * currenMinTime/60f)) / 2f;
        }

        private int currentMinuteCustomes;
        private float currenMinTime;
        private int prevMinuteCustomers;
        private void Update()
        {
            currenMinTime += Time.deltaTime;

            if(currenMinTime > 60f)
            {
                currenMinTime = 0;
                prevMinuteCustomers = currentMinuteCustomes;
                currentMinuteCustomes = 0;
            }
        }
    }
}
