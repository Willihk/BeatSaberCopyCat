﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System.Linq;

public class MasterLaserController : MonoBehaviour
{
    [SerializeField]
    List<LaserController> laserControllers;

    [SerializeField]
    EventType[] supportedEventTypes;

    [SerializeField]
    Material blueMaterial;
    [SerializeField]
    Material redMaterial;

    private void Start()
    {
        if (laserControllers == null)
            laserControllers = new List<LaserController>();

        blueMaterial = new Material(blueMaterial);
        redMaterial = new Material(redMaterial);

        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EventPlayingSystem>().OnPlayEvent += PlayEvent;
    }

    private void PlayEvent(int type, int value)
    {
        if (supportedEventTypes.Any(x => (int)x == type))
        {
            switch (type)
            {
                case 2:
                case 3:
                    if (value > 4)
                    {
                        value -= 4;
                        laserControllers.ForEach(x => x.SetMaterial(blueMaterial));
                    }
                    else
                    {
                        laserControllers.ForEach(x => x.SetMaterial(redMaterial));
                    }

                    switch (value)
                    {
                        case 0:
                            laserControllers.ForEach(x => x.TurnOff());
                            break;
                        case 1:
                            laserControllers.ForEach(x => x.TurnOn());
                            break;
                        case 2:
                            laserControllers.ForEach(x => x.Flash());
                            break;
                        case 3:
                            laserControllers.ForEach(x => x.Fade());
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
                    Debug.Log("rotating");
                    break;
                default:
                    break;
            }
        }
    }
}
