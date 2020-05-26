using UnityEngine;
using System.Collections;

public class SaberController : MonoBehaviour
{
    public LayerMask layer;
    public Vector3 previousPos;

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1, layer))
        {
            if (Vector3.Angle(transform.position - previousPos, hit.transform.up) > 130)
            {
                Destroy(hit.transform.gameObject);
            }
        }

        previousPos = transform.position;
    }
}
