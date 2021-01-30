using UnityEngine;
using System.Collections;
using System;
using BeatGame.Events;

namespace BeatGame.Logic.Managers
{
    public class HealthManager : MonoBehaviour
    {
        public static HealthManager Instance;

        public float MaxHealth = 50;
        public float Health = 50;

        public event Action OnDeath;
        public event Action OnHealthChanged;
        
        [SerializeField]
        GameEvent<int> NoteHitEvent;
        [SerializeField]
        GameEvent<int> NoteMissedEvent;
        [SerializeField]
        GameEvent<int> NoteBadCutEvent;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            NoteHitEvent.EventListeners += HitNote;

            NoteMissedEvent.EventListeners += MissedNote;

            NoteBadCutEvent.EventListeners += NoteBadCut;

            GameManager.Instance.OnSongStart += Setup;
        }

        private void OnDestroy()
        {
             NoteHitEvent.EventListeners -= HitNote;

            NoteMissedEvent.EventListeners -= MissedNote;

            NoteBadCutEvent.EventListeners -= NoteBadCut;

            if (GameManager.Instance != null)
                GameManager.Instance.OnSongStart -= Setup;
        }

        private void Setup()
        {
            Health = MaxHealth / 2;
        }

        public void AddHealth(int amount)
        {
            Health += amount;
            if (Health > 100)
            {
                Health = 100;
            }
            OnHealthChanged?.Invoke();
        }

        public void RemoveHealth(float amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                if (SettingsManager.Instance.Settings["Modifiers"]["NoFail"].IntValue != 1)
                    OnDeath?.Invoke();

                Health = 0;
            }
            OnHealthChanged?.Invoke();
        }

        public void HitNote(int type)
        {
            AddHealth(1);
        }

        public void NoteBadCut(int type)
        {
            RemoveHealth(10);
        } 

        public void MissedNote(int type)
        {
            RemoveHealth(15);
        }

        public void HitBomb()
        {
            RemoveHealth(15);
        }

        public void InsideObstacle()
        {
            RemoveHealth(100f * Time.deltaTime);
        }
    }
}