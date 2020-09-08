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
                JumpSpeed = CurrentSongDataManager.Instance.SongSpawningInfo.NoteJumpSpeed,
                SecondEquivalentOfBeat = (float)CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat,
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
        public float SecondEquivalentOfBeat;
        [ReadOnly]
        public float JumpDistance;
        [ReadOnly]
        public float JumpSpeed;
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
            float spawnOffset = 1;
            float distanceOffset = 6;
            if (Notes[index].Time - HalfJumpDuration <= CurrentBeat + spawnOffset && Notes[index].Time - HalfJumpDuration >= LastBeat + spawnOffset)
            {
                Entity entity;
                if (Notes[index].Type == 3)
                    entity = SpawnBomb(index);
                else
                    entity = SpawnNote(index);

                CommandBuffer.RemoveComponent<Prefab>(index, entity);

                //if (Notes[index].TransformData.Speed != JumpSpeed)
                //{
                //    CommandBuffer.AddComponent(index, entity, new CustomSpeed { Value = Notes[index].TransformData.Speed });
                //}

                //if (Notes[index].TransformData.WorldRotation != 0)
                //{
                //    CommandBuffer.AddComponent<WorldRotation>(index, entity);
                //    CommandBuffer.SetComponent(index, entity, new WorldRotation { Value = Quaternion.Euler(0, Notes[index].TransformData.WorldRotation, 0) });
                //    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, Notes[index].TransformData.WorldRotation, 0), Vector3.one);

                //    float3 forward = matrix.MultiplyPoint(Vector3.forward);
                //    forward *= JumpDistance;
                //    CommandBuffer.SetComponent(index, entity, new Translation { Value = Notes[index].TransformData.Position + new float3(0, HeightOffset, 0) + forward * distanceOffset });

                //    CommandBuffer.AddComponent<MoveOverTime>(index, entity);
                //    CommandBuffer.SetComponent(
                //        index,
                //        entity,
                //        new MoveOverTime
                //        {
                //            Duration = SecondEquivalentOfBeat * spawnOffset,
                //            StartPosition = Notes[index].TransformData.Position + new float3(0, HeightOffset, 0) + forward * distanceOffset,
                //            EndPosition = Notes[index].TransformData.Position + new float3(0, HeightOffset, 0) + forward
                //        });
                //}
                //else
                //{
                CommandBuffer.SetComponent(index, entity, new Translation { Value = Notes[index].TransformData.Position + new float3(0, HeightOffset, JumpDistance * distanceOffset) });

                CommandBuffer.AddComponent<MoveOverTime>(index, entity);
                CommandBuffer.SetComponent(index, entity, new MoveOverTime
                {
                    Duration = SecondEquivalentOfBeat * spawnOffset,
                    StartPosition = Notes[index].TransformData.Position + new float3(0, HeightOffset, JumpDistance * distanceOffset),
                    EndPosition = Notes[index].TransformData.Position + new float3(0, HeightOffset, JumpDistance)
                });
                //}

                CommandBuffer.SetComponent(index, entity, new DestroyOnBeat { Beat = CurrentBeat + spawnOffset });

                CommandBuffer.SetComponent(index, entity, new Rotation { Value = Notes[index].TransformData.LocalRotation });

                CommandBuffer.SetComponent(index, entity, new Note { Type = Notes[index].Type, CutDirection = Notes[index].CutDirection });

            }
        }

        Entity SpawnBomb(int index)
        {
            return CommandBuffer.Instantiate(index, NotePrefabs[4]);


        }

        public Entity SpawnNote(int index)
        {
            if (Notes[index].CutDirection == (int)CutDirection.Any)
            {
                if (Notes[index].Type == 1)
                    return CommandBuffer.Instantiate(index, NotePrefabs[0]);
                else
                    return CommandBuffer.Instantiate(index, NotePrefabs[1]);
            }
            else
            {
                if (Notes[index].Type == 1)
                    return CommandBuffer.Instantiate(index, NotePrefabs[2]);
                else
                    return CommandBuffer.Instantiate(index, NotePrefabs[3]);
            }
        }
    }
}
