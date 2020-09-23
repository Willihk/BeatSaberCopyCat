using Unity.Entities;
using Unity.Mathematics;

namespace BeatGame.Data.Saber
{
    public struct SaberNoteHitData
    {
        public Entity Entity;
        public Note Note;
        public float3 Position;
        public quaternion Rotation;
    }
}