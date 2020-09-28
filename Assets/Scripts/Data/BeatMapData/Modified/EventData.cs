using MessagePack;
using Unity.Mathematics;

namespace BeatGame.Data.Map.Modified
{
    [MessagePackObject]
    public struct EventData
    {
        [Key(0)]
        public double Time;

        [Key(1)]
        public int Type;

        [Key(2)]
        public int Value;

        [Key(3)]
        public float4 Color;

        [Key(4)]
        public int PropID;
    }
}