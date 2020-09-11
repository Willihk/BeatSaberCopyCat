using Unity.Mathematics;
using BeatGame.Data;
using BeatGame.Data.Map.Modified;
using BeatGame.Data.Map.Raw;
using BeatGame.Data.Map;

namespace BeatGame.Utility
{
    public class PlacementHelper
    {
        public static NoteData ConvertNoteDataWithVanillaMethod(RawNoteData rawNoteData, float3 lineOffset)
        {
            float3 euler = new float3();
            quaternion rotation = new quaternion();
            switch ((CutDirection)rawNoteData.CutDirection)
            {
                case CutDirection.Upwards:
                    euler = new float3(0, 0, 180);
                    rotation = new quaternion(0, 0, 1, 0);
                    break;
                case CutDirection.Downwards:
                    rotation = new quaternion(0, 0, 0.0008726948f, 0.9999996f);
                    break;
                case CutDirection.TowardsLeft:
                    euler = new float3(0, 0, -90);
                    rotation = new quaternion(0, 0, -0.7071068f, 0.7071068f);
                    break;
                case CutDirection.TowardsRight:
                    euler = new float3(0, 0, 90);
                    rotation = new quaternion(0, 0, 0.7071068f, 0.7071068f);
                    break;
                case CutDirection.TowardsTopLeft:
                    euler = new float3(0, 0, -135);
                    rotation = new quaternion(0, 0, -0.9238796f, 0.3826833f);
                    break;
                case CutDirection.TowardsTopRight:
                    euler = new float3(0, 0, 135);
                    rotation = new quaternion(0, 0, 0.9238796f, 0.3826834f);
                    break;
                case CutDirection.TowardsBottomLeft:
                    euler = new float3(0, 0, -45);
                    rotation = new quaternion(0, 0, -0.3826834f, 0.9238796f);
                    break;
                case CutDirection.TowardsBottomRight:
                    euler = new float3(0, 0, 45);
                    rotation = new quaternion(0, 0, 0.3826834f, 0.9238796f);
                    break;
                case CutDirection.Any:
                    rotation = new quaternion(0, 0, 0.0008726948f, 0.9999996f);
                    break;
                default:
                    break;
            }

            var note = new NoteData
            {
                Time = rawNoteData.Time,
                CutDirection = rawNoteData.CutDirection,
                Type = rawNoteData.Type,
                TransformData = new TransformData
                {
                    Position = GetVanillaPosition(rawNoteData.LineIndex, rawNoteData.LineLayer, lineOffset),
                    LocalRotation = rotation,
                },
            };

            return note;
        }

        public static ObstacleData ConvertObstacleDataWithVanillaMethod(RawObstacleData rawObstacleData, float jumpSpeed, float secondEquivalentOfBeat, float3 lineOffset)
        {
            float4x4 scale = new float4x4
            {
                c0 = new float4(rawObstacleData.Width, 0, 0, 0),
                c1 = new float4(0, rawObstacleData.Type == 0 ? lineOffset.y * 3 : lineOffset.y * 2, 0, 0),
                c2 = new float4(0, 0, ConvertDurationToZScale((float)rawObstacleData.Duration, jumpSpeed, secondEquivalentOfBeat) / 2, 0),
                c3 = new float4(0, 0, 0, 1)
            };
            scale.c0.x *= lineOffset.x;

            float lineIndex = rawObstacleData.LineIndex + (scale.c0.x / 2);
            float lineLayer = 0;

            if (rawObstacleData.Type == 0)
            {
                lineLayer = 1;
            }
            else if (rawObstacleData.Type == 1)
            {
                lineLayer = 2;
            }

            return new ObstacleData
            {
                Time = rawObstacleData.Time,
                TransformData = new TransformData
                {
                    Position = GetVanillaPosition(lineIndex, lineLayer, lineOffset) + new float3(0, 0, scale.c2.z),
                    Scale = scale,
                },
            };
        }

        public static float3 GetVanillaPosition(float lineIndex, float lineLayer, float3 lineOffset)
        {
            return new float3(lineIndex * lineOffset.x - (lineOffset.x * 1.5f), lineLayer * lineOffset.y, 0);
        }

        public static float ConvertDurationToZScale(float duration, float jumpSpeed, float secondEquivalentOfBeat)
        {
            return duration * secondEquivalentOfBeat * jumpSpeed;
        }
    }
}