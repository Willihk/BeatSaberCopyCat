using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System.Linq;
using BeatGame.Data;

namespace BeatGame.Logic.Lasers
{
    public class MasterLaserController : MonoBehaviour
    {
        [SerializeField]
        bool findLaserControllersInChildren = false;
        [SerializeField]
        List<LaserControllerBase> controllers;

        [SerializeField]
        SongEventType[] supportedEventTypes;

        [SerializeField]
        protected float laserIntensity = 3;
        [SerializeField]
        protected float laserFlashIntensity = 6;

        Material material;

        private void OnEnable()
        {
            if (controllers == null)
                controllers = new List<LaserControllerBase>();


            material = new Material(Resources.Load<Material>("Materials/Map/Lasers/LaserMaterial"));

            material.SetFloat("_EmissionIntensity", laserIntensity);

            EventPlayingSystem.Instance.OnPlayEvent += PlayEvent;


            if (findLaserControllersInChildren)
                GetControllersInChildReqursive(transform);

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
                if (item.TryGetComponent(out LaserControllerBase controller))
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
                        Color color = new Color(eventData.Color.x, eventData.Color.y, eventData.Color.z, eventData.Color.w);
                        material.SetColor("_Color", color);

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
                    case 12:
                    case 13:
                        for (int i = 0; i < controllers.Count; i++)
                        {
                            if (i % 2 == 0)
                                controllers[i].SetRotation(eventData.Value);
                            else
                                controllers[i].SetRotation(-eventData.Value);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public virtual void TurnOff()
        {
            material.SetFloat("_FadeAmount", 0);
            controllers.ForEach(x => x.TurnOff());
        }

        public virtual void TurnOn()
        {
            material.SetFloat("_FadeAmount", 1);
        }

        public virtual void Flash()
        {
            TurnOn();
            StopAllCoroutines();
            StartCoroutine(FlashMaterial());
        }

        public virtual void Fade()
        {
            TurnOn();
            StopAllCoroutines();
            StartCoroutine(FadeLaser());
        }

        protected IEnumerator FlashMaterial()
        {
            float intensity = laserIntensity;
            while (intensity < laserFlashIntensity)
            {
                intensity += (laserFlashIntensity - laserIntensity) / .1f * Time.deltaTime;
                material.SetFloat("_EmissionIntensity", intensity);

                yield return null;
            }

            while (intensity > laserIntensity)
            {
                intensity -= (laserFlashIntensity - laserIntensity) / .2f * Time.deltaTime;
                material.SetFloat("_EmissionIntensity", intensity);

                yield return null;
            }

        }

        protected IEnumerator FadeLaser()
        {
            float fadeAmount = .7f;
            while (fadeAmount > 0)
            {
                fadeAmount -= .7f / .6f * Time.deltaTime;

                material.SetFloat("_FadeAmount", fadeAmount);
                yield return null;
            }

            TurnOff();
            material.SetFloat("_FadeAmount", fadeAmount);
        }
    }
}