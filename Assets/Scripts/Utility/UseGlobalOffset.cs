using UnityEngine;
using System.Collections;
using BeatGame.Logic.Managers;

namespace BeatGame.Utility
{
    public class UseGlobalOffset : MonoBehaviour
    {
        private void Awake()
        {
            transform.position += (Vector3)SettingsManager.GlobalOffset;
        }
    }
}