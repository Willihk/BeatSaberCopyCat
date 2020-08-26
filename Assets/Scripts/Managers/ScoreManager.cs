using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace BeatGame.Logic.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance;

        public int CurrentScore;

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
            }
        }

        public void AddScore(int amount)
        {
            CurrentScore += amount;
        }
    }
}