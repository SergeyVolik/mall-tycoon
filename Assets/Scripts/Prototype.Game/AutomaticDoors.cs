using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class AutomaticDoors : MonoBehaviour
    {
        private Animator m_Animator;

        [SerializeField]
        private PhysicsCallbacks m_PhysicsCallbacks;
        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_PhysicsCallbacks.onTriggerEnter += M_PhysicsCallbacks_onTriggerEnter;
            m_PhysicsCallbacks.onTriggerExit += M_PhysicsCallbacks_onTriggerExit;

        }

        private int m_UnitsInside = 0;
        private void M_PhysicsCallbacks_onTriggerExit(Collider obj)
        {
            m_UnitsInside--;

            if(m_UnitsInside == 0)
                m_Animator.Play("DoorClose", 0);

        }

        private void M_PhysicsCallbacks_onTriggerEnter(Collider obj)
        {
            m_UnitsInside++;

            m_Animator.Play("DoorOpen", 0);
        }
    }
}
