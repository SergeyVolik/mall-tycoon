using System;
using UnityEngine;

namespace Prototype.Audio
{
    /// <summary>
    /// Wait while audio have finished and produce and event  
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class OnAudioFinishedEvent : MonoBehaviour
    {
        private AudioSource audioSource;

        public event Action OnAudioFinished = delegate { };

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (!audioSource.isPlaying)
            {
                OnAudioFinished.Invoke();
            }
        }
    }
}