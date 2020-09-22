using UnityEngine;
using System.Collections;
using System;

namespace BeatGame.Logic.Managers
{
    public class HealthManager : MonoBehaviour
    {
        public static HealthManager Instance;

        public float MaxHealth = 50;
        public float Health = 50;

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

        public void RemoveHealth(float amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                if (SettingsManager.Instance.Settings["Modifiers"]["NoFail"].IntValue != 1)
                {
                    OnDeath?.Invoke();
                }
            }
        }

        public void HitNote()
        {
            AddHealth(1);
        }

        public void MissedNote()
        {
            RemoveHealth(15);
        }

        public void HitBomb()
        {
            RemoveHealth(15);
        }

        public void InsideObstacle()
        {
            RemoveHealth(60f * Time.deltaTime);
            Debug.Log("Is inside obstacle");
        }
    }
}