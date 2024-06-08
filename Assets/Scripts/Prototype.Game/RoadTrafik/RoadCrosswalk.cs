using UnityEngine;

namespace Prototype
{
    public class RoadCrosswalk : MonoBehaviour
    {
        public int count;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<CustomerAI>())
                count++;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<CustomerAI>())
                count--;
        }

        public bool IsEmpty => count == 0;
    }
}
