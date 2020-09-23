using BeatGame.Logic.Saber;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class SaberHitDetectionSystem : SystemBase
{
    List<SaberController> registeredControllers = new List<SaberController>();

    NativeArray<float3> raycastOffsets;
    NativeList<SaberData> saberDatas;

    NativeQueue<HitData> detections;

    EntityQuery noteQuery;

    JobHandle job;

    protected override void OnCreate()
    {
        detections = new NativeQueue<HitData>(Allocator.Persistent);
        saberDatas = new NativeList<SaberData>(Allocator.Persistent);
        raycastOffsets = new NativeArray<float3>(5, Allocator.Persistent);

        noteQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(Note), typeof(WorldRenderBounds), typeof(Rotation), typeof(Translation) }
        });
    }

    protected override void OnDestroy()
    {
        detections.Dispose();
        saberDatas.Dispose();
        raycastOffsets.Dispose();
    }

    public void RegisterController(SaberController saberController)
    {
        for (int i = 0; i < saberController.raycastPoints.Length; i++)
        {
            raycastOffsets[i] = saberController.raycastPoints[i].localPosition;
        }

        registeredControllers.Add(saberController);
        Debug.Log("Registered controller");
    }

    protected override void OnUpdate()
    {
        if (registeredControllers.Count == 0)
        {
            Debug.Log("No registered controllers");
            return;
        }

        saberDatas.Clear();

        for (int i = 0; i < registeredControllers.Count; i++)
        {
            saberDatas.Add(new SaberData
            {
                Forward = registeredControllers[i].transform.forward,
                Position = registeredControllers[i].transform.position,
                Length = registeredControllers[i].saberLength
            });
        }

        var job = new DetectionJob
        {
            SaberDatas = saberDatas,
            RaycastOffsets = raycastOffsets,
            HitDetections = detections.AsParallelWriter(),
            NoteType = GetComponentTypeHandle<Note>(true),
            TranslationType = GetComponentTypeHandle<Translation>(true),
            RotationType = GetComponentTypeHandle<Rotation>(true),
            WorldRenderBoundsType = GetComponentTypeHandle<WorldRenderBounds>(true),
            EntityType = GetEntityTypeHandle(),
        };
        job.Schedule(noteQuery).Complete();

        while (detections.TryDequeue(out HitData hit))
        {
            for (int i = 0; i < registeredControllers.Count; i++)
            {
                if (registeredControllers[i].affectsNoteType == hit.Note.Type)
                {
                    Debug.Log(registeredControllers[i].affectsNoteType + " hit note type: " + hit.Note.Type);
                    registeredControllers[i].HandleHit(hit);
                    EntityManager.DestroyEntity(hit.Entity);
                }
            }
        }
        detections.Clear();
    }


    struct DetectionJob : IJobChunk
    {
        [ReadOnly]
        public NativeArray<SaberData> SaberDatas;

        [ReadOnly]
        public NativeArray<float3> RaycastOffsets;

        [ReadOnly]
        public ComponentTypeHandle<WorldRenderBounds> WorldRenderBoundsType;

        [ReadOnly]
        public ComponentTypeHandle<Note> NoteType;
        [ReadOnly]
        public ComponentTypeHandle<Translation> TranslationType;
        [ReadOnly]
        public ComponentTypeHandle<Rotation> RotationType;
        [ReadOnly]
        public EntityTypeHandle EntityType;

        public NativeQueue<HitData>.ParallelWriter HitDetections;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityType);
            NativeArray<WorldRenderBounds> renderBounds = chunk.GetNativeArray(WorldRenderBoundsType);
            NativeArray<Note> notes = chunk.GetNativeArray(NoteType);

            NativeArray<Translation> translations = chunk.GetNativeArray(TranslationType);
            NativeArray<Rotation> rotations = chunk.GetNativeArray(RotationType);

            for (int i = 0; i < chunk.Count; i++)
            {
                for (int offsetIndex = 0; offsetIndex < RaycastOffsets.Length; offsetIndex++)
                {
                    for (int saberIndex = 0; saberIndex < SaberDatas.Length; saberIndex++)
                    {
                        if (renderBounds[i].Value.ToBounds().IntersectRay(new Ray(SaberDatas[saberIndex].Position + RaycastOffsets[offsetIndex], SaberDatas[saberIndex].Forward), out float distance)
                            && distance <= SaberDatas[saberIndex].Length)
                        {
                            Debug.Log("Hit note " + distance.ToString());
                            HitDetections.Enqueue(new HitData
                            {
                                Note = notes[i],
                                Entity = entities[i],
                                Position = translations[i].Value,
                                Rotation = rotations[i].Value
                            });
                        }
                    }
                }
            }
        }
    }

   public struct HitData
    {
        public Entity Entity;
        public Note Note;
        public float3 Position;
        public quaternion Rotation;
    }

    struct SaberData
    {
        public float Length;
        public float3 Position;
        public float3 Forward;
    }
}
