using UnityEngine;
using UnityEngine.Audio;

namespace Prototype.Audio
{
    /// <summary>
    /// Audio setting for playing <see cref="AudioSource">
    /// </summary>
    [CreateAssetMenu(menuName = "Prototype/AudioSFX")]
    public class AudioSFXSO : ScriptableObject
    {
        /// <summary>
        /// Setup and play AudioSource
        /// </summary>
        /// <param name="source"></param>
        public void Play(AudioSource source)
        {
            Data.Play(source);
        }

        public void Play()
        {
            Data.Play();
        }

        public PlaySFXData Data;
    }

    [System.Serializable]
    public class PlaySFXData
    {

        public Vector2 volumeRange = new Vector2(1, 1);

        public Vector2 pinch = new Vector2(1, 1);

        public AudioClip[] clips;

        public AudioMixerGroup mixer;

        /// <summary>
        /// Setup and play AudioSource
        /// </summary>
        /// <param name="source"></param>
        public void Play(AudioSource source)
        {
            source.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            source.volume = UnityEngine.Random.Range(volumeRange.x, volumeRange.y);
            source.pitch = UnityEngine.Random.Range(pinch.x, pinch.y);

            source.outputAudioMixerGroup = mixer;
            source.Play();
        }

        public AudioSource Play()
        {
            if(clips.Length == 0)
                return null;

 
            AudioSourcePool.GetInstance().Get(out var source);           
            Play(source);

            return source;
        }
    }     
}
