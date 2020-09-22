using BeatGame.Data.Map.Modified;
using BeatGame.Logic.Managers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ObstacleSpawningSystem : SystemBase
{
    public NativeList<ObstacleData> obstacles;

    JobHandle scheduledJob;
    BeginSimulationEntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        obstacles = new NativeList<ObstacleData>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        scheduledJob.Complete();
        obstacles.Dispose();
    }

    protected override void OnUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPlaying)
        {
            var job = new SpawnJobParallel
            {
                CommandBuffer = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
                Entity = EntityPrefabManager.Instance.GetEntityPrefab("Wall"),
                Obstacles = obstacles,
                HeightOffset = SettingsManager.GlobalOffset.y,
                CurrentBeat = GameManager.Instance.CurrentBeat - Time.DeltaTime,
                LastBeat = GameManager.Instance.LastBeat - Time.DeltaTime,
                HalfJumpDuration = CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration,
                JumpDistance = CurrentSongDataManager.Instance.SongSpawningInfo.JumpDistance,
                Speed = CurrentSongDataManager.Instance.SongSpawningInfo.NoteJumpSpeed,
                SecondEquivalentOfBeat = (float)CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat
            };

            scheduledJob = job.Schedule(obstacles.Length, 64);
            entityCommandBufferSystem.AddJobHandleForProducer(JobHandle.CombineDependencies(Dependency, scheduledJob));
        }
    }

    [BurstCompile]
    struct SpawnJobParallel : IJobParallelFor
    {
        public EntityCommandBuffer.ParallelWriter CommandBuffer;
        [ReadOnly]
        public NativeList<ObstacleData> Obstacles;

        [ReadOnly]
        public float HeightOffset;
        [ReadOnly]
        public double HalfJumpDuration;
        [ReadOnly]
        public double CurrentBeat;
        [ReadOnly]
        public double LastBeat;
        [ReadOnly]
        public float JumpDistance;
        [ReadOnly]
        public float Speed;
        [ReadOnly]
        public float SecondEquivalentOfBeat;

        [ReadOnly]
        public Entity Entity;

        public void Execute(int index)
        {
            var obstacle = Obstacles[index];
            float spawnOffset = 2;
            float distanceOffset = 6;

            if (obstacle.Time - HalfJumpDuration <= CurrentBeat + spawnOffset && obstacle.Time - HalfJumpDuration >= LastBeat + spawnOffset)
            {
                var entity = CommandBuffer.Instantiate(index, Entity);
                CommandBuffer.RemoveComponent<Prefab>(index, entity);

                CommandBuffer.AddComponent<MoveOverTime>(index, entity);
                CommandBuffer.AddComponent<ColorData>(index, entity);

                float4 color = new float4
                {
                    xyz = obstacle.Color,
                    w = .7f
                };

                CommandBuffer.SetComponent(index, entity, new ColorData { Value = color });

                if (obstacle.TransformData.Speed != Speed)
                {
                    CommandBuffer.AddComponent(index, entity, new CustomSpeed { Value = obstacle.TransformData.Speed });
                }

                //CommandBuffer.AddComponent<CompositeScale>(index, entity);
                CommandBuffer.SetComponent(index, entity, new NonUniformScale { Value = new float3(obstacle.TransformData.Scale.c0.x, obstacle.TransformData.Scale.c1.y, obstacle.TransformData.Scale.c2.z) });
                //CommandBuffer.SetComponent(index, entity, new CompositeScale { Value = obstacle.TransformData.Scale });

                CommandBuffer.SetComponent(index, entity, new Rotation { Value = obstacle.TransformData.LocalRotation });

                CommandBuffer.SetComponent(index, entity, new DestroyOnBeat { Beat = (float)CurrentBeat + spawnOffset + (obstacle.TransformData.Scale.c2.z * 2 / Speed) });

                if (obstacle.TransformData.WorldRotation != 0)
                {
                    CommandBuffer.AddComponent<WorldRotation>(index, entity);
                    CommandBuffer.SetComponent(index, entity, new WorldRotation { Value = Quaternion.Euler(0, obstacle.TransformData.WorldRotation, 0) });

                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, obstacle.TransformData.WorldRotation, 0), Vector3.one);

                    float3 adjustedDistance = matrix.MultiplyPoint(Vector3.forward) * JumpDistance;
                    CommandBuffer.SetComponent(index, entity, new Translation { Value = obstacle.TransformData.Position + new float3(0, HeightOffset, 0) + adjustedDistance * distanceOffset });

                    CommandBuffer.SetComponent(index, entity, new MoveOverTime
                    {
                        Duration = SecondEquivalentOfBeat * spawnOffset,
                        StartPosition = obstacle.TransformData.Position + new float3(0, HeightOffset, 0) + adjustedDistance * distanceOffset,
                        EndPosition = obstacle.TransformData.Position + new float3(0, HeightOffset, 0) + adjustedDistance
                    });
                }
                else
                {
                    CommandBuffer.SetComponent(index, entity, new Translation { Value = obstacle.TransformData.Position + new float3(0, HeightOffset, JumpDistance * distanceOffset) });

                    CommandBuffer.SetComponent(index, entity, new MoveOverTime
                    {
                        Duration = SecondEquivalentOfBeat * spawnOffset,
                        StartPosition = obstacle.TransformData.Position + new float3(0, HeightOffset, JumpDistance * distanceOffset),
                        EndPosition = obstacle.TransformData.Position + new float3(0, HeightOffset, JumpDistance)
                    });
                }
            }
        }
    }
}
