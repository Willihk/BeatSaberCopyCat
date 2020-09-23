using System;

namespace BeatGame.Data
{
    [Serializable]
    public struct SongSpawningInfo
    {
        public float BPM;
     
        public float NoteJumpSpeed;
        public float NoteJumpStartBeatOffset;

        public float SecondEquivalentOfBeat;
        public float HalfJumpDuration;

        public float JumpDistance;
    }
}