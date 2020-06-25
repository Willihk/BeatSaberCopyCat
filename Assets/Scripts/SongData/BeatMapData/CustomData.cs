using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CustomData
{
    [JsonProperty("_position")]
    public double[] Position { get; set; }

    [JsonProperty("_scale")]
    public double[] Scale { get; set; }

    [JsonProperty("_color")]
    public double[] Color { get; set; }

    [JsonProperty("_localRotation", NullValueHandling = NullValueHandling.Ignore)]
    public long[] LocalRotation { get; set; }
}

