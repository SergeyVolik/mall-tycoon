using UnityEngine;

namespace Prototype
{
    public class TitorialSave : ISaveComponentData
    {
        public SerializableGuid SaveId { get; set; }
        public bool finished;
    }

    public class StartTutorial : MonoBehaviour, IActivateableFromRaycast, ISceneSaveComponent<TitorialSave>
    {
        public DialogueSequenceSO startDialogue;
        public DialogueSequenceSO buyUpgradeDialogue;
        public DialogueSequenceSO upgradeCashierDialogue;
        public DialogueSequenceSO showPrAgency;
        public DialogueSequenceSO lastDialogue;


        public AutomaticDoors doors;
        public TradingSpot tradingSpot;
        public CashierBehaviour cashier;
        public PRAgency prAgency;


        private TutorState state = TutorState.OpenMarket;

        public bool finished = false;

        [field:SerializeField]
        public SerializableGuid SaveId { get; set; }

        private enum TutorState
        {
            OpenMarket,
            WaitFirstCustomerFinish
        }

        public void ActivateFromRaycast()
        {
            if (state != TutorState.OpenMarket)
                return;

            Market.GetInstance().OpenMarket();
            doors.Enable();
            state = TutorState.WaitFirstCustomerFinish;

            tradingSpot.onCustomerFinished += TradingSpot_onCheckoutFinished;
        }

        private void TradingSpot_onCheckoutFinished()
        {
            tradingSpot.onCustomerFinished -= TradingSpot_onCheckoutFinished;
            CameraController.GetInstance().ForceTarget(tradingSpot.transform);
            buyUpgradeDialogue.StartDialogue();
            buyUpgradeDialogue.onFinished += BuyUpgradeDialogue_onFinished;
        }

        private void BuyUpgradeDialogue_onFinished()
        {
            CameraController.GetInstance().ForceTarget(null);
            buyUpgradeDialogue.onFinished -= BuyUpgradeDialogue_onFinished;

            TraderUpgradeUI.Instance.closeButton.onClick.AddListener(OnCloseTraiderUI);
        }

        void OnCloseTraiderUI()
        {
            TraderUpgradeUI.Instance.closeButton.onClick.RemoveListener(OnCloseTraiderUI);
            upgradeCashierDialogue.StartDialogue();
            CameraController.GetInstance().ForceTarget(cashier.transform);
            upgradeCashierDialogue.onFinished += UpgradeCashierDialogue_onFinished;
        }
        void OnCloseCashierUI()
        {
            CashiersUpgradeUI.Instance.closeButton.onClick.RemoveListener(OnCloseCashierUI);
            showPrAgency.StartDialogue();
            CameraController.GetInstance().ForceTarget(prAgency.enterPoint);

            showPrAgency.onFinished += ShowPrAgency_onFinished;
        }

        private void ShowPrAgency_onFinished()
        {
            CameraController.GetInstance().ForceTarget(null);
            showPrAgency.onFinished -= ShowPrAgency_onFinished;
            PRAgencyUI.Instance.closeButton.onClick.AddListener(ClosePRAgencyUI);
        }

        void ClosePRAgencyUI()
        {
            PRAgencyUI.Instance.closeButton.onClick.RemoveListener(ClosePRAgencyUI);
            lastDialogue.StartDialogue();
        }

        private void UpgradeCashierDialogue_onFinished()
        {
            CameraController.GetInstance().ForceTarget(null);
            upgradeCashierDialogue.onFinished -= UpgradeCashierDialogue_onFinished;
            CashiersUpgradeUI.Instance.closeButton.onClick.AddListener(OnCloseCashierUI);
        }

        private void Start()
        {
            if (!finished)
            {
                finished = true;
                Market.GetInstance().IsOpened = false;
                doors.Disable();
                startDialogue.StartDialogue();

                var marketQueue = Market.GetInstance().marketEnterQueue;

                for (int i = 0; i < marketQueue.queuePoints.Length; i++)
                {
                    var spawnpos = marketQueue.queuePoints[i].position;
                    var customer = CustomerSpawnSystem.GetInstance().SpawnCustomerAtPosition(spawnpos);
                    customer.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                }
            }
        }

        public TitorialSave SaveComponent()
        {
            return new TitorialSave
            {
                SaveId = SaveId,
                finished = finished
            };
        }

        public void LoadComponent(TitorialSave data)
        {
            finished = data.finished;
        }
    }
}
