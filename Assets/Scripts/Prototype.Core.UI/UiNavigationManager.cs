using System.Collections.Generic;
using UnityEngine;

namespace Prototype.UI
{
    public class UINavigationManager : Singleton<UINavigationManager>, INavigationManager
    {                     
        public Stack<INavigateable> Navigateables => _navigateables;
        private Stack<INavigateable> _navigateables = new Stack<INavigateable>();

        public bool IsCurrentPage(INavigateable navigateable)
        {
            if (Navigateables.Count == 0)
                return false;

            if (navigateable == Navigateables.Peek())
                return true;

            return false;
        }
        public void Navigate(INavigateable navigateable, bool additive = false)
        {
            if (Navigateables.Count > 0)
            {
                if (Navigateables.Peek() == navigateable)
                {
                    Debug.LogError("Page already active");
                    return;
                }

                var prev = Navigateables.Pop();
                prev.Hide(additive);
                Navigateables.Push(prev);
            }

            navigateable.Show();
            Navigateables.Push(navigateable);
        }

        public INavigateable Pop()
        {
            INavigateable page = null;

            if (Navigateables.Count > 0)
            {
                page = Navigateables.Pop();

                page?.Hide(false);

            }
            else {
                Debug.LogError("Navigateables are empty");
                return null;
            }

            if (Navigateables.Count > 0)
            {
                var prev = Navigateables.Pop();

                prev?.Show();

                Navigateables.Push(prev);
            }

            return page;
        }

        public void PopAll()
        {
            while(Navigateables.Count > 0)
                Pop();
        }
    }

    public interface INavigationManager
    {
        public Stack<INavigateable> Navigateables { get; }
        void Navigate(INavigateable navigateable, bool onlyDisableInput);
        INavigateable Pop();
        bool IsCurrentPage(INavigateable navigateable);
    }
}
