using UnityEngine;
using System.Collections;

public class LaserControllerBase : MonoBehaviour
{
    [SerializeField]
    protected float laserIntensity = 15;
    [SerializeField]
    protected float laserFlashIntensity = 30;

    protected Quaternion startRotation;
    protected float rotationSpeed;

    protected Material material;

    public virtual void TurnOff()
    {
        material.SetFloat("_FadeAmount", 0);
        transform.rotation = startRotation;
    }

    public virtual void TurnOn()
    {
        material.SetFloat("_FadeAmount", 1);
    }

    public virtual void Flash()
    {
        TurnOn();
        StopAllCoroutines();
        StartCoroutine(FlashMaterial());
    }

    public virtual void Fade()
    {
        TurnOn();
        StopAllCoroutines();
        StartCoroutine(FadeLaser());
    }

    protected IEnumerator FlashMaterial()
    {
        float intensity = laserIntensity;
        while (intensity < laserFlashIntensity)
        {
            intensity += (laserFlashIntensity - laserIntensity) / .1f * Time.deltaTime;
            material.SetFloat("_EmissionIntensity", intensity);

            yield return null;
        }

        while (intensity > laserIntensity)
        {
            intensity -= (laserFlashIntensity - laserIntensity) / .2f * Time.deltaTime;
            material.SetFloat("_EmissionIntensity", intensity);

            yield return null;
        }

    }

    protected IEnumerator FadeLaser()
    {
        float fadeAmount = .7f;
        while (fadeAmount > 0)
        {
            fadeAmount -= .7f / .6f * Time.deltaTime;

            material.SetFloat("_FadeAmount", fadeAmount);
            yield return null;
        }

        TurnOff();
        material.SetFloat("_FadeAmount", fadeAmount);
    }

    public virtual void SetMaterial(Material material)
    {
        this.material = material;
    }

    public virtual void SetRotation(float value)
    {
        if (value == 0)
        {
            transform.rotation = startRotation;
            // Reset to default
        }

        rotationSpeed = value;
    }
}
