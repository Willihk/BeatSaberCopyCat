using System;
using UnityEngine;

namespace BeatGame.Events
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvents/IntEvent")]
    [Serializable]
    public class IntGameEvent : GameEvent<int>
    {
    }
}