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
            if (saberInfos == null || saberInfos.Count == 0)
            {
                customSaberFolderPath = SettingsManager.Instance.Settings["Other"]["SaberFolderPath"].StringValue;
                EnsureFolderExists();
                StartCoroutine(LoadSabersRoutine());
            }

            GameManager.Instance.ActivateSabers(false);
        }

        void OnDisable()
        {
            GameManager.Instance.ActivatePointer();
        }

        public void SaberSelected(int index)
        {
            SaberManager.Instance.SetNewActiveSaber(index);
        }

        void EnsureFolderExists()
        {
            if (!Directory.Exists(customSaberFolderPath))
            {
                Directory.CreateDirectory(customSaberFolderPath);
            }
        }

        IEnumerator LoadSabersRoutine()
        {
            string[] allBundlePaths = Directory.GetFiles(customSaberFolderPath, "*.saber");

            for (int i = 0; i < allBundlePaths.Length; i++)
            {
                LoadBundle(allBundlePaths[i]);

                var entryObject = Instantiate(entryPrefab, entryHolder);
                entryObject.GetComponent<SaberEntryController>().Initizalize(saberInfos[i], i);
                entryObject.GetComponent<Components.Tabs.TabButton>().SetTabGroup(tabGroup);

                yield return new WaitForSeconds(.2f);
            }
        }

        public void LoadBundle(string bundlePath)
        {
            if (SaberManager.Instance.IsPathAlreadyLoaded(bundlePath))
            {
                saberInfos.Add(SaberManager.Instance.LoadedSabers.Find(x => x.Path == bundlePath));
                return;
            }

            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);

            if (bundle == null)
                return;

            var prefab = bundle.LoadAsset<GameObject>("_customsaber");
            SaberDescriptor customSaber = prefab.GetComponent<SaberDescriptor>();

            var saberInfo = new CustomSaberInfo
            {
                Path = bundlePath,
                SaberDescriptor = customSaber,
            };

            SaberManager.Instance.SetupSaber(prefab, saberInfo);

            bundle.Unload(false);
            saberInfos.Add(saberInfo);
        }
    }
}