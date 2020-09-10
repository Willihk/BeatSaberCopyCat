using MessagePack;
using Unity.Mathematics;

namespace BeatGame.Data.Map.Modified
{
    [MessagePackObject]
    public struct TransformData
    {
        [Key(0)]
        public float3 Position;

        [Key(1)]
        public float4x4 Scale;

        [Key(2)]
        public quaternion LocalRotation;

        [Key(3)]
        public float WorldRotation;

        [Key(4)]
        public float Speed;
    }
}