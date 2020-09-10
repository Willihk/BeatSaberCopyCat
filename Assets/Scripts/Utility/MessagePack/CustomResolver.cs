using MessagePack;
using MessagePack.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

namespace BeatGame.MessagePack
{
    public class CustomResolver : IFormatterResolver
    {
        public static readonly CustomResolver Instance = new CustomResolver();

        private CustomResolver()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                Formatter = (IMessagePackFormatter<T>)UnityResolveryResolverGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }

    public static class UnityResolveryResolverGetFormatterHelper
    {
        private static readonly Dictionary<Type, object> FormatterMap = new Dictionary<Type, object>()
        {
            // standard
            { typeof(float3), new Float3Formatter() },
            { typeof(float4), new Float4Formatter() },
            { typeof(quaternion), new QuaternionFormatter() },
            { typeof(float4x4), new Float4x4Formatter() },
   

            // standard + array
            { typeof(float3[]), new ArrayFormatter<float3>() },
            { typeof(float4[]), new ArrayFormatter<float4>() },
            { typeof(quaternion[]), new ArrayFormatter<quaternion>() },
            { typeof(float4x4[]), new ArrayFormatter<float4x4>() },
       

            // standard + list
            //{ typeof(List<Vector2>), new ListFormatter<Vector2>() },
        };

        internal static object GetFormatter(Type t)
        {
            object formatter;
            if (FormatterMap.TryGetValue(t, out formatter))
            {
                return formatter;
            }

            return null;
        }
    }
}
