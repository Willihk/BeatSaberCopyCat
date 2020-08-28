using UnityEngine;
using System.Collections;
using TMPro;
using BeatGame.Logic.Managers;

namespace BeatGame.UI.Controllers
{
    public class SongCompletedUIController : MonoBehaviour
    {
        public static SongCompletedUIController Instance;
        [SerializeField]
        Canvas canvas;
        [SerializeField]
        TextMeshProUGUI songNameText;
        [SerializeField]
        TextMeshProUGUI difficultyText;
        [SerializeField]
        TextMeshProUGUI scoreText;

        private void OnEnable()
        {
            canvas.worldCamera = Camera.main;
            canvas.enabled = false;
            Instance = this;
        }

        public void Display()
        {
            canvas.enabled = true;
            songNameText.text = CurrentSongDataManager.Instance.SelectedSongData.SongInfoFileData.SongName;
            difficultyText.text = CurrentSongDataManager.Instance.Difficulity;
            scoreText.text = ScoreManager.Instance.CurrentScore.ToString();
        }

        public void ReturnToMenu()
        {
            GameManager.Instance.ReturnToMenu();
        }

    }
}