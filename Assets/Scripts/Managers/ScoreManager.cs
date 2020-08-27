﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace BeatGame.Logic.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance;

        public int CurrentScore;
        public int CurrentCombo;
        public int CurrentMultiplier;
        public int CurrentMultiplierCount;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Contains("Map"))
            {
                CurrentScore = 0;
                CurrentCombo = 0;
                CurrentMultiplier = 1;
            }
        }

        public void AddScore(int amount)
        {
            AddCombo();
            CurrentScore += amount * CurrentMultiplier;
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
    }
}