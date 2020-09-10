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
    public sealed class Float4x4Formatter :IMessagePackFormatter<float4x4>
    {
        public void Serialize(ref MessagePackWriter writer, float4x4 value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.WriteArrayHeader(16);
            writer.Write(value.c0.x);
            writer.Write(value.c1.x);
            writer.Write(value.c2.x);
            writer.Write(value.c3.x);
            writer.Write(value.c0.y);
            writer.Write(value.c1.y);
            writer.Write(value.c2.y);
            writer.Write(value.c3.y);
            writer.Write(value.c0.z);
            writer.Write(value.c1.z);
            writer.Write(value.c1.z);
            writer.Write(value.c1.z);
            writer.Write(value.c0.w);
            writer.Write(value.c1.w);
            writer.Write(value.c2.w);
            writer.Write(value.c3.w);
        }

        public float4x4 Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.IsNil)
            {
                throw new InvalidOperationException("typecode is null, struct not supported");
            }

            var length = reader.ReadArrayHeader();
            var __m00__ = default(float);
            var __m10__ = default(float);
            var __m20__ = default(float);
            var __m30__ = default(float);
            var __m01__ = default(float);
            var __m11__ = default(float);
            var __m21__ = default(float);
            var __m31__ = default(float);
            var __m02__ = default(float);
            var __m12__ = default(float);
            var __m22__ = default(float);
            var __m32__ = default(float);
            var __m03__ = default(float);
            var __m13__ = default(float);
            var __m23__ = default(float);
            var __m33__ = default(float);
            for (int i = 0; i < length; i++)
            {
                var key = i;
                switch (key)
                {
                    case 0:
                        __m00__ = reader.ReadSingle();
                        break;
                    case 1:
                        __m10__ = reader.ReadSingle();
                        break;
                    case 2:
                        __m20__ = reader.ReadSingle();
                        break;
                    case 3:
                        __m30__ = reader.ReadSingle();
                        break;
                    case 4:
                        __m01__ = reader.ReadSingle();
                        break;
                    case 5:
                        __m11__ = reader.ReadSingle();
                        break;
                    case 6:
                        __m21__ = reader.ReadSingle();
                        break;
                    case 7:
                        __m31__ = reader.ReadSingle();
                        break;
                    case 8:
                        __m02__ = reader.ReadSingle();
                        break;
                    case 9:
                        __m12__ = reader.ReadSingle();
                        break;
                    case 10:
                        __m22__ = reader.ReadSingle();
                        break;
                    case 11:
                        __m32__ = reader.ReadSingle();
                        break;
                    case 12:
                        __m03__ = reader.ReadSingle();
                        break;
                    case 13:
                        __m13__ = reader.ReadSingle();
                        break;
                    case 14:
                        __m23__ = reader.ReadSingle();
                        break;
                    case 15:
                        __m33__ = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = default(float4x4);
            ____result.c0.x = __m00__;
            ____result.c1.x = __m10__;
            ____result.c2.x = __m20__;
            ____result.c3.x = __m30__;
            ____result.c0.y = __m01__;
            ____result.c1.y = __m11__;
            ____result.c2.y = __m21__;
            ____result.c3.y = __m31__;
            ____result.c0.z = __m02__;
            ____result.c1.z = __m12__;
            ____result.c2.z = __m22__;
            ____result.c3.z = __m32__;
            ____result.c0.w = __m03__;
            ____result.c1.w = __m13__;
            ____result.c2.w = __m23__;
            ____result.c3.w = __m33__;
            return ____result;
        }
    }
}
