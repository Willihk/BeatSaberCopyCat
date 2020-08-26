using UnityEngine;
using System.Collections;
using TMPro;
using BeatGame.Logic.Managers;
using System.Globalization;

namespace Assets.Scripts.UI.InGame
{
    public class ScoreUIController : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI scoreText;

        NumberFormatInfo formatInfo = new NumberFormatInfo { NumberGroupSeparator = " " };

        int currentScore;

        // Update is called once per frame
        void Update()
        {
            if (ScoreManager.Instance.CurrentScore != currentScore)
            {
                scoreText.text = ScoreManager.Instance.CurrentScore.ToString("n0", formatInfo);
                currentScore = ScoreManager.Instance.CurrentScore;
            }
        }
    }
}