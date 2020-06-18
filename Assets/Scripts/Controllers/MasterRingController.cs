using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class MasterRingController : MonoBehaviour
{
    [SerializeField]
    int ringCount = 25;
    [SerializeField]
    Vector3 ringGap = new Vector3(0, 0, 10);

    [SerializeField]
    GameObject ringObject;
    [SerializeField]
    List<GameObject> rings;

    [SerializeField]
    SongEventType[] supportedEventTypes;

    float ringSpeed;

    private void Start()
    {
        if (rings == null)
            rings = new List<GameObject>();

        CreateRings();

        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EventPlayingSystem>().OnPlayEvent += PlayEvent;
    }

    void CreateRings()
    {
        for (int i = rings.Count; i < ringCount; i++)
        {
            var newRing = Instantiate(ringObject, transform);
            newRing.transform.position += ringGap * i;

            rings.Add(newRing);
        }
    }

    private void Update()
    {
        RotateRingsIntoPosition();
    }

    void RotateRingsIntoPosition()
    {
        if (ringSpeed < 0)
            ringSpeed += math.abs(ringSpeed) / .5f * Time.deltaTime;
        if (ringSpeed > 0)
            ringSpeed -= ringSpeed / .5f * Time.deltaTime;

        for (int i = 0; i < rings.Count; i++)
        {
            rings[i].transform.Rotate(new Vector3(0, 0, ringSpeed * 6 * i * Time.deltaTime), Space.Self);
        }
    }

    void NewRotation()
    {
        ringSpeed = Random.Range(-10, 10);
    }


    private void PlayEvent(int type, int value)
    {
        if (supportedEventTypes.Any(x => (int)x == type))
        {
            switch (type)
            {
                case 8:
                    NewRotation();
                    break;
                default:
                    break;
            }
        }
    }
}
