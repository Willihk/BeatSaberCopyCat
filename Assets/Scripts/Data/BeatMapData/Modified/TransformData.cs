using Unity.Mathematics;

namespace BeatGame.Data
{
    public struct TransformData
    {
        public float3 Position { get; set; }

        public float4x4 Scale { get; set; }

        public quaternion LocalRotation { get; set; }
    }
}