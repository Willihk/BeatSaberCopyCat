using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class LaserController : MonoBehaviour
{
    [SerializeField]
    Material BlueMaterial;
    [SerializeField]
    Material RedMaterial;

    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    float laserLength = 100;

    private void Start()
    {
        if (lineRenderer == null)
            TryGetComponent(out lineRenderer);


        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        UpdatePositions();
    }


    void UpdatePositions()
    {

        lineRenderer.SetPosition(0, transform.TransformPoint(transform.up * laserLength));
        lineRenderer.SetPosition(1, transform.TransformPoint(-transform.up * laserLength));
    }
}
