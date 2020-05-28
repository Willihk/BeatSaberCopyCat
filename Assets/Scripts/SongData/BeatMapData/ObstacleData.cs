using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct ObstacleData
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
}
