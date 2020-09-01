using Newtonsoft.Json;

namespace BeatGame.Data.Map.Raw
{
    public struct RawEventData
    {
        [JsonProperty("_time")]
        public double Time { get; set; }

        [JsonProperty("_type")]
        public int Type { get; set; }
        /* 
         0 = Back Top Lasers
         1 = Track Ring Neons
         2 = Left Lasers
         3 = Right Lasers
         4 = Bottom/Back/side Lasers

         8 = Ring Rotation
         9 = Small Ring Zoom

        12 = Left Laser Speed
        13 = Right Laser Speed
        */

        [JsonProperty("_value")]
        public int Value { get; set; }

        [JsonProperty("_customData")]
        public CustomEventData CustomData {get; set;}
    }
}