using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.UI
{
    public class TabWindow : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI Title;
        public RectTransform Content;

        public void SetTitle(string title)
        {
            Title.text = title;
        }
        public string GetTitle()
        {
            return Title.text;
        }
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}