using UnityEngine;

namespace Prototype
{
    public class PRAgency : MonoBehaviour, IActivateableFromRaycast
    {
        public CustomerSpawnSystem m_Spanwer;
        public PRAgencyUI UI;
        public string agencyName;
       
        private void Awake()
        {
            m_Spanwer = FindObjectOfType<CustomerSpawnSystem>();
           
        }

        public void ActivateFromRaycast()
        {
            UI.Bind(this);
            UI.Navigate();
        }
    }
}
