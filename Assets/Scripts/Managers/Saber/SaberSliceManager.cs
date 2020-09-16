using UnityEngine;
using System.Collections;
using BeatGame.Data.Map.Modified;
using EzySlice;

namespace BeatGame.Logic.Managers
{
    public class SaberSliceManager : MonoBehaviour
    {
        public static SaberSliceManager Instance;

        public GameObject baseNote;

        public Material LeftSaberMaterial;
        public Material RightSaberMaterial;


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void Slice(Transform noteTransform, Vector3 direction, int noteType)
        {
            baseNote.transform.position = noteTransform.position;
            baseNote.transform.rotation = noteTransform.rotation;

            Material material = noteType == 1 ? RightSaberMaterial : LeftSaberMaterial;

            SlicedHull hull = baseNote.Slice(baseNote.gameObject.transform.position, direction, material);

            if (hull != null)
            {
                GameObject hullObject = hull.CreateLowerHull(baseNote.gameObject, LeftSaberMaterial);
                var rigidbody = hullObject.AddComponent<Rigidbody>();
                rigidbody.AddExplosionForce(2, noteTransform.position, .5f);
                Destroy(hullObject, 10);

                hullObject = hull.CreateUpperHull(baseNote.gameObject, LeftSaberMaterial);
                rigidbody = hullObject.AddComponent<Rigidbody>();
                rigidbody.AddExplosionForce(2, noteTransform.position, .5f);
                Destroy(hullObject, 10);
            }
        }
    }
}