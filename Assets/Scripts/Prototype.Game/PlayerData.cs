using System;

namespace Prototype
{
    public class PlayerData : Singleton<PlayerData>
    {
        public PlayerResources Resources;
        public ResourceTypeSO moneyResource;
        public ResourcesTypesSO ResourceYypes;
        private void Awake()
        {
            Resources.resources.onResourceChanged += Resources_onResourceChanged;
        }

        private void OnDestroy()
        {
            Resources.resources.onResourceChanged -= Resources_onResourceChanged;
        }
        private void Resources_onResourceChanged(ResourceTypeSO arg1, float arg2)
        {
            if (moneyResource == arg1)
            {
                onMoneyChanged.Invoke(arg2);
            }
        }

        public float GetMoney() => Resources.resources.GetResource(moneyResource);
        public void DecreaseMoney(float money) => Resources.resources.RemoveResource(moneyResource, money);
        public void IncreaseMoney(float money) => Resources.resources.AddResource(moneyResource, money);

        public event Action<float> onMoneyChanged = delegate { };
    }
}
