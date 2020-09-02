using Unity.Entities;
using Unity.Mathematics;

namespace BeatGame.Data.Map.Modified
{
    public struct NoteData
    {
        public double Time { get; set; }

        public TransformData TransformData;

        public int Type { get; set; }

        public int CutDirection { get; set; }

        public float3 Color { get; set; }
    }
}