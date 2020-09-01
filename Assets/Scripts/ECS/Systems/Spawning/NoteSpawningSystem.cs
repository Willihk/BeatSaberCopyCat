using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Transforms;
using Unity.Rendering;
using BeatGame.Data;
using BeatGame.Logic.Managers;
using Unity.Jobs;
using Unity.Burst;
using BeatGame.Data.Map.Modified;
using BeatGame.Data.Map;

public class NoteSpawningSystem : SystemBase
{
    public NativeList<NoteData> notesToSpawn;
    NativeArray<Entity> notePrefabs;


    protected override void OnCreate()
    {
        notesToSpawn = new NativeList<NoteData>(Allocator.Persistent);
    }

    protected override void OnStartRunning()
    {
        notePrefabs = new NativeArray<Entity>(5, Allocator.Persistent);
        notePrefabs[0] = EntityPrefabManager.Instance.GetEntityPrefab("Note Blue AnyDirection");
        notePrefabs[1] = EntityPrefabManager.Instance.GetEntityPrefab("Note Red AnyDirection");
        notePrefabs[2] = EntityPrefabManager.Instance.GetEntityPrefab("Note Blue");
        notePrefabs[3] = EntityPrefabManager.Instance.GetEntityPrefab("Note Red");
        notePrefabs[4] = EntityPrefabManager.Instance.GetEntityPrefab("Bomb");
    }

    protected override void OnDestroy()
    {
        notesToSpawn.Dispose();
        notePrefabs.Dispose();
    }

    protected override void OnUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPlaying)
        {
            EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            SpawnNotesJob job = new SpawnNotesJob
            {
                CommandBuffer = commandBuffer.AsParallelWriter(),
                Notes = notesToSpawn,
                NotePrefabs = notePrefabs,
                HeightOffset = SettingsManager.GlobalOffset.y,
                LastBeat = GameManager.Instance.LastBeat,
                CurrentBeat = (float)GameManager.Instance.CurrentBeat,
                JumpDistance = CurrentSongDataManager.Instance.SongSpawningInfo.JumpDistance,
                HalfJumpDuration = CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration
            };
            job.Schedule(notesToSpawn.Length, 64).Complete();

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }
    }

    [BurstCompile]
    struct SpawnNotesJob : IJobParallelFor
    {
        public EntityCommandBuffer.ParallelWriter CommandBuffer;
        [ReadOnly]
        public NativeList<NoteData> Notes;
        [ReadOnly]
        public float HeightOffset;
        [ReadOnly]
        public float JumpDistance;
        [ReadOnly]
        public float CurrentBeat;
        [ReadOnly]
        public double HalfJumpDuration;
        [ReadOnly]
        public double LastBeat;
        [ReadOnly]
        public NativeArray<Entity> NotePrefabs;

        public void Execute(int index)
        {
            if (Notes[index].Time - HalfJumpDuration <= CurrentBeat && Notes[index].Time - HalfJumpDuration >= LastBeat)
            {
                if (Notes[index].Type == 3)
                {
                    SpawnBomb(index);
                }
                else
                {
                    SpawnNote(index);
                }
            }
        }

        void SpawnBomb(int index)
        {
            var noteEntity = CommandBuffer.Instantiate(index, NotePrefabs[4]);

            CommandBuffer.SetComponent(index, noteEntity, new Translation { Value = Notes[index].TransformData.Position + new float3(0, HeightOffset, JumpDistance) });

            CommandBuffer.SetComponent(index, noteEntity, new DestroyOnBeat { Beat = CurrentBeat });

            CommandBuffer.SetComponent(index, noteEntity, new Note { Type = Notes[index].Type, CutDirection = Notes[index].CutDirection });
        }

        public void SpawnNote(int index)
        {
            Entity noteEntity;
            if (Notes[index].CutDirection == (int)CutDirection.Any)
            {
                if (Notes[index].Type == 1)
                    noteEntity = CommandBuffer.Instantiate(index, NotePrefabs[0]);
                else
                    noteEntity = CommandBuffer.Instantiate(index, NotePrefabs[1]);
            }
            else
            {
                if (Notes[index].Type == 1)
                    noteEntity = CommandBuffer.Instantiate(index, NotePrefabs[2]);
                else
                    noteEntity = CommandBuffer.Instantiate(index, NotePrefabs[3]);
            }

            CommandBuffer.RemoveComponent<Prefab>(index, noteEntity);

            CommandBuffer.SetComponent(index, noteEntity, new DestroyOnBeat { Beat = CurrentBeat });

            CommandBuffer.SetComponent(index, noteEntity, new Rotation { Value = Notes[index].TransformData.LocalRotation });
            CommandBuffer.SetComponent(index, noteEntity, new Translation { Value = Notes[index].TransformData.Position + new float3(0, HeightOffset, JumpDistance) });

            CommandBuffer.SetComponent(index, noteEntity, new Note { Type = Notes[index].Type, CutDirection = Notes[index].CutDirection });
        }
    }
}
