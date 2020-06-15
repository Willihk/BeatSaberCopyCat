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

    // Needs to be here to run the system
    EntityQuery defaultQuery;

    Material redEmissiveMaterial;
    Material redMaterial;

    protected override void OnCreate()
    {
        redEmissiveMaterial = Resources.Load<Material>("Materials/Note/Emissive/Red Emissive");
        redMaterial = Resources.Load<Material>("Materials/Note/Note Red");
        notesToSpawn = new NativeList<NoteSpawnData>(Allocator.Persistent);

        defaultQuery = GetEntityQuery(new EntityQueryDesc { All = new ComponentType[] { typeof(Entity) } });
    }

    protected override void OnDestroy()
    {
        notesToSpawn.Dispose();
    }

    protected override void OnUpdate()
    {
        if (GameManager.Instance.IsPlaying)
        {
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

            if (note.Time - CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration <= GameManager.Instance.CurrentBeat)
            {
                if (note.Type == 3)
                {
                    SpawnBomb(note);
                }
                else
                {
                    SpawnNote(note);
                }
                notesToSpawn.RemoveAtSwapBack(i);
                i--;
            }
        }
    }

    void SpawnBomb(NoteSpawnData note)
    {
        var noteEntity = EntityPrefabManager.Instance.SpawnEntityPrefab("Bomb");
        EntityManager.SetComponentData(noteEntity, new Translation { Value = GetSpawnPosition(note.LineIndex, note.LineLayer) + new float3(0, 0, GetNeededOffset()) });

        EntityManager.SetComponentData(noteEntity, new Note { Data = note });

    }

    void SpawnNote(NoteSpawnData note)
    {
        var noteEntity = EntityPrefabManager.Instance.SpawnEntityPrefab("Note");
        EntityManager.SetComponentData(noteEntity, new Translation { Value = GetSpawnPosition(note.LineIndex, note.LineLayer) + new float3(0, 0, GetNeededOffset()) });

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
                var linkedGroup = EntityManager.GetBuffer<LinkedEntityGroup>(noteEntity);
                EntityManager.SetComponentData(linkedGroup[2].Value, new NonUniformScale { Value = new float3(0.05f, 0.1f, 0.1f) });
                EntityManager.SetComponentData(linkedGroup[2].Value, new Translation { Value = new float3(0, 0, -0.237f) });
                break;
            default:
                break;
        }

        EntityManager.SetComponentData(noteEntity, new Rotation { Value = quaternion.Euler(euler) });
        EntityManager.SetComponentData(noteEntity, new Note { Data = note });

        if (note.Type == 0)
        {
            var linkedGroup = EntityManager.GetBuffer<LinkedEntityGroup>(noteEntity);

            var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(linkedGroup[1].Value);
            renderMesh.material = redMaterial;
            EntityManager.SetSharedComponentData(linkedGroup[1].Value, renderMesh);

            // needs to be reassigned because of structural change
            linkedGroup = EntityManager.GetBuffer<LinkedEntityGroup>(noteEntity);

            renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(linkedGroup[2].Value);
            renderMesh.material = redEmissiveMaterial;
            EntityManager.SetSharedComponentData(linkedGroup[2].Value, renderMesh);
        }
    }

    float3 GetSpawnPosition(float lineIndex, float lineLayer)
    {
        return new float3(lineIndex * CurrentSongDataManager.Instance.SpawnPointOffset.x - 1.6f, lineLayer * CurrentSongDataManager.Instance.SpawnPointOffset.y, 0);
    }

    float GetNeededOffset()
    {
        return CurrentSongDataManager.Instance.SongSpawningInfo.JumpDistance;
    }
}
