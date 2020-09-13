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

            CustomTrail tlm;
            if (newLeftSaber.GetComponent<CustomTrail>())
            {
                tlm = newLeftSaber.GetComponent<CustomTrail>();

                GameObject trail = GameObject.CreatePrimitive(PrimitiveType.Cube);
                trail.transform.SetParent(newLeftSaber);
                Destroy(trail.GetComponent<BoxCollider>());
                trail.transform.position = tlm.PointStart.position;
                trail.transform.localEulerAngles = new Vector3(90, 0, 0);
                TrailHandler newTrail = trail.AddComponent<TrailHandler>();
                newTrail.pointEnd = tlm.PointEnd.gameObject;
                newTrail.pointStart = tlm.PointStart.gameObject;
                newTrail.mat = tlm.TrailMaterial;
                newTrail.startColor = LeftSaberMaterial.color;
                newTrail.endColor = LeftSaberMaterial.color;
                newTrail.endColor.a = 0;
                newTrail.Onload();
            }

            if (newRightSaber.GetComponent<CustomTrail>())
            {
                tlm = newRightSaber.GetComponent<CustomTrail>();

                GameObject trail = GameObject.CreatePrimitive(PrimitiveType.Cube);
                trail.transform.SetParent(newRightSaber);
                Destroy(trail.GetComponent<BoxCollider>());
                trail.transform.position = tlm.PointStart.position;
                trail.transform.localEulerAngles = new Vector3(90, 0, 0);
                TrailHandler newTrail = trail.AddComponent<TrailHandler>();
                newTrail.pointEnd = tlm.PointEnd.gameObject;
                newTrail.pointStart = tlm.PointStart.gameObject;
                newTrail.mat = tlm.TrailMaterial;
                newTrail.startColor = RightSaberMaterial.color;
                newTrail.endColor = RightSaberMaterial.color;
                newTrail.endColor.a = 0;
                newTrail.Onload();
            }

            newLeftSaber.gameObject.SetActive(true);
            newRightSaber.gameObject.SetActive(true);
        }

        void ReplaceGlowMaterialsReqursive(Transform transform, bool leftSaberMaterial)
        {
            if (transform.TryGetComponent(out Renderer renderer))
            {
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    if (renderer.materials[i].name.Contains("Glow_mat"))
                    {
                        Material[] materials = renderer.materials;
                        if (leftSaberMaterial)
                        {
                            materials[i] = LeftSaberMaterial;
                        }
                        else
                        {
                            materials[i] = RightSaberMaterial;
                        }
                        renderer.materials = materials;
                    }
                }
            }

            foreach (Transform child in transform.transform)
            {
                ReplaceGlowMaterialsReqursive(child, leftSaberMaterial);
            }
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