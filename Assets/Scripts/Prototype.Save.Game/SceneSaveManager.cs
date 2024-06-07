using System.Collections.Generic;

namespace Prototype
{
    [System.Serializable]
    public class SceneSaveData
    {
        public List<TransformSave> TransSave = new List<TransformSave>();
        public List<GameObjectSave> GoSave = new List<GameObjectSave>();
        public List<CashierBehaviourSave> Cashiers = new List<CashierBehaviourSave>();
        public List<TradingSpotSaveData> TradingSpots = new List<TradingSpotSaveData>();
        public List<CustomerSpawnerSave> Spawners = new List<CustomerSpawnerSave>();
    }

    public class SceneSaveManager : BaseSaveManager<SceneSaveData>
    {
        public string SaveKey;

        public override ISerializedProvider<SceneSaveData> SerializerProvider { get; set; }

        public override void LoadPass(SceneSaveData sceneSaveData)
        {
            SaveHelper.LoadComponents<TransformSave, SaveTransform>(sceneSaveData.TransSave);
            SaveHelper.LoadComponents<GameObjectSave, SaveGameObjectState>(sceneSaveData.GoSave);
            SaveHelper.LoadComponents<CustomerSpawnerSave, CustomerSpawnSystem>(sceneSaveData.Spawners);
            SaveHelper.LoadComponents<TradingSpotSaveData, TradingSpot>(sceneSaveData.TradingSpots);
            SaveHelper.LoadComponents<CashierBehaviourSave, CashierBehaviour>(sceneSaveData.Cashiers);
        }

        public override void SavePass(SceneSaveData sceneSaveData)
        {
            sceneSaveData.TransSave = SaveHelper.SaveComponents<TransformSave, SaveTransform>();
            sceneSaveData.GoSave = SaveHelper.SaveComponents<GameObjectSave, SaveGameObjectState>();
            sceneSaveData.Spawners = SaveHelper.SaveComponents<CustomerSpawnerSave, CustomerSpawnSystem>();
            sceneSaveData.TradingSpots = SaveHelper.SaveComponents<TradingSpotSaveData, TradingSpot>();
            sceneSaveData.Cashiers = SaveHelper.SaveComponents<CashierBehaviourSave, CashierBehaviour>();
        }

        private void Start()
        {
            SerializerProvider = new PlayerPrefsSerializer<SceneSaveData>();
            Load(SaveKey);
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause == true)
            {
                Save(SaveKey);
            }
        }

        private void OnApplicationQuit()
        {
            Save(SaveKey);
        }
    }
}
