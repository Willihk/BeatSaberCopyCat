using Unity.Mathematics;

namespace BeatGame.Data
{
    public struct TransformData
    {
        public float3 Position;

        public float4x4 Scale;

        public quaternion LocalRotation;

        public float WorldRotation;
    }
}