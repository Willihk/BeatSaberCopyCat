using BeatGame.Logic.Managers;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoveOverTimeSystem : SystemBase
{
    EntityQuery objectsToMoveQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        objectsToMoveQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(MoveOverTime) }
        });
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        new MoveJob
        {
            CommandBuffer = commandBuffer.AsParallelWriter(),
            DeltaTime = Time.DeltaTime,
            EntityType = GetEntityTypeHandle(),
            TranslationType = GetComponentTypeHandle<Translation>(),
            MoveOverTimeType = GetComponentTypeHandle<MoveOverTime>(),
        }.Schedule(objectsToMoveQuery, Dependency).Complete();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }

    [BurstCompile]
    struct MoveJob : IJobChunk
    {
        [ReadOnly]
        public float DeltaTime;

        public ComponentTypeHandle<Translation> TranslationType;

        [ReadOnly]
        public EntityTypeHandle EntityType;

        public ComponentTypeHandle<MoveOverTime> MoveOverTimeType;

        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityType);
            NativeArray<Translation> translations = chunk.GetNativeArray(TranslationType);
            NativeArray<MoveOverTime> moveOverTimes = chunk.GetNativeArray(MoveOverTimeType);

            for (int i = 0; i < chunk.Count; i++)
            {
                var moveOverTime = moveOverTimes[i];
                moveOverTime.Elapsed += DeltaTime;
                moveOverTimes[i] = moveOverTime;

                float3 position = math.lerp(moveOverTimes[i].StartPosition, moveOverTimes[i].EndPosition, math.clamp(moveOverTimes[i].Elapsed / moveOverTimes[i].Duration, 0, 1));

                if (moveOverTimes[i].Elapsed >= moveOverTimes[i].Duration)
                {
                    position = moveOverTimes[i].EndPosition;
                    CommandBuffer.RemoveComponent<MoveOverTime>(chunkIndex, entities[i]);
                }

                var translation = translations[i];
                translation.Value = position;
                translations[i] = translation;
            }
        }
    }
}
