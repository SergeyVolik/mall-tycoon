using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class Trader : MonoBehaviour
    {
        public int producCost = 10;
        public ResourceTypeSO resourceCost;

        public QueueBehaviour queue;
        private Transform m_CurrentCustomer;
        private Camera m_Camera;
        public Cooldown cooldown;
        public CircularCooldownView cooldownView;

        private void Awake()
        {
            m_Camera = Camera.main;

            cooldownView.Bind(cooldown);
        }

        public void Update()
        {
            if (m_CurrentCustomer == null)
            {
                if (queue.TryPeek(out var peek))
                {
                    m_CurrentCustomer = peek;
                    cooldown.Restart();
                }
            }
            else if (cooldown.IsFinished)
            {
                var customerAI = m_CurrentCustomer.GetComponent<CustomerAI>();
                customerAI.buyedProducCost = producCost;
                customerAI.holdedResource = resourceCost;
                m_CurrentCustomer = null;
                queue.Dequeue();
            }

            cooldownView.cooldownRoot.transform.forward = m_Camera.transform.forward;
            cooldown.Tick(Time.deltaTime);
            cooldownView.Tick();
        }
    }
}
