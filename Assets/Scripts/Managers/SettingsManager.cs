using MessagePack;
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

        public static float3 LineOffset = new float3(.4f, .5f, 0);
        public static float3 GlobalOffset = new float3(0, 0, 0);

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
                UpdateSettingsIfOutdated(Settings);
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

            PopulateSettingsWithDefault(Settings);

            SaveConfig();
        }

        void PopulateSettingsWithDefault(Configuration settings)
        {
            settings["Version"]["Version"].StringValue = Application.version;

            settings["General"]["FastLoad"].IntValue = 1;
            settings["General"]["HeightOffset"].FloatValue = 0;
            settings["General"]["HitEffects"].IntValue = 1;
            settings["General"]["NoteSlicing"].IntValue = 1;

            settings["Modifiers"]["NoFail"].IntValue = 0;
            settings["Modifiers"]["DoubleSaber"].IntValue = 0;
            settings["Modifiers"]["NoArrows"].IntValue = 0;

            settings["Audio"]["MasterVolume"].FloatValue = .7f;
            settings["Audio"]["MusicVolume"].FloatValue = .7f;
            settings["Audio"]["EffectsVolume"].FloatValue = .7f;

            settings["Other"]["RootFolderPath"].StringValue = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\BeatSaber\";
        }

        void UpdateSettingsIfOutdated(Configuration oldSettings)
        {
            var newSettings = new Configuration();
            PopulateSettingsWithDefault(newSettings);

            if (newSettings["Version"]["Version"].StringValue == oldSettings["Version"]["Version"].StringValue)
                return;


            // Needs a update

            var sectionEnumerator = newSettings.GetEnumerator();
            while (sectionEnumerator.MoveNext())
            {
                var settingEnumerator = sectionEnumerator.Current.GetEnumerator();
                while (settingEnumerator.MoveNext())
                {
                    if (oldSettings.Contains(sectionEnumerator.Current.Name, settingEnumerator.Current.Name))
                    {
                        string value = oldSettings[sectionEnumerator.Current.Name][settingEnumerator.Current.Name].RawValue;
                        if (!string.IsNullOrEmpty(value))
                            newSettings[sectionEnumerator.Current.Name][settingEnumerator.Current.Name].RawValue = value;
                    }
                }
            }

            newSettings["Version"]["Version"].StringValue = Application.version;
            Settings = newSettings;

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