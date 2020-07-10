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
    public NativeList<NoteData> notesToSpawn;

    // Needs to be here to run the system
    EntityQuery defaultQuery;

    Material redEmissiveMaterial;
    Material redMaterial;

    protected override void OnCreate()
    {
        redEmissiveMaterial = Resources.Load<Material>("Materials/Note/Emissive/Red Emissive");
        redMaterial = Resources.Load<Material>("Materials/Note/Note Red");
        notesToSpawn = new NativeList<NoteData>(Allocator.Persistent);

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

    void SpawnBomb(NoteData note)
    {
        var noteEntity = EntityPrefabManager.Instance.SpawnEntityPrefab("Bomb");
        EntityManager.SetComponentData(noteEntity, new Translation { Value = note.TransformData.Position + new float3(0, 0, GetNeededOffset()) });

        EntityManager.SetComponentData(noteEntity, new Note { Type = note.Type, CutDirection = note.CutDirection });
    }

    void SpawnNote(NoteData note)
    {
        var noteEntity = EntityPrefabManager.Instance.SpawnEntityPrefab("Note");

        switch ((CutDirection)note.CutDirection)
        {
            case CutDirection.Any:
                var linkedGroup = EntityManager.GetBuffer<LinkedEntityGroup>(noteEntity);
                EntityManager.SetComponentData(linkedGroup[1].Value, new NonUniformScale { Value = new float3(0.05f, 0.1f, 0.1f) });
                EntityManager.SetComponentData(linkedGroup[1].Value, new Translation { Value = new float3(0, 0, -0.237f) });
                break;
            default:
                break;
        }


        var rotation = EntityManager.GetComponentData<Rotation>(noteEntity);

        EntityManager.SetComponentData(noteEntity, new Rotation { Value = math.mul(rotation.Value, quaternion.Euler(note.TransformData.LocalRotation)) });
        EntityManager.SetComponentData(noteEntity, new Translation { Value = note.TransformData.Position + new float3(0, 0, GetNeededOffset()) });

        EntityManager.SetComponentData(noteEntity, new Note { Type = note.Type, CutDirection = note.CutDirection });

        if (note.Type == 0)
        {
            var linkedGroup = EntityManager.GetBuffer<LinkedEntityGroup>(noteEntity);

            var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(linkedGroup[0].Value);
            renderMesh.material = redMaterial;
            EntityManager.SetSharedComponentData(linkedGroup[0].Value, renderMesh);

            // needs to be reassigned because of structural change
            linkedGroup = EntityManager.GetBuffer<LinkedEntityGroup>(noteEntity);

            renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(linkedGroup[1].Value);
            renderMesh.material = redEmissiveMaterial;
            EntityManager.SetSharedComponentData(linkedGroup[1].Value, renderMesh);
        }
    }

    float GetNeededOffset()
    {
        return CurrentSongDataManager.Instance.SongSpawningInfo.JumpDistance;
    }
}
