using UnityEngine;
using System;
using UnityEngine.Audio;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Prototype.Audio
{
    /// <summary>
    /// Global Audio Manager. Control sounds behaviour
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        private const string SFXVolumeParam = "SFXVolume";
        private const string MasterVolumeParam = "MasterVolume";
        private const string MusicVolumeParam = "MusicVolume";
        private bool m_awaked;

        public AudioSFXSO initMusic;
        public AudioMixer mixer;

        private AudioSource musicSource;

       
        private float masterVolume = 1f;
        private float sfxVolume = 1f;
        private float musicVolume = 1f;

        private void Awake()
        {
            if (m_awaked)
                return;
            m_awaked = true;

            PlayMusic(initMusic);
        }

        void Start()
        {
            LoadSettings();
        }

        void OnDisable()
        {
            SaveSettings();
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveSettings();
            }
        }

        public float GetSFXGlobalVolume()
        {
            return sfxVolume;
        }
        public float GetMasterGlobalVolume()
        {
            return masterVolume;
        }
        public float GetMusicGlobalVolume()
        {
            return musicVolume;
        }
        
        public void SetMasterGlobalVolume(float volume)
        {
            masterVolume = Mathf.Clamp(volume, 0, 1);
            AudioListener.volume = masterVolume;
        }

        public void SetSFXGlobalVolume(float volume)
        {
            var clamped = Mathf.Clamp(volume, 0, 1);
            var value = (volume == 0 ? -80 : MathF.Log10(clamped) * 20);
            sfxVolume = clamped;
            mixer.SetFloat(SFXVolumeParam, value);
        }

        public void SetMusicGlobalVolume(float volume)
        {
            var clamped = Mathf.Clamp(volume, 0, 1);
            var value = (volume == 0 ? -80 : MathF.Log10(clamped) * 20);
            musicVolume = clamped;

            mixer.SetFloat(MusicVolumeParam, value);
        }
        
        /// <summary>
        /// Save audio settings to PlayerPrefs
        /// </summary>
        public void SaveSettings()
        {
            //Debug.Log($"save master v={GetMasterGlobalVolume()}");
            
            PlayerPrefs.SetFloat(MasterVolumeParam, GetMasterGlobalVolume());
            PlayerPrefs.SetFloat(SFXVolumeParam, GetSFXGlobalVolume());
            PlayerPrefs.SetFloat(MusicVolumeParam, GetMusicGlobalVolume());
        }
        /// <summary>
        /// Load audio settings from PlayerPrefs
        /// </summary>
        void LoadSettings()
        {
            SetMasterGlobalVolume(PlayerPrefs.GetFloat(MasterVolumeParam, 1f));
            SetSFXGlobalVolume(PlayerPrefs.GetFloat(SFXVolumeParam, 1f));
            SetMusicGlobalVolume(PlayerPrefs.GetFloat(MusicVolumeParam, 1f));

            //Debug.Log($"load master v={GetMasterGlobalVolume()} pref={PlayerPrefs.GetFloat(MasterVolumeParam, 1f)}");
        }

        /// <summary>
        /// Play sounds FX
        /// </summary>
        /// <param name="audioSFX">audio setting</param>
#if ODIN_INSPECTOR
        [HideInEditorMode]
        [Button]
#endif
        public void PlaySFX(AudioSFXSO audioSFX)
        {
            if (audioSFX == null)
                return;

            audioSFX.Play();
        }


        /// <summary>
        /// Play Music track
        /// </summary>
        /// <param name="audioSFX">audio setting</param>
#if ODIN_INSPECTOR
        [HideInEditorMode]
        [Button]
#endif
        public void PlayMusic(AudioSFXSO audioSFX)
        {
            if (audioSFX == null)
                return;

            if (musicSource == null)
            {
                AudioSourcePool.GetInstance().Get(out musicSource);
                musicSource.loop = true;
            }
            
            audioSFX.Play(musicSource);
        }
    }
}