using System;
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
        private bool IsActive = true;
        private bool m_IsOpened = false;
        private int m_UnitsInside = 0;
        
        private void M_PhysicsCallbacks_onTriggerExit(Collider obj)
        {
            m_UnitsInside--;

            if (!NeedOpen)
                Close();
        }

        private void Close()
        {
            if (!m_IsOpened)
                return;

            m_Animator.Play("DoorClose", 0);
            m_IsOpened = false;
        }

        private void M_PhysicsCallbacks_onTriggerEnter(Collider obj)
        {
            m_UnitsInside++;

            if(IsActive)
                Open();
        }

        private void Open()
        {
            if (m_IsOpened)
                return;

            m_Animator.Play("DoorOpen", 0);
            m_IsOpened = true;
        }

        private bool NeedOpen => m_UnitsInside != 0;
        internal void Disable()
        {
            IsActive = false;

             Close();
        }

        internal void Enable()
        {
            IsActive = true;

            if (NeedOpen)
            {
                Open();
            }
        }
    }
}
