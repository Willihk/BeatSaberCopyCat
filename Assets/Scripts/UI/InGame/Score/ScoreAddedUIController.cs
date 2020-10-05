using UnityEngine;
using System.Collections;
using BeatGame.Logic.Managers;
using TMPro;

namespace BeatGame.UI.Controllers
{
    public class ScoreAddedUIController : MonoBehaviour
    {

        [SerializeField]
        TextMeshProUGUI text;

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
            text.enabled = false;
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.OnScoreAdded += ScoreAdded;
        }

        private void OnDisable()
        {
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.OnScoreAdded -= ScoreAdded;
        }

        void ScoreAdded(int amount)
        {
            StopAllCoroutines();
            StartCoroutine(Animate(.5f, amount));
        }

        IEnumerator Animate(float duration, int amountAdded)
        {
            text.enabled = true;
            text.rectTransform.localPosition = startPosition;
            text.text = amountAdded.ToString();

            float startTime = Time.time;

            while (true)
            {
                float elapsed = Time.time - startTime;

                text.rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, movementCurve.Evaluate(elapsed / duration));
                text.color = Color.Lerp(startColor, endColor, movementCurve.Evaluate(elapsed / duration));
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
                text.color = Color.Lerp(endColor, color, movementCurve.Evaluate(elapsed / duration));

                if (elapsed >= duration / 2)
                    break;

                yield return null;
            }
            text.enabled = false;
        }
    }
}