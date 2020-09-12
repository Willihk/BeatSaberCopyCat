using UnityEngine;
using System.Collections;
using BeatGame.Logic.Managers;
using System.IO;
using System.Collections.Generic;
using CustomSaber;

namespace BeatGame.Logic.Saber
{
    public class SaberLoader : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            string customPlatformsFolderPath = SettingsManager.Instance.Settings["Other"]["SaberFolderPath"].StringValue;

            // Create the CustomPlatforms folder if it doesn't already exist
            if (!Directory.Exists(customPlatformsFolderPath))
            {
                Directory.CreateDirectory(customPlatformsFolderPath);
            }

            // Find AssetBundles in our CustomSabers directory
            string[] allBundlePaths = Directory.GetFiles(customPlatformsFolderPath, "*.saber");


            // Populate the array
            for (int i = 0; i < allBundlePaths.Length; i++)
            {
                LoadBundle(allBundlePaths[i], transform);
            }

        }

        public void LoadBundle(string bundlePath, Transform parent)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle == null)
            {
                return;
            }
            var prefab = bundle.LoadAsset<GameObject>("_customsaber");

            var saber = Instantiate(prefab, parent);

            if (saber.GetComponentInChildren<CustomTrail>(true) != null)
            {
                CustomTrail tlm;
                foreach (Transform child in saber.transform)
                {
                    if (child.GetComponent<CustomTrail>())
                    {
                        tlm = child.GetComponent<CustomTrail>();

                        GameObject trail = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        trail.transform.SetParent(child);
                        Destroy(trail.GetComponent<BoxCollider>());
                        trail.transform.position = tlm.PointStart.position;
                        trail.transform.localEulerAngles = new Vector3(90, 0, 0);
                        TrailHandler newTrail = trail.AddComponent<TrailHandler>();
                        newTrail.pointEnd = tlm.PointEnd.gameObject;
                        newTrail.pointStart = tlm.PointStart.gameObject;
                        newTrail.mat = tlm.TrailMaterial;
                        newTrail.Onload();
                    }
                }

            }

            foreach (Transform child in saber.transform)
            {
                if (child.name == "RightSaber" || child.name == "LeftSaber")
                {
                    GameObject _Tip = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    _Tip.name = "Tip";
                    _Tip.transform.SetParent(child);
                    _Tip.transform.localScale = new Vector3(0, 0, 0);
                    _Tip.transform.localPosition = new Vector3(0, 0, child.localScale.z);
                    GameObject _base = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    _base.name = "Base";
                    _base.transform.SetParent(child);
                    _base.transform.localScale = new Vector3(0, 0, 0);
                    _base.transform.localPosition = new Vector3(0, 0, 0);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}