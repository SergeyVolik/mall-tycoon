using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.UI
{
    public class TabButton : MonoBehaviour
    {
        public Button button;
        public TMPro.TextMeshProUGUI tabButtonText;

        public void SetText(string text)
        {
            tabButtonText.text = text;
        }
    }
}