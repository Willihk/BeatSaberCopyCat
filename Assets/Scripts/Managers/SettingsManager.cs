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

        public static float3 LineOffset = new float3(.4f, .4f, 0);
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

            Settings["General"]["FastLoad"].IntValue = 1;
            Settings["General"]["Reflections"].IntValue = 1;
            Settings["General"]["HeightOffset"].FloatValue = 0;
            Settings["General"]["HitEffects"].IntValue = 1;
            Settings["General"]["NoteSlicing"].IntValue = 1;

            Settings["Modifiers"]["NoFail"].IntValue = 0;
            Settings["Modifiers"]["DoubleSaber"].IntValue = 0;
            Settings["Modifiers"]["NoArrows"].IntValue = 0;

            Settings["Audio"]["MasterVolume"].FloatValue = .7f;
            Settings["Audio"]["MusicVolume"].FloatValue = .7f;
            Settings["Audio"]["EffectsVolume"].FloatValue = .7f;

            Settings["Other"]["DataFolderPath"].StringValue = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\BeatSaber\Beat Saber_Data\";
            Settings["Other"]["SaberFolderPath"].StringValue = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\BeatSaber\Beat Saber_Data\CustomSabers\";
            Settings["Other"]["SongFolderPath"].StringValue = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\BeatSaber\Beat Saber_Data\CustomLevels\";

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