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

        private SelfServiceCashier m_SelfServiceCashier;
        public SelfServiceCashier CashierSelfService => m_SelfServiceCashier;

        public CashierBehaviour[] Cashiers => m_Cashiers;
        public TradingSpot[] TradingSpots => m_TradingSpots;

        public int MarketPatrol { get; internal set; }

        [SerializeField] private AudioSource source;
        [SerializeField] private PhysicsCallbacks roomTrigger;

        private int m_UnitsInside;
        public int maxUnitsInMarket;

        [SerializeField]
        private Transform[] m_CustomerEnterPositions;

        [SerializeField]
        private Transform[] m_MarketPatrolPoints;

        [SerializeField]
        private Transform m_PatrolEnter;

        public Vector3 GetRadnomInMarketPosition()
        {
            return m_CustomerEnterPositions[UnityEngine.Random.Range(0, m_CustomerEnterPositions.Length)].position;
        }
        public Vector3 GetPatrolEnter()
        {
            return m_PatrolEnter.position;
        }

        public Vector3 GetRadnomPatrolPoint()
        {
            return m_MarketPatrolPoints[UnityEngine.Random.Range(0, m_MarketPatrolPoints.Length)].position;
        }

        private void Awake()
        {
            roomTrigger.onTriggerEnter += RoomTrigger_onTriggerEnter;
            roomTrigger.onTriggerExit += RoomTrigger_onTriggerExit;

            UpdateCrowdVolume();
            m_Cashiers = GetComponentsInChildren<CashierBehaviour>(true);
            m_TradingSpots = GetComponentsInChildren<TradingSpot>(true);
            m_SelfServiceCashier = GetComponentInChildren<SelfServiceCashier>(true);
            foreach (var item in m_TradingSpots)
            {
                item.onCheckoutFinished += Item_onCheckoutFinished;
            }
        }

        private void Item_onCheckoutFinished()
        {
            m_CurrentCustomesTick++;
        }

        private void RoomTrigger_onTriggerExit(Collider obj)
        {
            m_UnitsInside--;
            UpdateCrowdVolume();
        }

        public IEnumerable<TradingSpot> GetReadyTraders()
        {
            foreach (var item in m_TradingSpots)
            {
                if (item.IsWorking() && item.queue.HasFreePlace())
                {
                    yield return item;
                }
            }
        }

        public bool HasReadyTraders()
        {
            foreach (var item in m_TradingSpots)
            {
                if (item.IsWorking() && item.queue.HasFreePlace())
                {
                    return true;
                }
            }

            return false;
        }

        public TradingSpot GetRandomTraider()
        {
            var tratingSpots = GetReadyTraders();

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

        List<ICashier> m_SameQueueLenCashier = new List<ICashier>();
        public ICashier GetOptimalCashier()
        {
            m_SameQueueLenCashier.Clear();

            int minQueueLen = int.MaxValue;

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

                var curLen = item.queue.Count;

                if (!item.traderAi.IsWorkFinished())
                {
                    curLen++;
                }
                if (minQueueLen == curLen)
                {
                    m_SameQueueLenCashier.Add(item);

                }
                if (minQueueLen > curLen)
                {
                    minQueueLen = curLen;
                    m_SameQueueLenCashier.Clear();
                    m_SameQueueLenCashier.Add(item);
                }
            }

            if (m_SelfServiceCashier && m_SelfServiceCashier.IsWorking())
            {
                if (minQueueLen == m_SelfServiceCashier.CustomerQueue.Count)
                {
                    m_SameQueueLenCashier.Add(m_SelfServiceCashier);
                }
                else if (minQueueLen > m_SelfServiceCashier.CustomerQueue.Count)
                {
                    m_SameQueueLenCashier.Clear();
                    m_SameQueueLenCashier.Add(m_SelfServiceCashier);
                }
            }

            if (m_SameQueueLenCashier.Count == 0)
                return null;

            return m_SameQueueLenCashier[UnityEngine.Random.Range(0, m_SameQueueLenCashier.Count)];
        }

        private void UpdateCrowdVolume()
        {
            source.volume = Mathf.Clamp01(m_UnitsInside / (float)maxUnitsInMarket);
        }

        public bool IsFull() => m_UnitsInside >= maxUnitsInMarket;
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
            return (m_PrevCustomersTick / tickInteval + (m_CurrentCustomesTick * m_CurrenTickTime / tickInteval)) / 2f;
        }
        private const float tickInteval = 60f;
        private int m_CurrentCustomesTick;
        private float m_CurrenTickTime;
        private int m_PrevCustomersTick;
        private void Update()
        {
            m_CurrenTickTime += Time.deltaTime;

            if(m_CurrenTickTime > tickInteval)
            {
                m_CurrenTickTime = 0;
                m_PrevCustomersTick = m_CurrentCustomesTick;
                m_CurrentCustomesTick = 0;
            }
        }

        internal bool HasPatrolFreePlace()
        {
            throw new NotImplementedException();
        }
    }
}
