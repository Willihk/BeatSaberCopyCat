using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json.Schema;
using Unity.Mathematics;

[ExecuteAlways]
public class LaserController : LaserControllerBase
{
    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    float laserLength = 100;

  
    private void Start()
    {
        if (lineRenderer == null)
            TryGetComponent(out lineRenderer);


        startRotation = transform.rotation;
        lineRenderer.positionCount = 2;

        material = lineRenderer.sharedMaterial;
        TurnOff();
    }

    void Update()
    {
        if (lineRenderer.enabled)
            UpdatePositions();

        if (rotationSpeed != 0)
        {
            transform.Rotate(new Vector3(rotationSpeed * 2 * Time.deltaTime, 0, 0), Space.Self);
        }
    }


    void UpdatePositions()
    {

        lineRenderer.SetPosition(0, transform.TransformPoint(transform.up * laserLength));
        lineRenderer.SetPosition(1, transform.TransformPoint(-transform.up * laserLength));
    }

    public override void TurnOff()
    {
        base.TurnOff();
        lineRenderer.enabled = false;
    }

    public override void TurnOn()
    {
        base.TurnOn();
        lineRenderer.enabled = true;
    }

    public override void SetMaterial(Material material)
    {
        base.SetMaterial(material);
        lineRenderer.material = material;
    }
}
