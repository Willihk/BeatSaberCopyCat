using MessagePack;
using Unity.Entities;
using Unity.Mathematics;

namespace BeatGame.Data.Map.Modified
{
    [MessagePackObject]
    public struct ObstacleData
    {
        [Key(0)]
        public double Time { get; set; }

        [Key(1)]
        public TransformData TransformData;

        [Key(2)]
        public float3 Color;
    }
}