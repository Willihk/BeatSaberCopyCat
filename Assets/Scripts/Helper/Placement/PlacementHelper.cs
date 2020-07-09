using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class PlacementHelper
{
    public static NoteData ConvertNoteDataWithVanillaMethod(RawNoteData rawNoteData, float3 spawnPointOffset)
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
                Position = GetVanillaPosition(rawNoteData.LineIndex, rawNoteData.LineLayer, spawnPointOffset),
                LocalRotation = euler,
            },
        };

        return note;
    }

    public static NoteData ConvertNoteDataWithNoodleExtensionsMethod(RawNoteData rawNoteData, float3 spawnpointOffset)
    {
        var note = ConvertNoteDataWithVanillaMethod(rawNoteData, spawnpointOffset);

        note.TransformData = GetTransformDataWithNoodle(note.TransformData, rawNoteData.CustomData);

        return note;
    }

    public static ObstacleData ConvertObstacleDataWithVanillaMethod(RawObstacleData rawObstacleData, float3 spawnPointOffset)
    {
        float4x4 scale = new float4x4
        {
            c0 = new float4(rawObstacleData.Width, 0, 0, 0),
            c1 = new float4(0, rawObstacleData.Type == 0 ? spawnPointOffset.y * 3 : spawnPointOffset.y * 2, 0, 0),
            c2 = new float4(0, 0, (float)rawObstacleData.Duration, 0),
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
                Position = GetVanillaPosition(lineIndex, lineLayer, spawnPointOffset) + scale.c0.x / 2 + new float3(1.3f, 0, 0),
                Scale = new float3(scale.c0.x, scale.c1.y, scale.c2.z),
            },
        };
    }

    public static ObstacleData ConvertObstacleDataWithNoodleExtensionsMethod(RawObstacleData rawNoteData, float3 spawnpointOffset)
    {
        var note = ConvertObstacleDataWithVanillaMethod(rawNoteData, spawnpointOffset);

        note.TransformData = GetTransformDataWithNoodle(note.TransformData, rawNoteData.CustomData);

        var temp = note.TransformData;
        temp.Position += new float3(note.TransformData.Scale.x / 2 + 1.3f, note.TransformData.Scale.y / 2, 0);
        note.TransformData = temp;

        return note;
    }


    public static float3 GetVanillaPosition(float lineIndex, float lineLayer, float3 spawnPointOffset)
    {
        return new float3(lineIndex * spawnPointOffset.x - 1.3f, lineLayer * spawnPointOffset.y, 0);
    }

    public static TransformData GetTransformDataWithNoodle(TransformData transformData, CustomData customData)
    {
        if (customData == null)
            return transformData;

        if (customData.Position != null)
        {
            transformData.Position = GetVanillaPosition((float)customData.Position[0], (float)customData.Position[1], new float3(.8f, .8f, 0));
        }
        if (customData.Scale != null)
        {
            transformData.Scale = new float3(
              (float)customData.Scale[0],
              (float)customData.Scale[1],
              transformData.Scale.z);
        }
        if (customData.LocalRotation != null)
        {
            transformData.LocalRotation = new float3(
                 customData.LocalRotation[0],
                 customData.LocalRotation[1],
                 customData.LocalRotation[2]);
        }

        return transformData;
    }
}
