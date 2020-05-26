using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct EventData
{
    [JsonProperty("_time")]
    public double Time { get; set; }

    [JsonProperty("_type")]
    public int Type { get; set; }

    [JsonProperty("_value")]
    public int Value { get; set; }
}
