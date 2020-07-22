using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.Entities;
using System.Collections.Generic;

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

    [SerializeField, ColorUsage(true, true)]
    Color blueColor = new Color(0, 0.7035143f, 1);
    [SerializeField, ColorUsage(true, true)]
    Color redColor = new Color(1, 0, 0);

    Color currentColor;

    private void Start()
    {
        material = new Material(material);

        if (controllers == null)
            controllers = new List<VolumetricControllerBase>();

        if (findLaserControllersInChildren)
            GetControllersInChildReqursive(transform);

        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EventPlayingSystem>().OnPlayEvent += PlayEvent;

        TurnOff();
    }

    void GetControllersInChildReqursive(Transform child)
    {
        foreach (Transform item in child)
        {
            if (item.TryGetComponent(out VolumetricControllerBase controller))
            {
                controllers.Add(controller);
                controller.SetMaterial(material);
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
                        material.SetColor("_Color", blueColor);
                        currentColor = blueColor;
                    }
                    else
                    {
                        material.SetColor("_Color", redColor);
                        currentColor = redColor;
                    }

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
                default:
                    break;
            }
        }
    }

    public virtual void TurnOff()
    {
        currentColor.a = 0;
        material.SetColor("_Color", currentColor);
        controllers.ForEach(x => x.TurnOff());
    }

    public virtual void TurnOn()
    {
        currentColor.a = 1;
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
        currentColor.a = 1;
        while (currentColor.a > 0)
        {
            currentColor.a -= 1f / .6f * Time.deltaTime;
            material.SetColor("_Color", currentColor);
            yield return null;
        }

        TurnOff();
    }
}
