using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using BeatGame.Data;

namespace BeatGame.Logic.Rings
{
    public class MasterRingController : MonoBehaviour
    {
        [SerializeField]
        int ringCount = 25;
        [SerializeField]
        Vector3 ringGap = new Vector3(0, 0, 10);

        [SerializeField]
        GameObject ringObject;
        [SerializeField]
        List<GameObject> rings;

        [SerializeField]
        SongEventType[] supportedEventTypes;

        [SerializeField]
        float ringSpeedMultiplier = 4;

        float ringSpeed;
        float currentZoomLevel;
        // 0 Default
        // 1 zoom in
        // 2 zoom out

        private void Awake()
        {
            if (rings == null)
                rings = new List<GameObject>();

            CreateRings();
        }

        private void Start()
        {
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EventPlayingSystem>().OnPlayEvent += PlayEvent;
        }

        void CreateRings()
        {
            for (int i = rings.Count; i < ringCount; i++)
            {
                var newRing = Instantiate(ringObject, transform);
                newRing.transform.position += ringGap * i;

                rings.Add(newRing);
            }
        }

        void ZoomRings()
        {
            if (currentZoomLevel == 0)
                return;

            Vector3 endPosition;
            for (int i = 0; i < ringCount; i++)
            {
                if (currentZoomLevel == 1)
                    endPosition = ringGap * (i + 1) * .6f;
                else
                    endPosition = ringGap * (i + 1) * 1.6f;

                endPosition = math.lerp(rings[i].transform.localPosition, endPosition, 2 * Time.deltaTime);

                rings[i].transform.localPosition = endPosition;
            }
        }

        private void Update()
        {
            RotateRingsIntoPosition();
            ZoomRings();
        }

        void RotateRingsIntoPosition()
        {
            if (ringSpeed < 0)
                ringSpeed += math.abs(ringSpeed) / .5f * Time.deltaTime;
            if (ringSpeed > 0)
                ringSpeed -= ringSpeed / .5f * Time.deltaTime;

            for (int i = 0; i < rings.Count; i++)
            {
                rings[i].transform.Rotate(new Vector3(0, 0, ringSpeed * ringSpeedMultiplier * (i + 1) * Time.deltaTime), Space.Self);
            }
        }

        void NewRotation()
        {
            ringSpeed = Random.Range(-10, 10);
        }

        private void PlayEvent(int type, int value)
        {
            if (supportedEventTypes.Any(x => (int)x == type))
            {
                switch (type)
                {
                    case 8:
                        NewRotation();
                        break;
                    case 9:
                        if (currentZoomLevel == 0)
                            currentZoomLevel = 2;
                        else if (currentZoomLevel == 1)
                            currentZoomLevel = 2;
                        else if (currentZoomLevel == 2)
                            currentZoomLevel = 1;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}