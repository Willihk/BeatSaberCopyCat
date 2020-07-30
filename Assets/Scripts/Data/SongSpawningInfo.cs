using System;

namespace BeatGame.Data
{
    [Serializable]
    public struct SongSpawningInfo
    {
        public float BPM;
        public float NoteJumpSpeed;
        public float NoteJumpStartBeatOffset;

        public double SecondEquivalentOfBeat;
        public double HalfJumpDuration;

        public float JumpDistance;
        public float DistanceToMove;
    }
}