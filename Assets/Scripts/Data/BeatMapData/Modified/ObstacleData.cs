using Unity.Entities;
using Unity.Mathematics;

namespace BeatGame.Data
{
    public struct ObstacleData
    {
        public double Time { get; set; }

        public TransformData TransformData;

        public float3 Color;
    }
}