using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

namespace BeatGame.Data
{
    public struct RawNoteData
    {
        [JsonProperty("_time")]
        public double Time { get; set; }

        [JsonProperty("_lineIndex")]
        public int LineIndex { get; set; }

        [JsonProperty("_lineLayer")]
        public int LineLayer { get; set; }

        [JsonProperty("_type")]
        public int Type { get; set; }

        [JsonProperty("_cutDirection")]
        public int CutDirection { get; set; }

        [JsonProperty("_customData", NullValueHandling = NullValueHandling.Ignore)]
        public CustomSpawnedObjectData CustomData { get; set; }
    }
}