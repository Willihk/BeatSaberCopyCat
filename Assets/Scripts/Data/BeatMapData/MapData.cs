using BeatGame.Data.Map.Modified;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BeatGame.Data.Map
{
    [Serializable]
    [MessagePackObject]
    public struct MapData
    {
        [Serializable]
        [MessagePackObject]
        public struct BPMChanx
        {
            [Key(0)]
            public double _BPM { get; set; }
            [Key(1)]
            public double _time { get; set; }
            [Key(2)]
            public int _beatsPerBar { get; set; }
            [Key(3)]
            public int _metronomeOffset { get; set; }

        }

        [Serializable]
        [MessagePackObject]
        public struct CustomData
        {
            [Key(0)]
            public double _time { get; set; }
            [Key(1)]
            public IList<BPMChanx> _BPMChanges { get; set; }
        }

        [Key(0)]
        public string _version { get; set; }
        [Key(1)]
        public CustomData _customData { get; set; }

        [Key(2)]
        public EventData[] Events { get; set; }

        [Key(3)]
        public NoteData[] Notes { get; set; }

        [Key(4)]
        public ObstacleData[] Obstacles { get; set; }
    }
}