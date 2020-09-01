using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using BeatGame.Data;
using BeatGame.Data.Map.Raw;

namespace BeatGame.Utility.ModSupport
{
    public class ChromaSupport
    {
        public static float3 GetColorForObstacle(CustomSpawnedObjectData data)
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

        public static float3 GetColorForEvent(RawEventData data)
        {
            if (data.CustomData.Color.w != 0)
            {
                return data.CustomData.Color.xyz;
            }
            else
            {
                if (data.Value > 4)
                {
                    return new float3(0, 0.7035143f, 1);
                }
                else
                {
                    return new float3(1, 0, 0);
                }
            }
        }
    }
}