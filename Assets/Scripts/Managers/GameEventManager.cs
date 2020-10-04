using UnityEngine;
using System.Collections;
using System;

namespace BeatGame.Logic.Managers
{
    public class GameEventManager : MonoBehaviour
    {
        public static GameEventManager Instance;

        // Int is note type
        public event Action<int> OnNoteMissed;
        // Int is note type
        public event Action<int> OnNoteBadCut;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            
        }

        public void NoteMissed(int type)
        {
            OnNoteMissed?.Invoke(type);
        }
    }
}