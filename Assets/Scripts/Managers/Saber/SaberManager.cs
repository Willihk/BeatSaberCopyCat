using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CustomSaber;

namespace BeatGame.Logic.Managers
{
    public class SaberManager : MonoBehaviour
    {
        public List<GameObject> SaberObjects = new List<GameObject>();

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

        }

        public void SetNewActiveSaber(int saberIndex)
        {
            if (saberIndex >= SaberObjects.Count)
                return;

            foreach (Transform child in leftSaberHolder.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in rightSaberHolder.transform)
            {
                Destroy(child.gameObject);
            }

            var newLeftSaber = Instantiate(SaberObjects[saberIndex].transform.Find("LeftSaber"), leftSaberHolder.transform);
            var newRightSaber = Instantiate(SaberObjects[saberIndex].transform.Find("RightSaber"), rightSaberHolder.transform);

            newLeftSaber.transform.localPosition = Vector3.zero;
            newRightSaber.transform.localPosition = Vector3.zero;

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

            var color = material.color;
            var intensitty = material.GetFloat("_Glow");

            var newMaterial = new Material(LeftSaberMaterial);
            newMaterial.color = color;
            newMaterial.SetColor("_EmissionColor", color * (intensitty));

            return newMaterial;
        }

        public void SetupSaber(GameObject prefab)
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
            SaberObjects.Add(saber);
        }
    }
}