using UnityEngine;
using UnityEngine.UI;

namespace Prototype.UI
{
    [RequireComponent(typeof(Button))]
    public class PopPage : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                UINavigationManager.GetInstance().Pop();
            });
        }
    }
}
