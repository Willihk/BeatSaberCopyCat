using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

public struct CustomData
{
    // Using float4 last value to check if there is a value defined

    [JsonProperty("_position"), JsonConverter(typeof(Float4ConverterWithValidation))]
    public float4 Position { get; set; }

    [JsonProperty("_scale"), JsonConverter(typeof(Float4ConverterWithValidation))]
    public float4 Scale { get; set; }

    [JsonProperty("_color"), JsonConverter(typeof(Float4ConverterWithValidation))]
    public float4 Color { get; set; }

    [JsonProperty("_localRotation", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(Float4ConverterWithValidation))]
    public float4 LocalRotation { get; set; }
}
