using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using Unity.Mathematics;

public class CustomDataConverter : JsonConverter
{

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException("Not implemented yet");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return new CustomData()
            {
                Position = new float3(-9999, -9999, -9999),
                Scale = new float3(-9999, -9999, -9999),
                LocalRotation = new float3(-9999, -9999, -9999),
                Color = new float3(-9999, -9999, -9999),
            };
        }
        else if (reader.TokenType == JsonToken.String)
        {
            return serializer.Deserialize(reader, objectType);
        }
        else
        {
            JToken obj = JObject.Load(reader);

            var customData = new CustomData()
            {
                Position = new float3(-9999, -9999, -9999),
                Scale = new float3(-9999, -9999, -9999),
                LocalRotation = new float3(-9999, -9999, -9999),
                Color = new float3(-9999, -9999, -9999),
            };

            if (obj["_position"] != null)
            {
                customData.Position = GetFloat3FromArray(obj["_position"].ToObject<double[]>());
            }
            if (obj["_scale"] != null)
            {
                customData.Scale = GetFloat3FromArray(obj["_scale"].ToObject<double[]>());
            }
            if (obj["_color"] != null)
            {
                customData.Color = GetFloat3FromArray(obj["_color"].ToObject<double[]>());
            }
            if (obj["_localRotation"] != null)
            {
                customData.LocalRotation = GetFloat3FromArray(obj["_localRotation"].ToObject<double[]>());
            }

            return customData;
        }
    }

    public float3 GetFloat3FromArray(double[] values)
    {
        float3 result = new float3(-9999, -9999, -9999);

        result.x = (float)values[0];
        result.y = (float)values[1];
        if (values.Length > 2)
            result.z = (float)values[2];

        return result;
    }

    public override bool CanWrite
    {
        get { return false; }
    }

    public override bool CanConvert(Type objectType)
    {
        return false;
    }
}
