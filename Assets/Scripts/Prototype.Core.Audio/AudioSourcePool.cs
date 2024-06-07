using Prototype.Audio;
using System;
using UnityEngine;

namespace Prototype
{
    public class AudioSourcePool : Singleton<AudioSourcePool>
    {
        public GameObject audioItemPrefab;
        protected AudioSource CreatePooledItem()
        {
            var go = new GameObject("sfx");
            var ps = go.AddComponent<AudioSource>();

            go.AddComponent<OnAudioFinishedEvent>().OnAudioFinished += () =>
            {
                GameObjectPool.ReleaseStatic(audioItemPrefab, go);
            };

            return ps;
        }

        internal void Get(out AudioSource musicSource)
        {
            musicSource = GameObjectPool.GetPoolObject(audioItemPrefab).GetComponent<AudioSource>();
            musicSource.gameObject.SetActive(true);
        }
    }
}