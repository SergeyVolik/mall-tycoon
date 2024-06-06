using Unity.VisualScripting;
using UnityEngine;

namespace Prototype
{
    public class TraderAI : MonoBehaviour
    {
        public Cooldown cooldown;
        public CircularCooldownView cooldownView;
        private Camera m_Camera;
        public Transform customerMovePoint;
        public CustomerAI CurrentCustomer { get; private set; }

        private void Awake()
        {
            cooldownView.Bind(cooldown);
            m_Camera = Camera.main;
        }

        public bool IsWorkFinished()
        {
            return cooldown.IsFinished;
        }

        public void Clear()
        {
            CurrentCustomer = null;
        }

        public bool IsHasCustomer()
        {
            return CurrentCustomer != null;
        }

        public void StartWorking(CustomerAI customer)
        {
            CurrentCustomer = customer;
            cooldown.Restart();
        }

        public void Tick()
        {
            if (CurrentCustomer)
            {
                bool customerInBuySpot = Vector3.Distance(CurrentCustomer.transform.position, customerMovePoint.position) < 0.5f;

                if (!customerInBuySpot)
                {
                    cooldownView.cooldownRoot.gameObject.SetActive(false);
                    cooldown.Stop();
                }
                else
                {
                    cooldownView.cooldownRoot.gameObject.SetActive(true);
                    cooldown.Play();
                }
            }
            cooldownView.cooldownRoot.transform.forward = m_Camera.transform.forward;
            cooldown.Tick(Time.deltaTime);
            cooldownView.Tick();
        }
    }
}
