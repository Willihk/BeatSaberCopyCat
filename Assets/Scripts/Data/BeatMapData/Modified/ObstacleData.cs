using Unity.Entities;
using Unity.Mathematics;

namespace BeatGame.Data.Map.Modified
{
    public struct ObstacleData
    {
        public double Time { get; set; }

        public TransformData TransformData;

        public float3 Color;
    }
}