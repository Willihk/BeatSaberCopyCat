using SharpConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;

namespace BeatGame.Logic.Managers
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance;

        public static float3 LineOffset = new float3(1f, 1f, 0);
        public static float3 GlobalOffset = new float3(0, 0, 3);

        public event Action OnConfigLoaded;
        public event Action OnConfigChanged;
        public event Action OnConfigSaved;

        public Configuration Settings;
        public bool HasLoadedSettings;

        string configFile = "/Settings.cfg";

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            if (File.Exists(Application.persistentDataPath + configFile))
            {
                LoadConfig();
            }
            else
            {
                CreateNewConfig();
            }

            GlobalOffset.y = Settings["General"]["HeightOffset"].FloatValue;
        }

        public void CreateNewConfig()
        {
            Settings = new Configuration();

            Settings["General"]["HeightOffset"].FloatValue = 0;
            Settings["General"]["HitEffects"].IntValue = 1;

            Settings["Audio"]["MasterVolume"].FloatValue = .7f;
            Settings["Audio"]["MusicVolume"].FloatValue = .7f;
            Settings["Audio"]["EffectsVolume"].FloatValue = .7f;

            Settings["Other"]["SongFolderPath"].StringValue = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Beat Saber Songs\";

            SaveConfig();
        }

        public void UpdateConfigSetting(string section, string name, object value)
        {
            Settings[section][name].SetValue(value);
            ApplyChanges();
        }

        public void ApplyChanges()
        {
            OnConfigChanged?.Invoke();
            SaveConfig();
        }

        public void LoadConfig()
        {
            Settings = Configuration.LoadFromFile(Application.persistentDataPath + configFile);
            HasLoadedSettings = true;
            OnConfigLoaded?.Invoke();
        }

        public void SaveConfig()
        {
            Settings.SaveToFile(Application.persistentDataPath + configFile);
            OnConfigSaved?.Invoke();
        }
    }
}