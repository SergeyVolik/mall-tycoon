using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public class GlobalSave
    {
        public List<ResourceSaveItem> playerResources = new List<ResourceSaveItem>();
        public DateTime exitTime;
        public float marketCustomerIncome;
    }

    [System.Serializable]
    public class ResourceSaveItem
    {
        public int resourceTypeHash;
        public float count;
    }

    public class GlobalDataSaveManager : BaseSaveManager<GlobalSave>
    {
        private const string PLAYER_SAVE_KEY = "PLAYER_SAVE_KEY";

        public static GlobalDataSaveManager Instance { get; private set; }

        private PlayerData m_PlayerData;

        public override ISerializedProvider<GlobalSave> SerializerProvider { get; set; }

        public GlobalSave LastLoad { get; private set; }
        public event Action<GlobalSave> OnLoaded = delegate { };

        private void Awake()
        {
            Instance = this;
            m_PlayerData = PlayerData.GetInstance();
            SerializerProvider = new PlayerPrefsSerializer<GlobalSave>();
            Load(PLAYER_SAVE_KEY);
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause == true)
            {
                Save(PLAYER_SAVE_KEY);
                Debug.Log("OnApplicationPause Global Save");
            }
        }

        private void OnApplicationQuit()
        {
            Save(PLAYER_SAVE_KEY);
            Debug.Log("OnApplicationQuit Global Save");
        }

        public override void LoadPass(GlobalSave loadData)
        {
            PlayerData.GetInstance().Resources.resources.Clear();

            foreach (var item in loadData.playerResources)
            {
                var resType = m_PlayerData.ResourceYypes.Types.FirstOrDefault(e => e.GetId() == item.resourceTypeHash);
                m_PlayerData.Resources.resources.SetResource(resType, item.count);
            }
            LastLoad = loadData;
            OnLoaded.Invoke(LastLoad);
        }

        public override void SavePass(GlobalSave saveData)
        {
            foreach (var item in m_PlayerData.Resources.resources.ResourceIterator())
            {
                saveData.playerResources.Add(new ResourceSaveItem
                {
                    count = item.Value,
                    resourceTypeHash = item.Key.GetId(),
                });
            }

            saveData.exitTime = DateTime.Now;
            saveData.marketCustomerIncome = Market.GetInstance().GetTotalIncomePerCustomer();
        }
    }
}