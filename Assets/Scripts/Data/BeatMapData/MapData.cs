using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BeatGame.Data
{
    [Serializable]
    public struct MapData
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
        public EventData[] Events { get; set; }

        public NoteData[] Notes { get; set; }

        public ObstacleData[] Obstacles { get; set; }
    }
}