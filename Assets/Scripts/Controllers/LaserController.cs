using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json.Schema;
using Unity.Mathematics;

[ExecuteAlways]
public class LaserController : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    float laserLength = 100;

    [SerializeField]
    float laserIntensity = 15;
    [SerializeField]
    float laserFlashIntensity = 30;

    Quaternion startRotation;
    float rotationSpeed;

    private void Start()
    {
        if (lineRenderer == null)
            TryGetComponent(out lineRenderer);

        startRotation = transform.rotation;
        lineRenderer.positionCount = 2;

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

    public void TurnOff()
    {
        lineRenderer.enabled = false;
        transform.rotation = startRotation;
    }

    public void TurnOn()
    {
        lineRenderer.enabled = true;
        lineRenderer.material.SetFloat("_FadeAmount", 1);
    }

    public void Flash()
    {
        TurnOn();
        StopAllCoroutines();
        StartCoroutine(FlashMaterial());
    }

    public void Fade()
    {
        TurnOn();
        StopAllCoroutines();
        StartCoroutine(FadeLaser());
    }

    IEnumerator FlashMaterial()
    {
        float intensity = laserIntensity;
        while (intensity < laserFlashIntensity)
        {
            intensity += (laserFlashIntensity - laserIntensity) / .1f * Time.deltaTime;
            lineRenderer.material.SetFloat("_EmissionIntensity", intensity);

            yield return null;
        }

        while (intensity > laserIntensity)
        {
            intensity -= (laserFlashIntensity - laserIntensity) / .2f * Time.deltaTime;
            lineRenderer.material.SetFloat("_EmissionIntensity", intensity);

            yield return null;
        }

    }

    IEnumerator FadeLaser()
    {
        float fadeAmount = .7f;
        while (fadeAmount > 0)
        {
            fadeAmount -= .7f / .6f * Time.deltaTime;

            lineRenderer.material.SetFloat("_FadeAmount", fadeAmount);
            yield return null;
        }

        TurnOff();
        lineRenderer.material.SetFloat("_FadeAmount", fadeAmount);
    }

    public void SetMaterial(Material material)
    {
        lineRenderer.material = material;
    }

    public void SetRotation(float value)
    {
        if (value == 0)
        {
            transform.rotation = startRotation;
            // Reset to default
        }

        rotationSpeed = value;
    }
}
