using UnityEngine;
using System.Collections;
using BeatGame.Utility.Json;
using Newtonsoft.Json;
using Unity.Mathematics;

namespace BeatGame.Data.Map.Raw
{
    public struct CustomEventData
    {
        [JsonProperty("_color"), JsonConverter(typeof(Float4ConverterWithValidation))]
        public float4 Color { get; set; }

        [JsonProperty("_propID")]
        public int PropID { get; set; }
    }
}