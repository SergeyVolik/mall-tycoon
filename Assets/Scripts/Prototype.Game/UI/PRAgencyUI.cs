using Prototype.UI;
using TMPro;

namespace Prototype
{
    public class PRAgencyUI : UIPage
    {
        public LevelUpUIItem customersMoveSpeedLevelUpUI;
        public LevelUpUIItem customerSpawnSpeedLevelUpUI;
        public TextMeshProUGUI moveSpeedText;
        public TextMeshProUGUI spawnSpeedText;
        private PRAgency m_PRAgency;

        protected override void Awake()
        {
            base.Awake();

            customersMoveSpeedLevelUpUI.buyButton.onClick.AddListener(() =>
            {
                m_PRAgency.m_Spanwer.LevelUpMoveSpeed();
            });

            customerSpawnSpeedLevelUpUI.buyButton.onClick.AddListener(() =>
            {
                m_PRAgency.m_Spanwer.LevelUpSpawnSpeed();
            });
        }

        public void Bind(PRAgency prAgency)
        {
            m_PRAgency = prAgency;
            m_PRAgency.m_Spanwer.OnChanged += UpdateUI;
            UpdateUI();
        }

        public override void Show()
        {
            RaycastInput.GetInstance().BlockRaycast = true;
            base.Show();
        }

        public override void Hide(bool onlyDisableRaycast = false)
        {
            RaycastInput.GetInstance().BlockRaycast = false;
            base.Hide(onlyDisableRaycast);
        }

        void UpdateUI()
        {
            moveSpeedText.text = $"customers move speed: {m_PRAgency.m_Spanwer.GetCurrentMoveSpeed()}";
            spawnSpeedText.text = $"customers spawn speed: {m_PRAgency.m_Spanwer.SpawnsPerMinute()} p/m";

            customersMoveSpeedLevelUpUI.buyButton.interactable = true;
            customersMoveSpeedLevelUpUI.cost.text = m_PRAgency.m_Spanwer.LevelUpMoveSpeedCost().ToString("0");
            customerSpawnSpeedLevelUpUI.buyButton.interactable = true;
            customerSpawnSpeedLevelUpUI.cost.text = m_PRAgency.m_Spanwer.LevelUpSpawnSpeedCost().ToString("0");
        }
    }
}
