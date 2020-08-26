using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using BeatGame.Logic.Managers;

namespace BeatGame.Logic.Audio
{
    public class AudioVolumeManager : MonoBehaviour
    {
        [SerializeField]
        AudioMixer audioMixer;

        private void Start()
        {
            SettingsManager.Instance.OnConfigChanged += UpdateVolume;
            if (SettingsManager.Instance.HasLoadedSettings == true)
            {
                UpdateVolume();
            }
        }

        private void UpdateVolume()
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(SettingsManager.Instance.Settings["Audio"]["MasterVolume"].FloatValue) * 30);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(SettingsManager.Instance.Settings["Audio"]["MusicVolume"].FloatValue) * 30);
            audioMixer.SetFloat("EffectsVolume", Mathf.Log10(SettingsManager.Instance.Settings["Audio"]["EffectsVolume"].FloatValue) * 30);
        }
    }
}