using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Transforms;
using Unity.Rendering;

public class NoteSpawningSystem : SystemBase
{
    public NativeList<NoteSpawnData> notesToSpawn;

    private const float SpawnTimeOffset = 5;

    // Needs to be here to run the system
    EntityQuery defaultQuery;

    float3 spawnPointOffset = new float3(1, 1, 0);

    float3[] spawnPoints = new float3[12];

    float currentTime;

    Material redMaterial;

    protected override void OnCreate()
    {
        redMaterial = Resources.Load<Material>("Materials/Note/Emissive/Red Emissive");
        notesToSpawn = new NativeList<NoteSpawnData>(Allocator.Persistent);

        defaultQuery = GetEntityQuery(new EntityQueryDesc { All = new ComponentType[] { typeof(Entity) } });

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                spawnPoints[y * 4 + x] = new float3(x * spawnPointOffset.x, y * spawnPointOffset.y, 0);
            }
        }
    }

    protected override void OnDestroy()
    {
        notesToSpawn.Dispose();
    }

    protected override void OnUpdate()
    {
        if (GameManager.Instance.IsPlaying)
        {
            currentTime += Time.DeltaTime;
            SpawnNeededNotes();
        }
    }

    void SpawnNeededNotes()
    {
        if (notesToSpawn.IsCreated == false)
            return;

        for (int i = 0; i < notesToSpawn.Length; i++)
        {
            var note = notesToSpawn[i];

            if (note.Time - SpawnTimeOffset <= currentTime)
            {
                SpawnNote(note);
                notesToSpawn.RemoveAtSwapBack(i);
                i--;
            }
        }
    }

    void SpawnNote(NoteSpawnData note)
    {
        var noteEntity = EntityPrefabManager.Instance.SpawnEntityPrefab("Note");
        EntityManager.SetComponentData(noteEntity, new Translation { Value = (GetSpawnPosition(note.LineIndex, note.LineLayer) + new float3(0, 0, GetNeededOffset(note.Time))) });

        float3 euler = float3.zero;

        switch ((CutDirection)note.CutDirection)
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
                //visualTransform.localScale = new Vector3(0.05f, 0.1f, 0.1f);
                //visualTransform.position = new Vector3(0, 0, visualTransform.position.z);
                break;
            default:
                break;
        }

        EntityManager.SetComponentData(noteEntity, new Rotation { Value = quaternion.Euler(euler) });

        if (note.Type == 0)
        {
            var linkedGroup = EntityManager.GetBuffer<LinkedEntityGroup>(noteEntity);

            var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(linkedGroup[2].Value);
            renderMesh.material = redMaterial;
            EntityManager.SetSharedComponentData(linkedGroup[2].Value, renderMesh);
        }
    }

    float3 GetSpawnPosition(int lineIndex, int lineLayer)
    {
        return spawnPoints[lineLayer * 4 + lineIndex];
    }

    float GetNeededOffset(double timeToSpawn)
    {
        return (float)(SpawnTimeOffset * CurrentSongDataManager.Instance.SelectedDifficultyMap.NoteJumpMovementSpeed + CurrentSongDataManager.Instance.SelectedDifficultyMap.NoteJumpStartBeatOffset);
    }
}
