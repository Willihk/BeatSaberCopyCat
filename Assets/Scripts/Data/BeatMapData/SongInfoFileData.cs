using MessagePack;

namespace BeatGame.Data.Map
{
    [MessagePackObject]
    public struct SongInfoFileData
    {
        [Key("_version")]
        public string Version { get; set; }

        [Key("_songName")]
        public string SongName { get; set; }

        [Key("_songSubName")]
        public string SongSubName { get; set; }

        [Key("_songAuthorName")]
        public string SongAuthorName { get; set; }

        [Key("_levelAuthorName")]
        public string LevelAuthorName { get; set; }

        [Key("_beatsPerMinute")]
        public float BeatsPerMinute { get; set; }

        [Key("_shuffle")]
        public float Shuffle { get; set; }

        [Key("_shufflePeriod")]
        public double ShufflePeriod { get; set; }

        [Key("_previewStartTime")]
        public double PreviewStartTime { get; set; }

        [Key("_previewDuration")]
        public float PreviewDuration { get; set; }

        [Key("_songFilename")]
        public string SongFilename { get; set; }

        [Key("_coverImageFilename")]
        public string CoverImageFilename { get; set; }

        [Key("_environmentName")]
        public string EnvironmentName { get; set; }

        [Key("_songTimeOffset")]
        public float SongTimeOffset { get; set; }

        [Key("_customData")]
        public InfoCustomData CustomData { get; set; }

        [Key("_difficultyBeatmapSets")]
        public DifficultyBeatmapSet[] DifficultyBeatmapSets { get; set; }
    }

    [MessagePackObject]
    public struct InfoCustomData
    {
        [Key("_contributors")]
        public Contributor[] Contributors { get; set; }
    }

    [MessagePackObject]
    public struct Contributor
    {
        [Key("_role")]
        public string Role;

        [Key("_name")]
        public string Name;

        [Key("_iconPath")]
        public string IconPath;
    }

    [MessagePackObject]
    public struct DifficultyBeatmapSet
    {
        [Key("_beatmapCharacteristicName")]
        public string BeatmapCharacteristicName { get; set; }

        [Key("_difficultyBeatmaps")]
        public DifficultyBeatmap[] DifficultyBeatmaps { get; set; }
    }

    [MessagePackObject]
    public struct DifficultyBeatmap
    {
        [Key("_difficulty")]
        public string Difficulty { get; set; }

        [Key("_difficultyRank")]
        public float DifficultyRank { get; set; }

        [Key("_beatmapFilename")]
        public string BeatmapFilename { get; set; }

        [Key("_noteJumpMovementSpeed")]
        public float NoteJumpMovementSpeed { get; set; }

        [Key("_noteJumpStartBeatOffset")]
        public double NoteJumpStartBeatOffset { get; set; }

        [Key("_customData")]
        public DifficultyBeatmapCustomData CustomData { get; set; }
    }

    [MessagePackObject]
    public struct DifficultyBeatmapCustomData
    {
        [Key("_difficultyLabel")]
        public string DifficultyLabel { get; set; }

        [Key("_editorOffset")]
        public float EditorOffset { get; set; }

        [Key("_editorOldOffset")]
        public float EditorOldOffset { get; set; }

        [Key("_warnings")]
        public string[] Warnings { get; set; }

        [Key("_information")]
        public string[] Information { get; set; }

        [Key("_suggestions")]
        public string[] Suggestions { get; set; }

        [Key("_requirements")]
        public string[] Requirements { get; set; }
    }
}