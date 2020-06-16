using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterRingController : MonoBehaviour
{
    [SerializeField]
    int ringCount = 25;
    [SerializeField]
    Vector3 ringGap = new Vector3(0,0, 10);

    [SerializeField]
    GameObject ringObject;
    [SerializeField]
    List<GameObject> rings;

    private void Start()
    {
        if (rings == null)
            rings = new List<GameObject>();

        CreateRings();
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
}
