using System;
using UnityEngine;
using UnityEngine.Events;

namespace Prototype
{
    public class PhysicsCallbacks : MonoBehaviour
    {
        public event Action<Collider> onTriggerEnter = delegate { };
        public event Action<Collider> onTriggerExit = delegate { };

        public UnityEvent<Collider> onTriggerEnterUE;
        public UnityEvent<Collider> onTriggerExitUE;

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter.Invoke(other);
            onTriggerEnterUE.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            onTriggerExit.Invoke(other);
            onTriggerExitUE.Invoke(other);
        }
    }
}