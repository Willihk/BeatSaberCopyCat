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
        [SerializeField]
        TextMeshProUGUI comboText;
        [SerializeField]
        TextMeshProUGUI multiplierText;

        NumberFormatInfo formatInfo = new NumberFormatInfo { NumberGroupSeparator = " " };

        int currentScore;
        int currentCombo;
        int currentMultiplier;

        // Update is called once per frame
        void Update()
        {
            if (ScoreManager.Instance.CurrentScore != currentScore)
            {
                scoreText.text = ScoreManager.Instance.CurrentScore.ToString("n0", formatInfo);
                currentScore = ScoreManager.Instance.CurrentScore;
            }
            if (ScoreManager.Instance.CurrentCombo != currentCombo)
            {
                comboText.text = ScoreManager.Instance.CurrentCombo.ToString("n0", formatInfo);
                currentCombo = ScoreManager.Instance.CurrentCombo;
            }
            if (ScoreManager.Instance.CurrentMultiplier != currentMultiplier)
            {
                multiplierText.text = ScoreManager.Instance.CurrentMultiplier.ToString();
                currentMultiplier = ScoreManager.Instance.CurrentMultiplier;
            }
        }
    }
}