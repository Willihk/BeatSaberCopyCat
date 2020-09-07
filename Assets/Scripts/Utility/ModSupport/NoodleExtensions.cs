using UnityEngine;
using System.Collections;
using BeatGame.Data;
using Unity.Mathematics;
using BeatGame.Utility;
using BeatGame.Logic.Managers;
using BeatGame.Data.Map.Modified;
using BeatGame.Data.Map.Raw;

namespace BeatGame.Utility.ModSupport
{
    public static class NoodleExtensions
    {
        public static TransformData ApplyNoodleExtensionsToTransform(TransformData transformData, CustomSpawnedObjectData customData, float jumpSpeed, float secondEquivalentOfBeat, float3 lineOffset)
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
                    c0 = new float4(customData.Scale.x, 0, 0, 0),
                    c1 = new float4(0, customData.Scale.y, 0, 0),
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

        public static NoteData ConvertNoteDataToNoodleExtensions(NoteData note, RawNoteData rawNoteData, float3 lineOffset)
        {
            note.TransformData = ApplyNoodleExtensionsToTransform(note.TransformData, rawNoteData.CustomData, 0, 0, lineOffset);

            return note;
        }


        public static ObstacleData ConvertObstacleDataToNoodleExtensions(ObstacleData obstacle, RawObstacleData rawData, float jumpSpeed, float secondEquivalentOfBeat, float3 lineOffset)
        {
            obstacle.TransformData = ApplyNoodleExtensionsToTransform(obstacle.TransformData, rawData.CustomData, jumpSpeed, secondEquivalentOfBeat, lineOffset);

            obstacle.TransformData.Scale.c2.z = PlacementHelper.ConvertDurationToZScale((float)rawData.Duration, jumpSpeed, secondEquivalentOfBeat) / 2;
            obstacle.TransformData.Position += new float3(obstacle.TransformData.Scale.c0.x / 2 + 1.5f, obstacle.TransformData.Scale.c1.y / 2, obstacle.TransformData.Scale.c2.z / 2);

            return obstacle;
        }
    }
}