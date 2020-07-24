using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System.Linq;

public class MasterLaserController : MonoBehaviour
{
    [SerializeField]
    bool findLaserControllersInChildren = false;
    [SerializeField]
    List<LaserControllerBase> controllers;

    [SerializeField]
    SongEventType[] supportedEventTypes;

    [SerializeField]
    Material blueMaterial;
    [SerializeField]
    Material redMaterial;

    [SerializeField]
    protected float laserIntensity = 3;
    [SerializeField]
    protected float laserFlashIntensity = 6;

    Material currentMaterial;

    private void OnEnable()
    {
        if (controllers == null)
            controllers = new List<LaserControllerBase>();

        if (findLaserControllersInChildren)
            GetControllersInChildReqursive(transform);

        blueMaterial = new Material(blueMaterial);
        redMaterial = new Material(redMaterial);

        blueMaterial.SetFloat("_EmissionIntensity", laserIntensity);
        redMaterial.SetFloat("_EmissionIntensity", laserIntensity);


        currentMaterial = blueMaterial;

        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EventPlayingSystem>().OnPlayEvent += PlayEvent;

        controllers.ForEach(x => x.SetMaterial(currentMaterial));

        TurnOff();
    }

    private void OnDisable()
    {
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EventPlayingSystem>().OnPlayEvent -= PlayEvent;
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

    private void PlayEvent(int type, int value)
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
                    if (value > 4)
                    {
                        value -= 4;
                        currentMaterial = blueMaterial;
                    }
                    else
                    {
                        currentMaterial = redMaterial;
                    }

                    controllers.ForEach(x => x.SetMaterial(currentMaterial));

                    switch (value)
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
                            controllers[i].SetRotation(value);
                        else
                            controllers[i].SetRotation(-value);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public virtual void TurnOff()
    {
        currentMaterial.SetFloat("_FadeAmount", 0);
        controllers.ForEach(x => x.TurnOff());
    }

    public virtual void TurnOn()
    {
        currentMaterial.SetFloat("_FadeAmount", 1);
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
            currentMaterial.SetFloat("_EmissionIntensity", intensity);

            yield return null;
        }

        while (intensity > laserIntensity)
        {
            intensity -= (laserFlashIntensity - laserIntensity) / .2f * Time.deltaTime;
            currentMaterial.SetFloat("_EmissionIntensity", intensity);

            yield return null;
        }

    }

    protected IEnumerator FadeLaser()
    {
        float fadeAmount = .7f;
        while (fadeAmount > 0)
        {
            fadeAmount -= .7f / .6f * Time.deltaTime;

            currentMaterial.SetFloat("_FadeAmount", fadeAmount);
            yield return null;
        }

        TurnOff();
        currentMaterial.SetFloat("_FadeAmount", fadeAmount);
    }
}
