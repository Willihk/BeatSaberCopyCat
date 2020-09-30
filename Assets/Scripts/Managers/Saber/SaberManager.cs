using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CustomSaber;
using BeatGame.Data.Saber;
using System.Linq;
using System.IO;

namespace BeatGame.Logic.Managers
{
    public class SaberManager : MonoBehaviour
    {
        public List<CustomSaberInfo> LoadedSabers = new List<CustomSaberInfo>();

        [SerializeField]
        GameObject defaultSabers;
        [SerializeField]
        GameObject leftSaberHolder;
        [SerializeField]
        GameObject rightSaberHolder;
        [SerializeField]
        Shader litShader;
        [SerializeField]
        Shader trailShader;

        public Material LeftSaberMaterial;
        public Material RightSaberMaterial;

        public static SaberManager Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            LoadedSabers.Add(new CustomSaberInfo() { SaberObject = defaultSabers });
            EnsureFolderExists();
            StartCoroutine(LoadSabersRoutine());
        }

        void EnsureFolderExists()
        {
            if (!Directory.Exists(SettingsManager.Instance.Settings["Other"]["RootFolderPath"].StringValue + "CustomSabers"))
            {
                Directory.CreateDirectory(SettingsManager.Instance.Settings["Other"]["RootFolderPath"].StringValue + "CustomSabers");
            }
        }

        public void SetNewActiveSaber(int saberIndex)
        {
            if (saberIndex >= LoadedSabers.Count)
                return;

            foreach (Transform child in leftSaberHolder.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in rightSaberHolder.transform)
            {
                Destroy(child.gameObject);
            }

            var newLeftSaber = Instantiate(LoadedSabers[saberIndex].SaberObject.transform.Find("LeftSaber"), leftSaberHolder.transform);
            var newRightSaber = Instantiate(LoadedSabers[saberIndex].SaberObject.transform.Find("RightSaber"), rightSaberHolder.transform);

            CreateTrail(newLeftSaber.gameObject);
            CreateTrail(newRightSaber.gameObject, false);

            newLeftSaber.gameObject.SetActive(true);
            newRightSaber.gameObject.SetActive(true);
        }

        void CreateTrail(GameObject saber, bool leftSaber = true)
        {
            CustomTrail tlm;
            if (saber.GetComponent<CustomTrail>())
            {
                tlm = saber.GetComponent<CustomTrail>();

                GameObject trail = GameObject.CreatePrimitive(PrimitiveType.Cube);
                trail.transform.SetParent(saber.transform);
                Destroy(trail.GetComponent<BoxCollider>());
                trail.transform.position = tlm.PointStart.position;
                trail.transform.localEulerAngles = new Vector3(90, 0, 0);
                TrailHandler newTrail = trail.AddComponent<TrailHandler>();
                newTrail.pointEnd = tlm.PointEnd.gameObject;
                newTrail.pointStart = tlm.PointStart.gameObject;

                newTrail.mat = new Material(trailShader);
                if (tlm.TrailMaterial.HasProperty("_AlphaTex"))
                    newTrail.mat.SetTexture("_AlphaTex", tlm.TrailMaterial.GetTexture("_AlphaTex"));
                else
                    newTrail.mat.SetTexture("_AlphaTex", tlm.TrailMaterial.GetTexture("_MainTex"));

                newTrail.mat.SetColor("_Color", leftSaber ? LeftSaberMaterial.color : RightSaberMaterial.color);

                if (leftSaber)
                {
                    newTrail.startColor = LeftSaberMaterial.color;
                    newTrail.endColor = LeftSaberMaterial.color;
                }
                else
                {
                    newTrail.startColor = RightSaberMaterial.color;
                    newTrail.endColor = RightSaberMaterial.color;
                }

                newTrail.endColor.a = 0;
                newTrail.Onload();
            }

        }

        void ReplaceGlowMaterialsReqursive(Transform transform, bool leftSaberMaterial)
        {
            if (transform.TryGetComponent(out Renderer renderer))
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    Material[] materials = renderer.materials;
                    if (renderer.materials[i].name.Contains("Glow_mat"))
                    {
                        if (leftSaberMaterial)
                        {
                            materials[i] = LeftSaberMaterial;
                        }
                        else
                        {
                            materials[i] = RightSaberMaterial;
                        }
                    }
                    else
                    {
                        materials[i] = ConvertMaterial(materials[i]);
                    }
                    renderer.materials = materials;
                }
            }

            foreach (Transform child in transform.transform)
            {
                ReplaceGlowMaterialsReqursive(child, leftSaberMaterial);
            }
        }

        Material ConvertMaterial(Material material)
        {
            Color color = Color.white;
            float intensity = 1;
            if (material.HasProperty("_Color"))
                color = material.color;

            if (material.HasProperty("_Glow"))
                intensity = material.GetFloat("_Glow");

            var newMaterial = new Material(LeftSaberMaterial);
            newMaterial.color = color;
            newMaterial.SetColor("_EmissionColor", color * (intensity));

            return newMaterial;
        }

        public void SetupSaber(GameObject prefab, CustomSaberInfo saberInfo)
        {
            var saber = Instantiate(prefab, transform);

            ReplaceGlowMaterialsReqursive(saber.transform.Find("LeftSaber"), true);
            ReplaceGlowMaterialsReqursive(saber.transform.Find("RightSaber"), false);

            foreach (Transform child in saber.transform)
            {
                if (child.name == "RightSaber" || child.name == "LeftSaber")
                {
                    GameObject _Tip = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(_Tip.GetComponent<Collider>());
                    _Tip.name = "Tip";
                    _Tip.transform.SetParent(child);
                    _Tip.transform.localScale = new Vector3(0, 0, 0);
                    _Tip.transform.localPosition = new Vector3(0, 0, child.localScale.z);
                    GameObject _base = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Destroy(_base.GetComponent<Collider>());
                    _base.name = "Base";
                    _base.transform.SetParent(child);
                    _base.transform.localScale = new Vector3(0, 0, 0);
                    _base.transform.localPosition = new Vector3(0, 0, 0);
                }
            }

            saber.SetActive(false);
            saberInfo.SaberObject = saber;
            LoadedSabers.Add(saberInfo);
        }

        public bool IsPathAlreadyLoaded(string path)
        {
            if (LoadedSabers.Any(x => x.Path == path))
                return true;

            return false;
        }


        IEnumerator LoadSabersRoutine()
        {
            string[] allBundlePaths = Directory.GetFiles(SettingsManager.Instance.Settings["Other"]["RootFolderPath"].StringValue + "CustomSabers", "*.saber");

            for (int i = 0; i < allBundlePaths.Length; i++)
            {
                string bundlePath = allBundlePaths[i];
                if (IsPathAlreadyLoaded(bundlePath))
                {
                    continue;
                }
                var bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
                yield return bundleRequest;

                var bundle = bundleRequest.assetBundle;
                if (bundle == null)
                    continue;

                var prefabRequest = bundle.LoadAssetAsync<GameObject>("_customsaber");
                yield return prefabRequest;
                GameObject prefab = (GameObject)prefabRequest.asset;

                SaberDescriptor customSaber = prefab.GetComponent<SaberDescriptor>();

                var saberInfo = new CustomSaberInfo
                {
                    Path = bundlePath,
                    SaberDescriptor = customSaber,
                };

                SetupSaber(prefab, saberInfo);

                bundle.Unload(false);

                yield return null;
            }
        }
    }
}