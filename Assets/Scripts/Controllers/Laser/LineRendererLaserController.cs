using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json.Schema;
using Unity.Mathematics;

namespace BeatGame.Logic.Lasers
{
    [ExecuteAlways]
    public class LineRendererLaserController : LaserControllerBase
    {
        [SerializeField]
        LineRenderer lineRenderer;

        [SerializeField]
        float laserLength = 100;

        private void OnEnable()
        {
            if (lineRenderer == null)
                TryGetComponent(out lineRenderer);


            startRotation = transform.rotation;
            lineRenderer.positionCount = 2;

            if (lineRenderer != null)
                material = lineRenderer.sharedMaterial;

            lineRenderer.enabled = true;
        }

        void Update()
        {
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
            if (rotationSpeed != 0)
                transform.rotation = startRotation;
        }

        public override void SetMaterial(Material material)
        {
            base.SetMaterial(material);
            if (lineRenderer == null)
                TryGetComponent(out lineRenderer);
            lineRenderer.material = material;
        }
    }
}