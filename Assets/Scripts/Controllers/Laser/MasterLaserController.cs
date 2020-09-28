using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BeatGame.Data.Map.Modified;
using BeatGame.Data.Map;

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
        float laserGroupSize = 1;
        [SerializeField]
        protected float laserIntensity = 3;
        [SerializeField]
        protected float laserFlashIntensity = 6;


        Material[] materials;

        private void OnEnable()
        {
            if (controllers == null)
                controllers = new List<LaserControllerBase>();

            var material = new Material(Resources.Load<Material>("Materials/Map/Lasers/LaserMaterial"));

            material.SetFloat("_EmissionIntensity", laserIntensity);

            EventPlayingSystem.Instance.OnPlayEvent += PlayEvent;


            if (findLaserControllersInChildren)
                GetControllersInChildReqursive(transform);

            materials = new Material[controllers.Count];

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = new Material(material);

                controllers[i].SetMaterial(materials[i]);
            }

            TurnOff(0);
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
                        for (int i = 0; i < laserGroupSize; i++)
                        {
                            if (eventData.PropID + i < materials.Length)
                                materials[eventData.PropID + i].SetColor("_Color", color);
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

        public virtual void TurnOff(int propID)
        {
            for (int i = 0; i < laserGroupSize; i++)
            {
                if (propID + i < materials.Length)
                    materials[propID + i].SetFloat("_FadeAmount", 0);
            }
        }

        public virtual void TurnOn(int propID)
        {
            for (int i = 0; i < laserGroupSize; i++)
            {
                if (propID + i < materials.Length)
                    materials[propID + i].SetFloat("_FadeAmount", 1);
            }
        }

        public virtual void Flash(int propID)
        {
            TurnOn(propID);
            StopAllCoroutines();
            StartCoroutine(FlashMaterial());
        }

        public virtual void Fade(int propID)
        {
            TurnOn(propID);
            StopAllCoroutines();
            StartCoroutine(FadeLaser());
        }

        protected IEnumerator FlashMaterial()
        {
            float intensity = laserIntensity;
            while (intensity < laserFlashIntensity)
            {
                intensity += (laserFlashIntensity - laserIntensity) / .1f * Time.deltaTime;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat("_EmissionIntensity", intensity);
                }

                yield return null;
            }

            while (intensity > laserIntensity)
            {
                intensity -= (laserFlashIntensity - laserIntensity) / .2f * Time.deltaTime;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat("_EmissionIntensity", intensity);
                }

                yield return null;
            }

        }

        protected IEnumerator FadeLaser()
        {
            float fadeAmount = .7f;
            while (fadeAmount > 0)
            {
                fadeAmount -= .7f / .6f * Time.deltaTime;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat("_FadeAmount", fadeAmount);
                }

                yield return null;
            }

            TurnOff(0);
        }
    }
}