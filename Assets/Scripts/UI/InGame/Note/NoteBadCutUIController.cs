using UnityEngine;
using System.Collections;
using BeatGame.Logic.Managers;
using UnityEngine.UI;
using BeatGame.Events;

namespace BeatGame.UI.Controllers
{
    public class NoteBadCutUIController : MonoBehaviour
    {
        [SerializeField]
        GameEvent<int> NoteBadCutEvent;

        [SerializeField]
        int noteType;
        [SerializeField]
        Image missImage;

        [SerializeField]
        Vector3 startPosition;
        [SerializeField]
        Vector3 endPosition;

        [SerializeField]
        Color startColor = new Color(1, .5f, .5f, .5f);
        [SerializeField]
        Color endColor = new Color(1, 1, 1, 1);

        [SerializeField]
        AnimationCurve movementCurve;

       

        private void OnEnable()
        {
            missImage.enabled = false;
           NoteBadCutEvent.EventListeners += BadCut;
        }

        private void OnDisable()
        {
            NoteBadCutEvent.EventListeners -= BadCut;
        }

        void BadCut(int type)
        {
            if (type == noteType)
            {
                StopAllCoroutines();
                StartCoroutine(AnimateImage(.5f));
            }
        }

        IEnumerator AnimateImage(float duration)
        {
            missImage.enabled = true;
            missImage.rectTransform.localPosition = startPosition;

            float startTime = Time.time;

            while (true)
            {
                float elapsed = Time.time - startTime;

                missImage.rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, movementCurve.Evaluate(elapsed / duration));
                missImage.color = Color.Lerp(startColor, endColor, movementCurve.Evaluate(elapsed / duration));
                if (elapsed >= duration)
                    break;

                yield return null;
            }

            startTime = Time.time;

            while (true)
            {
                float elapsed = Time.time - startTime;
                Color color = endColor;
                color.a = 0;
                missImage.color = Color.Lerp(endColor, color, movementCurve.Evaluate(elapsed / duration));

                if (elapsed >= duration / 2)
                    break;

                yield return null;
            }
            missImage.enabled = false;
        }
    }
}