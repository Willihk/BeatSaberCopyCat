﻿using UnityEngine;
using System.Collections;
using BeatGame.Data.Map.Modified;
using EzySlice;

namespace BeatGame.Logic.Managers
{
    public class SaberSliceManager : MonoBehaviour
    {
        public static SaberSliceManager Instance;

        public GameObject baseNote;

        public float forceMultiplier = 100;

        public Material LeftCutMaterial;
        public Material RightCutMaterial;
        public Material LeftMatMaterial;
        public Material RightMatMaterial;


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void Slice(Transform noteTransform, Vector3 direction, int noteType)
        {
            baseNote.transform.position = noteTransform.position;
            baseNote.transform.rotation = noteTransform.rotation;

            baseNote.GetComponent<Renderer>().material = noteType == 1 ? RightMatMaterial : LeftMatMaterial;

            Material material = noteType == 1 ? RightCutMaterial : LeftCutMaterial;

            SlicedHull hull = baseNote.Slice(baseNote.gameObject.transform.position, direction, material);

            if (hull != null)
            {
                GameObject hullObject = hull.CreateLowerHull(baseNote.gameObject, material);
                var rigidbody = hullObject.AddComponent<Rigidbody>();
                rigidbody.AddExplosionForce(5 * forceMultiplier, noteTransform.position, .5f);
                Destroy(hullObject, 10);

                hullObject = hull.CreateUpperHull(baseNote.gameObject, material);
                rigidbody = hullObject.AddComponent<Rigidbody>();
                rigidbody.AddExplosionForce(5 * forceMultiplier, noteTransform.position, .5f);
                Destroy(hullObject, 10);
            }
        }
    }
}