using Prototype.UI;
using TMPro;
using UnityEngine.UI;

namespace Prototype
{
    public class PRAgencyUI : UIPage
    {
        public LevelUpUIItem customersMoveSpeedLevelUpUI;
        public LevelUpUIItem customerSpawnSpeedLevelUpUI;
        public TextMeshProUGUI moveSpeedText;
        public TextMeshProUGUI spawnSpeedText;
        private bool m_Binded;
        private PRAgency m_PRAgency;

        public static PRAgencyUI Instance { get; private set; }
        public Button closeButton;
        protected override void Awake()
        {
            base.Awake();

            customersMoveSpeedLevelUpUI.buyButton.GetComponent<HoldedButton>().onClick += () =>
            {
                m_PRAgency.m_Spanwer.customerMoveSpeed.LevelUp();
            };

            customerSpawnSpeedLevelUpUI.buyButton.GetComponent<HoldedButton>().onClick += () =>
            {
                m_PRAgency.m_Spanwer.customerSpawnSpeed.LevelUp();
            };

            Instance = this;
            closeButton.onClick.AddListener(() =>
            {

            });
        }

        public void Bind(PRAgency prAgency)
        {
            Unbind();

            m_PRAgency = prAgency;
            PlayerData.GetInstance().onMoneyChanged += PRAgencyUI_onMoneyChanged;
            m_PRAgency.m_Spanwer.customerMoveSpeed.onChanged += UpdateUI;
            m_PRAgency.m_Spanwer.customerSpawnSpeed.onChanged += UpdateUI;

            UpdateUI();
        }

        private void Unbind()
        {
            if (m_PRAgency)
            {
                m_PRAgency.m_Spanwer.customerMoveSpeed.onChanged -= UpdateUI;
                m_PRAgency.m_Spanwer.customerSpawnSpeed.onChanged -= UpdateUI;
                PlayerData.GetInstance().onMoneyChanged -= PRAgencyUI_onMoneyChanged;
            }

            m_PRAgency = null;
        }

        private void PRAgencyUI_onMoneyChanged(float obj)
        {
            UpdateUI();
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide(bool onlyDisableRaycast = false)
        {
            base.Hide(onlyDisableRaycast);
            Unbind();
        }

        void UpdateUI()
        {
            var customerMoveSpeed = m_PRAgency.m_Spanwer.customerMoveSpeed;
            var customerSpawnSpeed = m_PRAgency.m_Spanwer.customerSpawnSpeed;

            moveSpeedText.text = $"customers move speed: {customerMoveSpeed.GetValue().ToString("0.0")}";
            spawnSpeedText.text = $"customers spawn speed: {m_PRAgency.m_Spanwer.SpawnsPerMinute().ToString("0.0")} p/m";

            customersMoveSpeedLevelUpUI.UpgradeItem(customerMoveSpeed);
            customerSpawnSpeedLevelUpUI.UpgradeItem(customerSpawnSpeed);        
        }
    }
}
