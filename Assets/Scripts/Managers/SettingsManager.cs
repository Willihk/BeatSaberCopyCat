﻿using SharpConfig;
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
        public static float3 GlobalOffset = new float3(0, 0, 0);

        public event Action OnConfigLoaded;
        public event Action OnConfigChanged;
        public event Action OnConfigSaved;

        public Configuration Settings;
        public bool HasLoadedSettings;

        string configFile = "/Settings.cfg";

        private void Awake()
        {
            Debug.Log("Application Version : " + Application.version);
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
            Settings["General"]["NoFail"].IntValue = 0;

            Settings["Audio"]["MasterVolume"].FloatValue = .7f;
            Settings["Audio"]["MusicVolume"].FloatValue = .7f;
            Settings["Audio"]["EffectsVolume"].FloatValue = .7f;

            Settings["Other"]["RootFolderPath"].StringValue = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\BeatSaber\";
            Settings["Other"]["SongFolderPath"].StringValue = $@"{Settings["Other"]["RootFolderPath"]}BeatSaber\Beat Saber_Data\CustomLevels\";

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
            Settings = Configuration.LoadFromFile(Settings["Other"]["RootFolderPath"] + configFile);
            HasLoadedSettings = true;
            OnConfigLoaded?.Invoke();
        }

        public void SaveConfig()
        {
            Settings.SaveToFile(Settings["Other"]["RootFolderPath"] + configFile);
            OnConfigSaved?.Invoke();
        }
    }
}