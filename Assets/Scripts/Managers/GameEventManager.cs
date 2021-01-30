using UnityEngine;
using System.Collections;
using System;
using BeatGame.Events;

namespace BeatGame.Logic.Managers
{
    public class GameEventManager : MonoBehaviour
    {
        public static GameEventManager Instance;

        // Int is sabers affected type
        public GameEvent<int> OnNoteHit;
        // Int is note type
        public GameEvent<int> OnNoteMissed;
        // Int is note type
        public GameEvent<int> OnNoteBadCut;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void NoteHit(int value)
        {
            OnNoteHit.Raise(value);
        }

        public void NoteMissed(int type)
        {
            OnNoteMissed.Raise(type);
        }

        public void NoteBadCut(int type)
        {
            OnNoteBadCut.Raise(type);
        }
    }
}