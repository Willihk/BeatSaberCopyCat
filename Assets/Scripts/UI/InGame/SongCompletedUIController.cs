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
        TextMeshProUGUI songNameText;
        [SerializeField]
        TextMeshProUGUI difficultyText;
        [SerializeField]
        TextMeshProUGUI scoreText;
        [SerializeField]
        TextMeshProUGUI newHighScoreText;

        private void OnEnable()
        {
            canvas.worldCamera = Camera.main;
            canvas.enabled = false;
            Instance = this;
        }

        HighScoreData highScore;

        public void Display()
        {
            canvas.enabled = true;
            songNameText.text = CurrentSongDataManager.Instance.SelectedSongData.SongInfoFileData.SongName;
            difficultyText.text = CurrentSongDataManager.Instance.Difficulity;
            scoreText.text = ScoreManager.Instance.CurrentScore.ToString();

            highScore = HighScoreManager.Instance.GetHighScoreForSong(
               CurrentSongDataManager.Instance.SelectedSongData.SongInfoFileData.SongName,
               CurrentSongDataManager.Instance.SelectedSongData.SongInfoFileData.LevelAuthorName,
               CurrentSongDataManager.Instance.SelectedDifficultyMap.Difficulty);

            if (ScoreManager.Instance.CurrentScore > highScore.Score)
                newHighScoreText.enabled = true;
            else
                newHighScoreText.enabled = false;
        }

        public void ReturnToMenu()
        {
            if (ScoreManager.Instance.CurrentScore > highScore.Score)
            {
                highScore.Score = ScoreManager.Instance.CurrentScore;
                HighScoreManager.Instance.UpdateScore(highScore);
            }

            GameManager.Instance.ReturnToMenu();
        }

    }
}