﻿using UnityEngine;
using System.Collections;
using BeatGame.Utility.Json;
using Newtonsoft.Json;
using Unity.Mathematics;

namespace BeatGame.Data
{
    public struct CustomEventData
    {
        [JsonProperty("_color"), JsonConverter(typeof(Float4ConverterWithValidation))]
        public float4 Color { get; set; }

        [JsonProperty("_propID")]
        public float4 PropID { get; set; }
    }
}