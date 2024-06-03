using UnityEngine;
using UnityEngine.Events;

namespace Prototype
{
    public class CashRegister : MonoBehaviour
    {
        public int producCost = 10;
        public QueueBehaviour queue;
        private Transform m_CurrentCustomer;
        private Camera m_Camera;
        public Cooldown cooldown;
        public CircularCooldownView cooldownView;

        public FloatingText floatingText;
        public UnityEvent onBuyUE;
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
                PlayerData.GetInstance().Resources.resources.AddResource(customerAI.holdedResource, customerAI.buyedProducCost);

                floatingText.Show(customerAI.buyedProducCost.ToString("0"));
                customerAI.buyedProducCost = 0;
                customerAI.holdedResource = null;
                m_CurrentCustomer = null;
                queue.Dequeue();
                onBuyUE.Invoke();
            }

            cooldownView.cooldownRoot.transform.forward = m_Camera.transform.forward;
            cooldown.Tick(Time.deltaTime);
            cooldownView.Tick();
        }
    }
}
