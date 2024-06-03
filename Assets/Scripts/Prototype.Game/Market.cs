using System;
using UnityEngine;

namespace Prototype
{
    public class Market : Singleton<Market>
    {
        public Trader Trader;
        public CashRegister CashRegister;
        [SerializeField] private AudioSource source;
        [SerializeField] private PhysicsCallbacks roomTrigger;
        private int m_UnitsInside;
        public int maxUnitsInMarket;

        private void Awake()
        {
            roomTrigger.onTriggerEnter += RoomTrigger_onTriggerEnter;
            roomTrigger.onTriggerExit += RoomTrigger_onTriggerExit;

            UpdateCrowdVolume();
        }

        private void RoomTrigger_onTriggerExit(Collider obj)
        {
            m_UnitsInside--;

            UpdateCrowdVolume();
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
