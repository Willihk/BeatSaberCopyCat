using BeatGame.Data.Saber;
using BeatGame.Logic.Saber;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Burst;
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

    NativeQueue<SaberNoteHitData> detections;

    EntityQuery noteQuery;

    JobHandle job;

    protected override void OnCreate()
    {
        detections = new NativeQueue<SaberNoteHitData>(Allocator.Persistent);
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
            return;

        saberDatas.Clear();

        for (int i = 0; i < registeredControllers.Count; i++)
        {
            saberDatas.Add(new SaberData
            {
                AffectsNoteType = registeredControllers[i].affectsNoteType,
                Forward = registeredControllers[i].transform.forward,
                Position = registeredControllers[i].transform.position,
                Length = registeredControllers[i].saberLength
            });
        }

        job.Complete();
        while (detections.TryDequeue(out SaberNoteHitData hit))
        {
            for (int i = 0; i < registeredControllers.Count; i++)
            {
                if (registeredControllers[i].affectsNoteType == hit.Note.Type)
                {
                    registeredControllers[i].RegisterHit(hit);
                }
            }
        }

        var newJob = new DetectionJob
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
        job = newJob.ScheduleParallel(noteQuery, Dependency);
        Dependency = JobHandle.CombineDependencies(Dependency, job);
    }

    [BurstCompile]
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

        public NativeQueue<SaberNoteHitData>.ParallelWriter HitDetections;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityType);
            NativeArray<WorldRenderBounds> renderBounds = chunk.GetNativeArray(WorldRenderBoundsType);
            NativeArray<Note> notes = chunk.GetNativeArray(NoteType);

            NativeArray<Translation> translations = chunk.GetNativeArray(TranslationType);
            NativeArray<Rotation> rotations = chunk.GetNativeArray(RotationType);

            for (int i = 0; i < chunk.Count; i++)
            {
                bool hitNote = false;
                for (int offsetIndex = 0; offsetIndex < RaycastOffsets.Length; offsetIndex++)
                {
                    for (int saberIndex = 0; saberIndex < SaberDatas.Length; saberIndex++)
                    {
                        if (renderBounds[i].Value.ToBounds().IntersectRay(new Ray(SaberDatas[saberIndex].Position + RaycastOffsets[offsetIndex], SaberDatas[saberIndex].Forward), out float distance)
                            && distance <= SaberDatas[saberIndex].Length
                            && notes[i].Type == SaberDatas[saberIndex].AffectsNoteType)
                        {
                            HitDetections.Enqueue(new SaberNoteHitData
                            {
                                Note = notes[i],
                                Entity = entities[i],
                                Position = translations[i].Value,
                                Rotation = rotations[i].Value
                            });
                            hitNote = true;
                            break;
                        }
                    }
                    if (hitNote)
                        break;
                }
            }
        }
    }

    struct SaberData
    {
        public float Length;
        public float AffectsNoteType;
        public float3 Position;
        public float3 Forward;
    }
}
