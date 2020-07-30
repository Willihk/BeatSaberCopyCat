using Newtonsoft.Json;

namespace BeatGame.Data
{
    public struct RawObstacleData
    {
        [JsonProperty("_time")]
        public double Time { get; set; }

        [JsonProperty("_lineIndex")]
        public int LineIndex { get; set; }

        [JsonProperty("_type")]
        public int Type { get; set; }

        [JsonProperty("_duration")]
        public double Duration { get; set; }

        [JsonProperty("_width")]
        public int Width { get; set; }

        [JsonProperty("_customData")]
        public CustomData CustomData { get; set; }
    }
}