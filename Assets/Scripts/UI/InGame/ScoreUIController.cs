using UnityEngine;
using System.Collections;
using TMPro;
using BeatGame.Logic.Managers;

namespace Assets.Scripts.UI.InGame
{
    public class ScoreUIController : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI scoreText;

        int currentScore;

        // Update is called once per frame
        void Update()
        {
            if (ScoreManager.Instance.CurrentScore != currentScore)
            {
                scoreText.text = ScoreManager.Instance.CurrentScore.ToString("{0:n0}").Replace(',', ' ');
                currentScore = ScoreManager.Instance.CurrentScore;
            }
        }
    }
}