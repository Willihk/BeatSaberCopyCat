using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace BeatGame.Logic.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance;

        public int CurrentScore;
        public int CurrentCombo;
        public int CurrentMultiplier;
        public int CurrentMultiplierCount;

        public float TotalMultiplier;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            GameManager.Instance.OnSongStart += ResetScore;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnSongStart -= ResetScore;
        }

        public void AddScore(int amount)
        {
            AddCombo();
            CurrentScore += (int)(amount * CurrentMultiplier * TotalMultiplier);
        }

        public void AddCombo()
        {
            CurrentCombo++;
            if (CurrentMultiplierCount < 14)
            {
                CurrentMultiplierCount++;
                UpdateMultiplier();
            }
        }

        public void MissedNote()
        {
            CurrentCombo = 0;

            if (CurrentMultiplier == 8)
                CurrentMultiplierCount -= 8;
            else if (CurrentMultiplier == 4)
                CurrentMultiplierCount -= 4;
            else if (CurrentMultiplier == 2)
                CurrentMultiplierCount -= 2;

            UpdateMultiplier();
        }

        void UpdateMultiplier()
        {
            if (CurrentMultiplierCount < 2)
                CurrentMultiplier = 1;
            else if (CurrentMultiplierCount < 6)
                CurrentMultiplier = 2;
            else if (CurrentMultiplierCount < 14)
                CurrentMultiplier = 4;
            else if (CurrentMultiplierCount == 14)
                CurrentMultiplier = 8;
        }

        public void ResetScore()
        {
            CurrentCombo = 0;
            CurrentMultiplier = 1;
            CurrentMultiplierCount = 0;
            CurrentScore = 0;
        }
    }
}