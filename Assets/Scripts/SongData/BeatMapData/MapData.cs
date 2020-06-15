using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapData
{
    [Serializable]
    public struct BPMChanx
    {

        public double _BPM { get; set; }
        public double _time { get; set; }
        public int _beatsPerBar { get; set; }
        public int _metronomeOffset { get; set; }

    }

    [Serializable]
    public struct CustomData
    {

        public double _time { get; set; }
        public IList<BPMChanx> _BPMChanges { get; set; }
    }

    public string _version { get; set; }
    public CustomData _customData { get; set; }

    [JsonProperty("_events")]
    public IList<EventData> Events { get; set; }

    [JsonProperty("_notes")]
    public IList<NoteSpawnData> Notes { get; set; }

    [JsonProperty("_obstacles")]
    public IList<ObstacleData> Obstacles { get; set; }
}
