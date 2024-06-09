using UnityEngine;

namespace Prototype
{
    public class SelfServiceCashier : MonoBehaviour, IActivateableFromRaycast, ICashier
    {
        public QueueBehaviour queue;
        public BuyFeedback buyFeedback;

        public QueueBehaviour CustomerQueue => queue;

        public void ActivateFromRaycast()
        {
            if (queue.Count > 0)
            {
                var customerAI = queue.Dequeue();
                customerAI.GoHome();
                buyFeedback.Play(customerAI.buyedProducCost.ToString("0"));

                PlayerData.GetInstance().Resources.resources.AddResource(customerAI.holdedResource, customerAI.buyedProducCost);

                customerAI.buyedProducCost = 0;
                customerAI.holdedResource = null;
            }
        }
    }
}
