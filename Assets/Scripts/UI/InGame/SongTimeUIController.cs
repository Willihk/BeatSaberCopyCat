using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
using BeatGame.Logic.Managers;

namespace BeatGame.UI.Controllers
{
    public class SongTimeUIController : MonoBehaviour
    {
        [SerializeField]
        Slider slider;
        [SerializeField]
        TextMeshProUGUI currentTimeText;
        [SerializeField]
        TextMeshProUGUI totalTimeText;

        float currentTime;
        int totalTime;

        public void Setup()
        {
            float seconds = GameManager.Instance.audioSource.clip.length;
            totalTime = (int)seconds;
            totalTimeText.text = $"{math.floor(seconds / 60)}:{math.floor(seconds % 60).ToString("00")}";
        }

        // Update is called once per frame
        void Update()
        {
            if (totalTime == 0)
                return;

            float seconds = (float)((GameManager.Instance.CurrentBeat - CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration) * CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat);

            if (seconds < 0)
                return;

            currentTime = seconds;
            currentTimeText.text = $"{math.floor(seconds / 60)}:{math.floor(seconds % 60).ToString("00")}";

            slider.value = currentTime / totalTime;
        }
    }
}