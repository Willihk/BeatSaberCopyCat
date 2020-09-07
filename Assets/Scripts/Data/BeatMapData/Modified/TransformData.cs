using Unity.Mathematics;

namespace BeatGame.Data.Map.Modified
{
    public struct TransformData
    {
        public float3 Position;

        public float4x4 Scale;

        public quaternion LocalRotation;

        public float WorldRotation;

        public float Speed;
    }
}