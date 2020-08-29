using BeatGame.Utility.Json;
using Newtonsoft.Json;
using Unity.Mathematics;

namespace BeatGame.Data
{
    public struct CustomSpawnedObjectData
    {
        // Last value of float4 is 1 if it is defined

        [JsonProperty("_position"), JsonConverter(typeof(Float4ConverterWithValidation))]
        public float4 Position { get; set; }

        [JsonProperty("_scale"), JsonConverter(typeof(Float4ConverterWithValidation))]
        public float4 Scale { get; set; }

        [JsonProperty("_color"), JsonConverter(typeof(Float4ConverterWithValidation))]
        public float4 Color { get; set; }

        [JsonProperty("_rotation", NullValueHandling = NullValueHandling.Ignore)]
        public float WorldRotation { get; set; }

        [JsonProperty("_localRotation"), JsonConverter(typeof(Float4ConverterWithValidation))]
        public float4 LocalRotation { get; set; }
    }
}