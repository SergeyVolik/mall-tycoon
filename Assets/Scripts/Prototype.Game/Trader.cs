using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class Trader : MonoBehaviour
    {
        public float buyProductTime = 2f;
        public int producCost = 10;
        float buyProducT;
        public QueueBehaviour queue;
        private Transform m_CurrentCustomer;
        public Image timerImage;
        public Canvas timerCanvas;
        private Camera m_Camera;

        private void Awake()
        {
            m_Camera = Camera.main;
        }

        public void Update()
        {
            if (m_CurrentCustomer == null)
            {
                if (queue.TryPeek(out var peek))
                {
                    m_CurrentCustomer = peek;
                }

                timerCanvas.gameObject.SetActive(false);
            }
            else {
                buyProducT += Time.deltaTime;
                timerImage.fillAmount = buyProducT / buyProductTime;
                timerCanvas.gameObject.SetActive(true);
                timerCanvas.transform.forward = m_Camera.transform.forward;
                if (buyProducT > buyProductTime)
                {                  
                    buyProducT = 0;
                    var customerAI = m_CurrentCustomer.GetComponent<CustomerAI>();
                    customerAI.buyedProducCost = producCost;
                    m_CurrentCustomer = null;
                    queue.Dequeue();
                }
            }
        }
    }
}
