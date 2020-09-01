using UnityEngine;
using System.Collections;
using System;

namespace BeatGame.Logic.Managers
{
    public class HealthManager : MonoBehaviour
    {
        public static HealthManager Instance;

        public int MaxHealth = 50;
        public int Health = 50;

        public Action OnDeath;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            GameManager.Instance.OnSongStart += Setup;
        }

        private void OnDestroy()
        {
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
        }

        public void RemoveHealth(int amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        public void HitNote()
        {
            AddHealth(15);
        }

        public void MissedNote()
        {
            RemoveHealth(8);
        }

        public void HitBomb()
        {
            RemoveHealth(15);
        }
    }
}