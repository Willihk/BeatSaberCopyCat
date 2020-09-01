using UnityEngine;
using System.Collections;
using TMPro;
using BeatGame.Logic.Managers;
using BeatGame.Data;

namespace BeatGame.UI.Controllers
{
    public class SongCompletedUIController : MonoBehaviour
    {
        public static SongCompletedUIController Instance;
        [SerializeField]
        public Canvas canvas;
        [SerializeField]
        TextMeshProUGUI titleText;
        [SerializeField]
        TextMeshProUGUI songNameText;
        [SerializeField]
        TextMeshProUGUI difficultyText;
        [SerializeField]
        TextMeshProUGUI scoreText;
        [SerializeField]
        TextMeshProUGUI newHighScoreText;

        bool failed;

        private void OnEnable()
        {
            canvas.worldCamera = Camera.main;
            canvas.enabled = false;
            Instance = this;
        }

        HighScoreData highScore;

        public void Display(bool failed = false)
        {
            this.failed = failed;
            canvas.enabled = true;
            if (failed)
            {
                titleText.text = "Game Over";
                titleText.color = Color.red;
            }
            else
            {
                titleText.text = "Song Completed";
                titleText.color = new Color(0.3921569f, 0.7843137f, 1);
            }

            songNameText.text = CurrentSongDataManager.Instance.SelectedSongData.SongInfoFileData.SongName;
            difficultyText.text = CurrentSongDataManager.Instance.Difficulity;
            scoreText.text = ScoreManager.Instance.CurrentScore.ToString();

            highScore = HighScoreManager.Instance.GetHighScoreForSong(
               CurrentSongDataManager.Instance.SelectedSongData.SongInfoFileData.SongName,
               CurrentSongDataManager.Instance.SelectedSongData.SongInfoFileData.LevelAuthorName,
               CurrentSongDataManager.Instance.SelectedDifficultyMap.Difficulty);

            if (ScoreManager.Instance.CurrentScore > highScore.Score && !failed)
                newHighScoreText.enabled = true;
            else
                newHighScoreText.enabled = false;
        }

        public void RestartSong()
        {
            if (ScoreManager.Instance.CurrentScore > highScore.Score && !failed)
            {
                highScore.Score = ScoreManager.Instance.CurrentScore;
                HighScoreManager.Instance.UpdateScore(highScore);
            }
            GameManager.Instance.Restart();
            canvas.enabled = false;
        }

        public void ReturnToMenu()
        {
            if (ScoreManager.Instance.CurrentScore > highScore.Score && !failed)
            {
                highScore.Score = ScoreManager.Instance.CurrentScore;
                HighScoreManager.Instance.UpdateScore(highScore);
            }
            GameManager.Instance.ReturnToMenu();
        }

    }
}