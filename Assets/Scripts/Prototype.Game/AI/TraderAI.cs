using UnityEngine;

namespace Prototype
{
    public class TraderAI : MonoBehaviour
    {
        public Cooldown cooldown;
        public CircularCooldownView cooldownView;
        private Camera m_Camera;
        private Transform m_Transform;
        public Transform customerMovePoint;
        public CustomerAI CurrentCustomer { get; private set; }

        private void Awake()
        {
            cooldownView.Bind(cooldown);
            m_Camera = Camera.main;
            m_Transform = transform;
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
            cooldownView.cooldownRoot.transform.forward = m_Camera.transform.forward;
            cooldown.Tick(Time.deltaTime);
            cooldownView.Tick();

            if (CurrentCustomer)
            {             
                bool customerInBuySpot = Vector3.Distance(CurrentCustomer.transform.position, customerMovePoint.position) < 0.25f;

                if (!customerInBuySpot)
                {
                    cooldownView.cooldownRoot.gameObject.SetActive(false);
                    cooldown.Stop();
                }
                else
                {
                    cooldownView.cooldownRoot.gameObject.SetActive(true);
                    var customerTrans = CurrentCustomer.transform;
                    var vec = (m_Transform.position - customerTrans.position);
                    customerTrans.rotation = Quaternion.Slerp(customerTrans.rotation, Quaternion.LookRotation(vec, Vector3.up), Time.deltaTime * 2);
                    cooldown.Play();
                }
            }
        }
    }
}
