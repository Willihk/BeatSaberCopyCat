using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
namespace BeatGame.Logic.Managers
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance;

        public static float3 LineOffset = new float3(.8f, .8f, 0);
        public static float3 GlobalOffset = new float3(0, 0, -1);

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}