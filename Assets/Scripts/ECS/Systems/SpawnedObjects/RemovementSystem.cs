using BeatGame.Logic.Managers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RemovementSystem : SystemBase
{
    BeginSimulationEntityCommandBufferSystem entityCommandBufferSystem;

    EntityQuery objectsToMoveOutQuery;


    protected override void OnCreate()
    {
        entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

        objectsToMoveOutQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(Obstacle), typeof(DestroyOnBeat) },
            None = new ComponentType[] { typeof(MoveOverTime) }
        });
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer commandBuffer = entityCommandBufferSystem.CreateCommandBuffer();

        double currentBeat = GameManager.Instance.CurrentBeat;
        double jumpDuration = CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration;

        int missedCount = 0;
        Entities.ForEach((Entity entity, in DestroyOnBeat destroyOnBeat, in Note note) =>
        {
            if (destroyOnBeat.Beat + jumpDuration * 2.2f <= currentBeat)
            {
                if (note.Type != 3)
                    missedCount++;

                commandBuffer.DestroyEntity(entity);
            }
        }).Run();

        commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        for (int i = 0; i < missedCount; i++)
        {
            ScoreManager.Instance.MissedNote();
            HealthManager.Instance.MissedNote();
        }

        new MoveOutJob
        {
            CommandBuffer = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
            CurrentBeat = (float)(GameManager.Instance.CurrentBeat - Time.DeltaTime),
            JumpDuration = (float)CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration,
            SecondEquivalentOfBeat = (float)CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat,
            DestroyOnBeatType = GetComponentTypeHandle<DestroyOnBeat>(true),
            WorldRotationType = GetComponentTypeHandle<WorldRotation>(true),
            TranslationType = GetComponentTypeHandle<Translation>(true),
            EntityType = GetEntityTypeHandle(),
        }.Schedule(objectsToMoveOutQuery).Complete();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();

        commandBuffer = entityCommandBufferSystem.CreateCommandBuffer();

        Entities.WithNone<MoveOverTime>().ForEach((Entity entity, ref DestroyOnBeat destroyOnBeat) =>
        {
            if (destroyOnBeat.Beat + jumpDuration * 8 <= currentBeat)
            {
                commandBuffer.DestroyEntity(entity);
            }
        }).Schedule(Dependency).Complete();
    }

    public void RemoveAllSpawnedObjects()
    {
        EntityCommandBuffer commandBuffer = entityCommandBufferSystem.CreateCommandBuffer();

        Entities.ForEach((Entity entity, ref DestroyOnBeat destroyOnBeat) =>
        {
            commandBuffer.DestroyEntity(entity);
        }).Schedule(Dependency).Complete();
    }


    [BurstCompile]
    struct MoveOutJob : IJobChunk
    {
        [ReadOnly]
        public float SecondEquivalentOfBeat;
        [ReadOnly]
        public float JumpDuration;
        [ReadOnly]
        public float CurrentBeat;

        [ReadOnly]
        public ComponentTypeHandle<Translation> TranslationType;
        [ReadOnly]
        public ComponentTypeHandle<DestroyOnBeat> DestroyOnBeatType;
        [ReadOnly]
        public ComponentTypeHandle<WorldRotation> WorldRotationType;
        [ReadOnly]
        public EntityTypeHandle EntityType;

        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {

            NativeArray<DestroyOnBeat> destroyOnBeats = chunk.GetNativeArray(DestroyOnBeatType);
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityType);
            NativeArray<Translation> translations = chunk.GetNativeArray(TranslationType);
            NativeArray<WorldRotation> worldRotations = default;

            bool customRotations = false;

            float3 forward = new float3(0, 0, 1);

            if (chunk.Has(WorldRotationType))
            {
                worldRotations = chunk.GetNativeArray(WorldRotationType);

                customRotations = true;
            }

            for (int i = 0; i < chunk.Count; i++)
            {
                if (destroyOnBeats[i].Beat + JumpDuration * 4 > CurrentBeat)
                    continue;

                if (customRotations)
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, worldRotations[i].Value, Vector3.one);
                    forward = matrix.MultiplyPoint(Vector3.forward);
                }

                CommandBuffer.AddComponent<MoveOverTime>(chunkIndex, entities[i]);
                CommandBuffer.SetComponent(chunkIndex, entities[i], new MoveOverTime
                {
                    Duration = SecondEquivalentOfBeat * 2,
                    StartPosition = translations[i].Value,
                    EndPosition = translations[i].Value - forward * 60
                });
            }
        }
    }
}
