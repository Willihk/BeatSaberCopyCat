using BeatGame.Data.Map.Modified;
using BeatGame.Data.Map.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace BeatGame.Utility.ModSupport
{
    public static class MappingExtensions
    {
        public static TransformData ConvertTransform(TransformData transformData, CustomSpawnedObjectData customData, float jumpSpeed, float secondEquivalentOfBeat, float3 lineOffset)
        {
            if (customData.NoteJumpSpeed == null)
            {
                transformData.Speed = jumpSpeed;
            }
            else
            {
                transformData.Speed = (float)customData.NoteJumpSpeed;
            }

            if (customData.Scale.w != 0)
            {
                transformData.Scale = new float4x4
                {
                    c0 = new float4(customData.Scale.x * lineOffset.x, 0, 0, 0),
                    c1 = new float4(0, customData.Scale.y * lineOffset.y, 0, 0),
                    c2 = new float4(0, 0, PlacementHelper.ConvertDurationToZScale(customData.Scale.z, jumpSpeed, secondEquivalentOfBeat), 0),
                    c3 = new float4(0, 0, 0, 1)
                };
            }
            if (customData.LocalRotation.w != 0)
            {
                transformData.LocalRotation = Quaternion.Euler(customData.LocalRotation.xyz);
            }
            if (customData.WorldRotation != 0)
            {
                transformData.WorldRotation = customData.WorldRotation;
            }
            if (customData.Position.w != 0)
            {
                transformData.Position = PlacementHelper.GetVanillaPosition(customData.Position.x, customData.Position.y, lineOffset);
            }

            return transformData;
        }

        public static float3 GetPosition(int lineIndex, int lineLayer, float3 lineOffset)
        {
            float3 position = new float3();

            if (lineIndex >= 1000 || lineIndex <= -1000)
            {
                if (lineIndex <= -1000)
                    lineIndex += 1000;
                else
                    lineIndex -= 1000;

                position.x = lineIndex / 1000f;
            }
            else
            {
                position.x = lineIndex;
            }

            if (lineLayer >= 1000 || lineLayer <= -1000)
            {
                if (lineLayer <= -1000)
                    lineLayer += 1000;
                else
                    lineLayer -= 1000;

                position.y = lineLayer / 1000f;
            }
            else
            {
                position.y = lineLayer;
            }

            return PlacementHelper.GetVanillaPosition(position.x, position.y, lineOffset);
        }


        public static NoteData ConvertNoteData(NoteData note, RawNoteData rawNoteData, float3 lineOffset)
        {
            float3 position = GetPosition(rawNoteData.LineIndex, rawNoteData.LineLayer, lineOffset);
            note.TransformData.Position = position;

            if (rawNoteData.CutDirection >= 1000)
            {
                note.TransformData.LocalRotation = Quaternion.Euler(0, 0, -(rawNoteData.CutDirection - 1000));
            }

            return note;
        }


        public static ObstacleData ConvertObstacleData(ObstacleData obstacle, RawObstacleData rawData, float jumpSpeed, float secondEquivalentOfBeat, float3 lineOffset)
        {
            obstacle.TransformData.Position = GetPosition(rawData.LineIndex, 0, lineOffset);

            float height = 0;
            float startHeight = 0;

            if (rawData.Type >= 40001 && rawData.Type <= 4005000)
            {
                int type = rawData.Type - 4001;
                height = type / 1000;
                startHeight = type % 1000;

                float normalHeight = lineOffset.y * 2f;

                obstacle.TransformData.Position.y = startHeight / 1000f * normalHeight;
            }
            else if (rawData.Type >= 1000)
            {
                height = rawData.Type;
                height -= 1000;
            }

            obstacle.TransformData.Scale.c1.y = height / 1000f;

            if (rawData.Type == 0)
                obstacle.TransformData.Position.y = 1;
            else if (rawData.Type == 1)
                obstacle.TransformData.Position.y = 2;

            if (rawData.Width >= 1000)
                obstacle.TransformData.Scale.c0.x = (rawData.Width - 1000f) / 1000f;
            else
                obstacle.TransformData.Scale.c0.x = rawData.Width;

            //obstacle.TransformData.Scale.c0.x *= lineOffset.x;
            obstacle.TransformData.Scale.c2 = new float4(0, 0, PlacementHelper.ConvertDurationToZScale((float)rawData.Duration, jumpSpeed, secondEquivalentOfBeat) / 2, 0);

            obstacle.TransformData.Scale.c2.z = PlacementHelper.ConvertDurationToZScale((float)rawData.Duration, jumpSpeed, secondEquivalentOfBeat) / 2;
            obstacle.TransformData.Position += new float3(lineOffset.x * 1.6f, obstacle.TransformData.Scale.c1.y / 2, obstacle.TransformData.Scale.c2.z / 2);

            return obstacle;
        }
    }
}
