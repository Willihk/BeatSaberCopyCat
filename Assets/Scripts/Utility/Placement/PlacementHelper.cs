using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using BeatGame.Data;

namespace BeatGame.Utility
{
    public class PlacementHelper
    {
        public static NoteData ConvertNoteDataWithVanillaMethod(RawNoteData rawNoteData, float3 lineOffset)
        {
            float3 euler = new float3();
            switch ((CutDirection)rawNoteData.CutDirection)
            {
                case CutDirection.Upwards:
                    euler = new float3(0, 0, 180);
                    break;
                case CutDirection.Downwards:
                    break;
                case CutDirection.TowardsLeft:
                    euler = new float3(0, 0, -90);
                    break;
                case CutDirection.TowardsRight:
                    euler = new float3(0, 0, 90);
                    break;
                case CutDirection.TowardsTopLeft:
                    euler = new float3(0, 0, -135);
                    break;
                case CutDirection.TowardsTopRight:
                    euler = new float3(0, 0, 135);
                    break;
                case CutDirection.TowardsBottomLeft:
                    euler = new float3(0, 0, -45);
                    break;
                case CutDirection.TowardsBottomRight:
                    euler = new float3(0, 0, 45);
                    break;
                case CutDirection.Any:
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
                    LocalRotation = quaternion.Euler(euler),
                },
            };

            return note;
        }

        public static NoteData ConvertNoteDataWithNoodleExtensionsMethod(RawNoteData rawNoteData, float3 lineOffset)
        {
            var note = ConvertNoteDataWithVanillaMethod(rawNoteData, lineOffset);

            note.TransformData = GetTransformDataWithNoodle(note.TransformData, rawNoteData.CustomData, 0, 0);

            return note;
        }

        public static ObstacleData ConvertObstacleDataWithVanillaMethod(RawObstacleData rawObstacleData, float3 lineOffset, float jumpSpeed, float secondEquivalentOfBeat)
        {
            float4x4 scale = new float4x4
            {
                c0 = new float4(rawObstacleData.Width, 0, 0, 0),
                c1 = new float4(0, rawObstacleData.Type == 0 ? lineOffset.y * 3 : lineOffset.y * 2, 0, 0),
                c2 = new float4(0, 0, ConvertDurationToZScale((float)rawObstacleData.Duration, jumpSpeed, secondEquivalentOfBeat), 0),
                c3 = new float4(0, 0, 0, 1)
            };
            float lineIndex = rawObstacleData.LineIndex + (rawObstacleData.Width / 2);
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
                    Position = GetVanillaPosition(lineIndex, lineLayer, lineOffset),
                    Scale = scale,
                },
            };
        }

        public static ObstacleData ConvertObstacleDataWithNoodleExtensionsMethod(RawObstacleData rawData, float3 lineOffset, float jumpSpeed, float secondEquivalentOfBeat)
        {
            var obstacle = ConvertObstacleDataWithVanillaMethod(rawData, lineOffset, jumpSpeed, secondEquivalentOfBeat);

            obstacle.TransformData = GetTransformDataWithNoodle(obstacle.TransformData, rawData.CustomData, jumpSpeed, secondEquivalentOfBeat);

            var temp = obstacle.TransformData;
            temp.Position += new float3(obstacle.TransformData.Scale.c0.x / 2 + 1.3f, obstacle.TransformData.Scale.c1.y / 2, 0);
            obstacle.TransformData = temp;

            return obstacle;
        }


        public static float3 GetVanillaPosition(float lineIndex, float lineLayer, float3 lineOffset)
        {
            return new float3(lineIndex * lineOffset.x - 1.3f, lineLayer * lineOffset.y, 0);
        }

        public static TransformData GetTransformDataWithNoodle(TransformData transformData, CustomData customData, float jumpSpeed, float secondEquivalentOfBeat)
        {
            if (customData.Position.w != 0)
            {
                transformData.Position = GetVanillaPosition(customData.Position.x, customData.Position.y, new float3(.8f, .8f, 0));
            }
            if (customData.Scale.w != 0)
            {
                transformData.Scale = new float4x4
                {
                    c0 = new float4(customData.Scale.x, 0, 0, 0),
                    c1 = new float4(0, customData.Scale.y, 0, 0),
                    c2 = new float4(0, 0, ConvertDurationToZScale(customData.Scale.z, jumpSpeed, secondEquivalentOfBeat), 0),
                    c3 = new float4(0, 0, 0, 1)
                };
            }
            if (customData.LocalRotation.w != 0)
            {
                transformData.LocalRotation = quaternion.Euler(customData.LocalRotation.xyz);
            }

            return transformData;
        }

        public static float ConvertDurationToZScale(float duration, float jumpSpeed, float secondEquivalentOfBeat)
        {
            return duration * secondEquivalentOfBeat * jumpSpeed;
        }
    }
}