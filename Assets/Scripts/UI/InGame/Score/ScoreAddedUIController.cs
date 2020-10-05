using UnityEngine;
using System.Collections;
using BeatGame.Logic.Managers;
using TMPro;

namespace BeatGame.UI.Controllers
{
    public class ScoreAddedUIController : MonoBehaviour
    {

        [SerializeField]
        int textPoolCount = 10;
        [SerializeField]
        TextMeshProUGUI text;

        [SerializeField]
        Vector3 startPosition;
        [SerializeField]
        Vector3 endPosition;

        [SerializeField]
        Vector3 randomMinOffset;
        [SerializeField]
        Vector3 randomMaxOffset;

        [SerializeField]
        Color startColor = new Color(1, .5f, .5f, .5f);
        [SerializeField]
        Color endColor = new Color(1, 1, 1, 1);

        [SerializeField]
        AnimationCurve movementCurve;

        TextMeshProUGUI[] textPoolObjects;


        private void OnEnable()
        {
            if (textPoolObjects == null)
            {
                textPoolObjects = new TextMeshProUGUI[textPoolCount];
                text.enabled = false;
                for (int i = 0; i < textPoolCount; i++)
                {
                    textPoolObjects[i] = Instantiate(text.gameObject, text.transform.parent).GetComponent<TextMeshProUGUI>();
                }
            }

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
            var textObject = GetAvailableText();
            if (textObject != null)
                StartCoroutine(Animate(.5f, amount, textObject));
        }

        TextMeshProUGUI GetAvailableText()
        {
            for (int i = 0; i < textPoolObjects.Length; i++)
            {
                if (!textPoolObjects[i].enabled)
                {
                    return textPoolObjects[i];
                }
            }
            return null;
        }

        IEnumerator Animate(float duration, int amountAdded, TextMeshProUGUI text)
        {
            text.enabled = true;
            text.rectTransform.localPosition = startPosition;
            text.text = amountAdded.ToString();

            Vector3 offset = Vector3.Lerp(randomMinOffset, randomMaxOffset, Random.Range(0, 1));

            float startTime = Time.time;

            while (true)
            {
                float elapsed = Time.time - startTime;

                text.rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, movementCurve.Evaluate(elapsed / duration)) + offset;
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