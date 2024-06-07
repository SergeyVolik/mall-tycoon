using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class LevelUpUIItem : MonoBehaviour
    {
        public TextMeshProUGUI title;
        public TextMeshProUGUI description;
        public TextMeshProUGUI cost;
        public Image icon;
        public TextMeshProUGUI buttonText;
        public Button buyButton;
        public GameObject finishState;
        internal void UpgradeItem(UpgradeData customerSpawnSpeed)
        {
            var playerdata = PlayerData.GetInstance().GetMoney();

            buyButton.interactable = customerSpawnSpeed.GetCostValue() <= playerdata
                && !customerSpawnSpeed.IsMaxLevel();

            cost.text = customerSpawnSpeed.IsMaxLevel() ? "Max" : TextUtils.ValueToShortString(customerSpawnSpeed.GetCostValue());

            ActivateFinishState(customerSpawnSpeed.IsMaxLevel());   
        }

        public void ActivateFinishState(bool enable)
        {
            buyButton.gameObject.SetActive(!enable);
            finishState.SetActive(enable);
        }
    }
}
