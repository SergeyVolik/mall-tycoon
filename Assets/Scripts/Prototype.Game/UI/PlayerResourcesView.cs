namespace Prototype
{
    public class PlayerResourcesView : ResourceView
    {
        private void Start()
        {          
            Bind(PlayerData.GetInstance().Resources.resources);
        }
    }
}
