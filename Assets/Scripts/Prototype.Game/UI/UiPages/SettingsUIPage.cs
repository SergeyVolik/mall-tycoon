using Prototype.UI;

namespace Prototype
{
    public class SettingsUIPage : UIPage
    {
        public static SettingsUIPage Instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }
    }
}
