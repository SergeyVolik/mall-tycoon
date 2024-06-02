namespace Prototype.UI
{
    public interface INavigateable
    {
        void Show();
        void Hide(bool onlyDisableInput);
    }

    public interface IPageHidedListener
    {
        void OnHided();
    }

    public interface IPageShowedListener
    {
        void OnShowed();
    }
}
