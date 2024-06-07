using Newtonsoft.Json;
using UnityEngine;

namespace Prototype
{
    public class PlayerPrefsSerializer<T> : ISerializedProvider<T> where T : class, new()
    {
        public void SerializedData(T data, string key)
        {
            var str = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(key, str);
        }

        public T DerializedData(string key)
        {
            var data = JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(key));
            return data;
        }
    }

    public abstract class BaseSaveManager<T> : MonoBehaviour, ISaveManager<T> where T : class, new()
    {
        public abstract ISerializedProvider<T> SerializerProvider { get; set; }

        public void Save(string key)
        {
            var saveData = new T();

            SavePass(saveData);
            SerializerProvider.SerializedData(saveData, key);
        }

        public void Load(string key)
        {
            if (!PlayerPrefs.HasKey(key))
                return;

            var loadData = SerializerProvider.DerializedData(key);

            LoadPass(loadData);
        }

        public abstract void SavePass(T saveData);
        public abstract void LoadPass(T LoadData);
    }
}