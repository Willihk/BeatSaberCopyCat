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

            SlicedHull hull = baseNote.Slice(baseNote.gameObject.transform.position, direction, LeftSaberMaterial);

            if (hull != null)
            {
                hull.CreateLowerHull(baseNote.gameObject, LeftSaberMaterial);
                hull.CreateUpperHull(baseNote.gameObject, LeftSaberMaterial);
            }
        }
    }
}