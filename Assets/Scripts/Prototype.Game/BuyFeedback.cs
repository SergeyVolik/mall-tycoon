using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype
{
    public class BuyFeedback : MonoBehaviour
    {
        public FloatingText floatingText;
        public AudioSource audioSource;
        public ParticleSystem particlesSystem;

        public void Play(string text)
        {
            floatingText.Show(text);
            audioSource.Play();
            particlesSystem.Play();
        }
    }
}
