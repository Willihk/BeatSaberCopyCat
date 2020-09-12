using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeatGame.Data.Saber;
using BeatGame.UI.Components.Tabs;
using System;
using BeatGame.Logic.Managers;
using System.IO;
using System.Linq;
using MessagePack;
using CustomSaber;

namespace BeatGame.UI.Controllers
{
    public class SaberBrowser : MonoBehaviour
    {
        List<CustomSaberInfo> saberInfos = new List<CustomSaberInfo>();

        [SerializeField]
        GameObject entryPrefab;
        [SerializeField]
        Transform entryHolder;
        [SerializeField]
        TabGroup tabGroup;

        string customSaberFolderPath;

        void OnEnable()
        {
            GameManager.Instance.ActivateSabers(false);
        }

        void OnDisable()
        {
            GameManager.Instance.ActivatePointer();
        }

        private void Start()
        {
            customSaberFolderPath = SettingsManager.Instance.Settings["Other"]["SaberFolderPath"].StringValue;

            EnsureSongFolderExists();
            LoadSabers();
            DisplaySabers();
        }

        public void SaberSelected(int index)
        {
            SaberManager.Instance.SetNewActiveSaber(index);
        }

        void EnsureSongFolderExists()
        {
            if (!Directory.Exists(customSaberFolderPath))
            {
                Directory.CreateDirectory(customSaberFolderPath);
            }
        }

        void DisplaySabers()
        {
            for (int i = 0; i < saberInfos.Count; i++)
            {
                var entryObject = Instantiate(entryPrefab, entryHolder);
                entryObject.GetComponent<SaberEntryController>().Initizalize(saberInfos[i], i);
                entryObject.GetComponent<Components.Tabs.TabButton>().SetTabGroup(tabGroup);
            }
        }

        void LoadSabers()
        {
            if (!Directory.Exists(customSaberFolderPath))
                return;

            string[] allBundlePaths = Directory.GetFiles(customSaberFolderPath, "*.saber");

            for (int i = 0; i < allBundlePaths.Length; i++)
            {
                LoadBundle(allBundlePaths[i]);
            }
        }

        public void LoadBundle(string bundlePath)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);

            if (bundle == null)
                return;

            var prefab = bundle.LoadAsset<GameObject>("_customsaber");
            SaberDescriptor customSaber = prefab.GetComponent<SaberDescriptor>();

            SaberManager.Instance.SetupSaber(prefab);

            bundle.Unload(false);

            saberInfos.Add(new CustomSaberInfo
            {
                Path = bundlePath,
                SaberDescriptor = customSaber
            });
        }
    }
}