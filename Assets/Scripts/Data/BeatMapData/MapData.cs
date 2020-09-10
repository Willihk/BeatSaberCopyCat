using BeatGame.Data.Map.Modified;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BeatGame.Data.Map
{
    [Serializable]
    [MessagePackObject(true)]
    public struct MapData
    {
        [Serializable]
        [MessagePackObject(true)]
        public struct BPMChanx
        {

            public double _BPM { get; set; }
            public double _time { get; set; }
            public int _beatsPerBar { get; set; }
            public int _metronomeOffset { get; set; }

        }

        [Serializable]
        [MessagePackObject(true)]
        public struct CustomData
        {

            public double _time { get; set; }
            public IList<BPMChanx> _BPMChanges { get; set; }
        }

        public string _version { get; set; }
        public CustomData _customData { get; set; }

        public EventData[] Events { get; set; }

        public NoteData[] Notes { get; set; }

        public ObstacleData[] Obstacles { get; set; }
    }
}