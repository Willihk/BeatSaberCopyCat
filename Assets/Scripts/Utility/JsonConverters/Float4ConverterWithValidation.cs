using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using Unity.Mathematics;

public class Float4ConverterWithValidation : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException("Not implemented yet");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return string.Empty;
        }
        else if (reader.TokenType == JsonToken.String)
        {
            return serializer.Deserialize(reader, objectType);
        }
        else
        {
            JToken obj = JArray.Load(reader);

            var values = ((JArray)obj).ToObject<double[]>();

            float4 result = new float4(0,0,0, 1);

            result.x = (float)values[0];
            result.y = (float)values[1];
            if (values.Length > 2)
                result.z = (float)values[2];

            return result;
        }
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
