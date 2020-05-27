using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaberMouseController : MonoBehaviour
{
    void Update()
    {
        var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        transform.position = point;
    }
}
