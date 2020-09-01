using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace BeatGame.Logic.Managers
{
    public class SceneFader : MonoBehaviour
    {
        public delegate void FadeCompleted();
        public static SceneFader Instance;
        FadeCompleted callBack;

        [SerializeField]
        MeshRenderer renderer;
        Material material;

        Color color;
        float timeToFade;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            material = renderer.material;
            color = new Color(0, 0, 0, 1);
        }

        public void FadeOut(float duration, FadeCompleted callBack = null)
        {
            timeToFade = duration;
            this.callBack = callBack;
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }

        public void FadeIn(float duration, FadeCompleted callBack = null)
        {
            timeToFade = duration;
            this.callBack = callBack;
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }

        IEnumerator FadeOut()
        {
            while (color.a > 0)
            {
                // StartValue / duration * deltaTime
                color.a -= 1f / timeToFade * Time.deltaTime;
                material.SetFloat("_FadeAmount", color.a);
                yield return null;
            }
            callBack?.Invoke();
        }

        IEnumerator FadeIn()
        {
            while (color.a < 1)
            {
                // StartValue / duration * deltaTime
                color.a += 1f / timeToFade * Time.deltaTime;
                material.SetFloat("_FadeAmount", color.a);
                yield return null;
            }
            callBack?.Invoke();
        }
    }
}