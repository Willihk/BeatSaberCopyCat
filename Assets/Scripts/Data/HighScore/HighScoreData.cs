using MessagePack;
using System;
namespace BeatGame.Data.Score
{
    [Serializable]
    [MessagePackObject]
    public struct HighScoreData : IEquatable<HighScoreData>
    {
        [Key("SongName")]
        public string SongName;
        [Key("LevelAuthor")]
        public string LevelAuthor;
        [Key("DifficultySet")]
        public string DifficultySet;
        [Key("Difficulty")]
        public string Difficulty;
        [Key("Score")]
        public int Score;

        public bool Equals(HighScoreData other)
        {
            return other.SongName != null
                && SongName.Equals(other.SongName)
                && other.LevelAuthor != null
                && LevelAuthor.Equals(other.LevelAuthor)
                && other.Difficulty != null
                && Difficulty.Equals(other.Difficulty)
                && !string.IsNullOrEmpty(DifficultySet)
                && !string.IsNullOrEmpty(other.DifficultySet)
                && DifficultySet.Equals(other.DifficultySet);
        }

        public override string ToString()
        {
            return SongName + LevelAuthor + Difficulty;
        }
    }
}