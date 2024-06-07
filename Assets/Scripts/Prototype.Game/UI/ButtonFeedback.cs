using Prototype.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype
{
    public class ButtonFeedback : MonoBehaviour
    {
        public PlaySFXData sfx;
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                sfx.Play();
            });
        }
    }
}
