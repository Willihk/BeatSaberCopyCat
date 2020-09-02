using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FireworkController : MonoBehaviour
{
    [SerializeField]
    AudioClip[] fireworkSounds;
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    VisualEffect fireworkEffect;

    public void StartFireworks()
    {
        StopAllCoroutines();
        StartCoroutine(SpawnFireworks());
    }

    public void StopFireworks()
    {
        StopAllCoroutines();
    }

    IEnumerator SpawnFireworks()
    {
        while (true)
        {
            fireworkEffect.SendEvent("SpawnFirework");
            audioSource.PlayOneShot(fireworkSounds[Random.Range(0, fireworkSounds.Length)]);
            yield return new WaitForSeconds(Random.Range(.5f, 1.2f));
        }
    }
}
