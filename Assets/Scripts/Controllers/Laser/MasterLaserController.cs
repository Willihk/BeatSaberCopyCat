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
    List<LaserControllerBase> laserControllers;

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

    private void Start()
    {
        if (laserControllers == null)
            laserControllers = new List<LaserControllerBase>();

        if (findLaserControllersInChildren)
            GetLasersInChildReqursive(transform);

        blueMaterial = new Material(blueMaterial);
        redMaterial = new Material(redMaterial);

        currentMaterial = blueMaterial;

        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EventPlayingSystem>().OnPlayEvent += PlayEvent;

        TurnOff();
    }

    void GetLasersInChildReqursive(Transform child)
    {
        foreach (Transform item in child)
        {
            if (item.TryGetComponent(out LaserControllerBase laserController))
            {
                laserControllers.Add(laserController);
            }
            GetLasersInChildReqursive(item);
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

                    laserControllers.ForEach(x => x.SetMaterial(currentMaterial));

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
                    for (int i = 0; i < laserControllers.Count; i++)
                    {
                        if (i % 2 == 0)
                            laserControllers[i].SetRotation(value);
                        else
                            laserControllers[i].SetRotation(-value);
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
        laserControllers.ForEach(x => x.TurnOff());
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
