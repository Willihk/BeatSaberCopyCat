﻿using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
namespace BeatGame.Logic.Managers
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance;

        public static float3 LineOffset = new float3(.8f, .8f, 0);

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}