using UnityEngine;
using System.Collections;
using BeatGame.Data.Map.Modified;
using System.Linq;
using BeatGame.Data.Map;

namespace BeatGame.Logic.Lighting
{
    public class LightCntroller : MonoBehaviour
    {
        [SerializeField]
        Light light;

        [SerializeField]
        float defaultIntensity = .2f;
        [SerializeField]
        float turnedOnIntensity = .8f;
        [SerializeField]
        float flashIntensity = 1f;

        [SerializeField]
        SongEventType[] supportedEventTypes;

        private void OnEnable()
        {
            EventPlayingSystem.Instance.OnPlayEvent += PlayEvent;

            TurnOff();
        }

        private void OnDisable()
        {
            if (EventPlayingSystem.Instance != null)
            {
                EventPlayingSystem.Instance.OnPlayEvent -= PlayEvent;
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
                        light.color = color;

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
            light.intensity = defaultIntensity;
            light.color = Color.grey;
        }

        public virtual void TurnOn()
        {
            light.intensity = turnedOnIntensity;
        }

        public virtual void Flash()
        {
            TurnOn();
            StopAllCoroutines();
            StartCoroutine(FlashRoutine());
        }

        public virtual void Fade()
        {
            TurnOn();
            StopAllCoroutines();
            StartCoroutine(FadeRoutine(turnedOnIntensity, defaultIntensity, .6f));
        }

        protected IEnumerator FlashRoutine()
        {
            float intensity = turnedOnIntensity;
            while (intensity < flashIntensity)
            {
                intensity += (flashIntensity - turnedOnIntensity) / .1f * Time.deltaTime;
                light.intensity = intensity;


                yield return null;
            }

            while (intensity > flashIntensity)
            {
                intensity -= (flashIntensity - turnedOnIntensity) / .2f * Time.deltaTime;
                light.intensity = intensity;

                yield return null;
            }
        }


        IEnumerator FadeRoutine(float startValue, float endValue, float duration)
        {
            float startTime = Time.time;

            while (true)
            {
                float elapsed = Time.time - startTime;

                light.intensity = Mathf.Lerp(startValue, endValue, elapsed / duration);

                if (light.intensity >= endValue)
                {
                    light.intensity = endValue;
                    break;
                }

                yield return null;
            }
            TurnOff();
        }
    }
}
