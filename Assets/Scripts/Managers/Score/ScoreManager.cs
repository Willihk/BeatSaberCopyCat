using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using BeatGame.Events;

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
        public event Action<int> OnScoreAdded;

        public GameEvent<int> NoteHitEvent;
        public GameEvent<int> NoteHitMissed;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnSongStart += ResetScore;

            NoteHitEvent.EventListeners += HitNote;

            NoteHitMissed.EventListeners += MissedNote;

            if (SettingsManager.Instance != null)
                SettingsManager.Instance.OnConfigChanged += CalcMultiplier;

            CalcMultiplier();
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnSongStart -= ResetScore;

            NoteHitEvent.EventListeners -= HitNote;

            NoteHitMissed.EventListeners -= MissedNote;

            if (SettingsManager.Instance != null)
                SettingsManager.Instance.OnConfigChanged -= CalcMultiplier;
        }

        public void HitNote(int type)
        {
            AddCombo();
            CurrentScore += (int)(100 * CurrentMultiplier * TotalMultiplier);
            OnScoreAdded?.Invoke(100);
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

        public void MissedNote(int type)
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

        public void CalcMultiplier()
        {
            TotalMultiplier = 1;
            var modifiers = SettingsManager.Instance.Settings["Modifiers"];

            if (modifiers["NoFail"].IntValue == 1)
            {
                TotalMultiplier += -0.50f;
            }
            if (modifiers["NoArrows"].IntValue == 1)
            {
                TotalMultiplier += -0.10f;
            }
            if (modifiers["DoubleSaber"].IntValue == 1)
            {
                TotalMultiplier += 0.10f;
            }
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