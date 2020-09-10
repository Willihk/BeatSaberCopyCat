using MessagePack;
using Unity.Entities;
using Unity.Mathematics;

namespace BeatGame.Data.Map.Modified
{
    [MessagePackObject]
    public struct NoteData
    {
        [Key(0)]
        public double Time { get; set; }

        [Key(1)]
        public TransformData TransformData;

        [Key(2)]
        public int Type { get; set; }

        [Key(3)]
        public int CutDirection { get; set; }

        [Key(4)]
        public float3 Color { get; set; }
    }
}