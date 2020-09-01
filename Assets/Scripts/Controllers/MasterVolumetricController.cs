using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.Entities;
using System.Collections.Generic;
using BeatGame.Data;
using BeatGame.Data.Map.Modified;

namespace BeatGame.Logic.Volumetrics
{
    public class MasterVolumetricController : MonoBehaviour
    {
        [SerializeField]
        bool findLaserControllersInChildren = false;
        [SerializeField]
        List<VolumetricControllerBase> controllers;

        [SerializeField]
        SongEventType[] supportedEventTypes;

        [SerializeField]
        Material material;

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

            controllers.ForEach(x => x.SetMaterial(material));

            TurnOff();
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
                        currentColor.r = eventData.Color.x;
                        currentColor.g = eventData.Color.y;
                        currentColor.b = eventData.Color.z;
                        currentColor.a = maxAlpha;
                        material.SetColor("_Color", currentColor);

                        // Easier value switch
                        if (eventData.Value > 4)
                            eventData.Value -= 4;

                        switch (eventData.Value)
                        {
                            case 0:
                                TurnOff();
                                break;
                            case 1:
                                TurnOn();
                                break;
                            case 2:
                                Flash();
                                break;
                            case 3:
                                Fade();
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

        public virtual void TurnOff()
        {
            currentColor.a = 0;
            material.SetColor("_Color", currentColor);
        }

        public virtual void TurnOn()
        {
            currentColor.a = maxAlpha;
            material.SetColor("_Color", currentColor);
        }

        public virtual void Flash()
        {
            StopAllCoroutines();
            TurnOn();
        }

        public virtual void Fade()
        {
            StopAllCoroutines();
            TurnOn();
            StartCoroutine(FadeVolume());
        }

        protected IEnumerator FadeVolume()
        {
            currentColor.a = maxAlpha;
            while (currentColor.a > 0)
            {
                currentColor.a -= maxAlpha / .55f * Time.deltaTime;
                material.SetColor("_Color", currentColor);
                yield return null;
            }

            TurnOff();
        }
    }
}