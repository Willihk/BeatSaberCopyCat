using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.Entities;
using System.Collections.Generic;
using BeatGame.Data;
using BeatGame.Data.Map.Modified;
using BeatGame.Data.Map;

namespace BeatGame.Logic.Volumetrics
{
    public class MasterVolumetricController : MonoBehaviour
    {
        [SerializeField]
        bool findLaserControllersInChildren = false;
        [SerializeField]
        int groupSize = 1;

        [SerializeField]
        List<VolumetricControllerBase> controllers;
        [SerializeField]
        SongEventType[] supportedEventTypes;

        [SerializeField]
        Material material;

        Material[] materials;
        Coroutine[] fadeRoutines;
        Coroutine[] FlashRoutines;
        Color[] currentColors;

        Color currentColor;
        float maxAlpha = 0.77f;

        private void OnEnable()
        {
            material = new Material(material);

            if (controllers == null)
                controllers = new List<VolumetricControllerBase>();

            if (findLaserControllersInChildren)
                GetControllersInChildReqursive(transform);

            EventPlayingSystem.Instance.OnPlayEvent += PlayEvent;


            materials = new Material[controllers.Count];
            fadeRoutines = new Coroutine[controllers.Count];
            FlashRoutines = new Coroutine[controllers.Count];

            currentColors = new Color[controllers.Count];

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = new Material(material);

                controllers[i].SetMaterial(materials[i]);
            }

            TurnOff(-1);
        }

        private void OnDisable()
        {
            if (EventPlayingSystem.Instance != null)
            {
                EventPlayingSystem.Instance.OnPlayEvent -= PlayEvent;
            }
            controllers = null;
        }

        void GetControllersInChildReqursive(Transform child)
        {
            foreach (Transform item in child)
            {
                if (item.TryGetComponent(out VolumetricControllerBase controller))
                {
                    controllers.Add(controller);
                }
                GetControllersInChildReqursive(item);
            }
        }

        private (int startIndex, int endIndex) GetRangeByPropID(int propID)
        {
            if (propID == -1)
                return (0, materials.Length);

            if (propID * groupSize + groupSize < materials.Length)
                return (propID * groupSize, propID * groupSize + groupSize);

            return (propID * groupSize, materials.Length);
        }

        private void PlayEvent(int type, EventData eventData)
        {
            if (supportedEventTypes.Any(x => (int)x == type))
            {
                switch (type)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        var range = GetRangeByPropID(eventData.PropID);

                        for (int i = range.startIndex; i < range.endIndex; i++)
                        {
                            currentColors[i].r = eventData.Color.x;
                            currentColors[i].g = eventData.Color.y;
                            currentColors[i].b = eventData.Color.z;
                            currentColors[i].a = maxAlpha;
                            materials[i].SetColor("_Color", currentColors[i]);
                        }

                        // Easier value switch
                        if (eventData.Value > 4)
                            eventData.Value -= 4;

                        switch (eventData.Value)
                        {
                            case 0:
                                TurnOff(eventData.PropID);
                                break;
                            case 1:
                                TurnOn(eventData.PropID);
                                break;
                            case 2:
                                Flash(eventData.PropID);
                                break;
                            case 3:
                                Fade(eventData.PropID);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public virtual void TurnOff(int propID)
        {
            var range = GetRangeByPropID(propID);

            for (int i = range.startIndex; i < range.endIndex; i++)
            {
                currentColors[i].a = 0;
                materials[i].SetColor("_Color", currentColors[i]);
            }
        }

        public virtual void TurnOn(int propID)
        {
            var range = GetRangeByPropID(propID);

            for (int i = range.startIndex; i < range.endIndex; i++)
            {
                currentColors[i].a = maxAlpha;
                materials[i].SetColor("_Color", currentColors[i]);
            }
        }

        public virtual void Flash(int propID)
        {
            var range = GetRangeByPropID(propID);

            TurnOn(propID);

            for (int i = range.startIndex; i < range.endIndex; i++)
            {
                if (fadeRoutines[i] != null)
                    StopCoroutine(fadeRoutines[i]);
            }
        }

        public virtual void Fade(int propID)
        {
            var range = GetRangeByPropID(propID);

            TurnOn(propID);

            for (int i = range.startIndex; i < range.endIndex; i++)
            {
                if (fadeRoutines[i] != null)
                    StopCoroutine(fadeRoutines[i]);

                fadeRoutines[i] = StartCoroutine(FadeVolume(i));
            }
        }

        protected IEnumerator FadeVolume(int index)
        {
            currentColors[index].a = maxAlpha;
            while (currentColors[index].a > 0)
            {
                currentColors[index].a -= maxAlpha / .55f * Time.deltaTime;
                materials[index].SetColor("_Color", currentColors[index]);
                yield return null;
            }
        }
    }
}