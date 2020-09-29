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
        int laserGroupSize = 1;
        [SerializeField]
        protected float laserIntensity = 3;
        [SerializeField]
        protected float laserFlashIntensity = 6;

        Material[] materials;
        Coroutine[] fadeRoutines;
        Coroutine[] FlashRoutines;

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
            fadeRoutines = new Coroutine[controllers.Count];
            FlashRoutines = new Coroutine[controllers.Count];

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
                if (item.TryGetComponent(out LaserControllerBase controller))
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

            if (propID * laserGroupSize + laserGroupSize < materials.Length)
                return (propID * laserGroupSize, propID * laserGroupSize + laserGroupSize);

            return (propID * laserGroupSize, materials.Length);
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
                        var range = GetRangeByPropID(eventData.PropID);

                        for (int i = range.startIndex; i < range.endIndex; i++)
                        {
                            materials[i].SetColor("_Color", color);
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
            var range = GetRangeByPropID(propID);

            for (int i = range.startIndex; i < range.endIndex; i++)
            {
                materials[i].SetFloat("_FadeAmount", 0);
            }
        }

        public virtual void TurnOn(int propID)
        {
            var range = GetRangeByPropID(propID);

            for (int i = range.startIndex; i < range.endIndex; i++)
            {
                materials[i].SetFloat("_FadeAmount", 1);
            }
        }

        public virtual void Flash(int propID)
        {
            var range = GetRangeByPropID(propID);

            TurnOn(propID);

            for (int i = range.startIndex; i < range.endIndex; i++)
            {
                if (FlashRoutines[i] != null)
                    StopCoroutine(FlashRoutines[i]);

                if (fadeRoutines[i] != null)
                    StopCoroutine(fadeRoutines[i]);


                FlashRoutines[i] = StartCoroutine(FlashLaser(i));
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

                if (FlashRoutines[i] != null)
                    StopCoroutine(FlashRoutines[i]);

                fadeRoutines[i] = StartCoroutine(FadeLaser(i));
            }
        }

        protected IEnumerator FlashLaser(int index)
        {
            float intensity = laserIntensity;
            while (intensity < laserFlashIntensity)
            {
                intensity += (laserFlashIntensity - laserIntensity) / .1f * Time.deltaTime;
                materials[index].SetFloat("_EmissionIntensity", intensity);

                yield return null;
            }

            while (intensity > laserIntensity)
            {
                intensity -= (laserFlashIntensity - laserIntensity) / .2f * Time.deltaTime;
                materials[index].SetFloat("_EmissionIntensity", intensity);

                yield return null;
            }
        }

        protected IEnumerator FadeLaser(int index)
        {
            float fadeAmount = .7f;
            while (fadeAmount > 0)
            {
                fadeAmount -= .7f / .6f * Time.deltaTime;

                materials[index].SetFloat("_FadeAmount", fadeAmount);

                yield return null;
            }
        }
    }
}