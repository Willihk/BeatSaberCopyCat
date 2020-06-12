using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System.Linq;

public class MasterLaserController : MonoBehaviour
{
    [SerializeField]
    List<LaserController> laserControllers;

    [SerializeField]
    int supportedEventType; 

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
        if (type == supportedEventType)
        {
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
                case 13:
                    // Rotate
                    break;
                default:
                    break;
            }
        }
    }
}
