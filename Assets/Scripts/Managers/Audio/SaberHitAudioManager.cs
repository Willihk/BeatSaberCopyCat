using UnityEngine;
using System.Collections;

namespace BeatGame.Logic.Managers
{
    public class SaberHitAudioManager : MonoBehaviour
    {
        public static SaberHitAudioManager Instance;

        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        AudioClip[] ShortHitSounds;
        [SerializeField]
        AudioClip[] LongHitSounds;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void PlaySound()
        {
            audioSource.clip = ShortHitSounds[Random.Range(0, ShortHitSounds.Length)];

            if (audioSource.clip != null)
                audioSource.Play();
        }
    }
}