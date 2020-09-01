using System;
namespace BeatGame.Data.Score
{
    [Serializable]
    public struct HighScoreData : IEquatable<HighScoreData>
    {
        public string SongName;
        public string LevelAuthor;
        public string Difficulty;
        public int Score;

        public bool Equals(HighScoreData other)
        {
            return SongName.Equals(other.SongName) && LevelAuthor.Equals(other.LevelAuthor) && Difficulty.Equals(other.Difficulty);
        }

        public override string ToString()
        {
            return SongName + LevelAuthor + Difficulty;
        }
    }
}