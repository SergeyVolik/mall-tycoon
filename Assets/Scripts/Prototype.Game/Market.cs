using System;
using UnityEngine;

namespace Prototype
{
    public class Market : Singleton<Market>
    {
        public TradingSpot Trader;
        private CashierBehaviour[] m_Cashiers;
        public CashierBehaviour[] Cashiers => m_Cashiers;
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
        }

        private void RoomTrigger_onTriggerExit(Collider obj)
        {
            m_UnitsInside--;
            UpdateCrowdVolume();
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
    }
}
