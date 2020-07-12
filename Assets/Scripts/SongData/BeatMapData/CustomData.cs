using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

public struct CustomData
{
    [JsonProperty("_position"), JsonConverter(typeof(Float3Converter))]
    public float3 Position { get; set; }

    [JsonProperty("_scale"), JsonConverter(typeof(Float3Converter))]
    public float3 Scale { get; set; }

    [JsonProperty("_color"), JsonConverter(typeof(Float3Converter))]
    public float3 Color { get; set; }

    [JsonProperty("_localRotation", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(Float3Converter))]
    public float3 LocalRotation { get; set; }
}
