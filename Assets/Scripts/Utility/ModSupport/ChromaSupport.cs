using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using BeatGame.Data;

namespace BeatGame.Utility.ModSupport
{
    public class ChromaSupport
    {
        public static float3 GetColor(CustomData data)
        {
            if (data.Color.w != 0)
            {
                return data.Color.xyz;
            }
            else
            {
                return new float3(1, 0, 0);
            }
        }
    }
}