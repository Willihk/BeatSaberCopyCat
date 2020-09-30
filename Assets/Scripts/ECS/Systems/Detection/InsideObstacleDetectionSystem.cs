using BeatGame.Logic.Managers;
using System;
using System.Security.Policy;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class InsideObstacleDetectionSystem : SystemBase
{
    public event Action OnEnteredObstacle;

    NativeQueue<bool> detections;

    EntityQuery obstacleQuery;

    JobHandle job;

    protected override void OnCreate()
    {
        base.OnCreate();

        detections = new NativeQueue<bool>(Allocator.Persistent);

        obstacleQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(Obstacle), typeof(WorldRenderBounds) }
        });
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        detections.Dispose();
    }

    protected override void OnUpdate()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            return;

        job.Complete();
        while (detections.TryDequeue(out bool isInsideObstacle))
        {
            if (isInsideObstacle)
            {
                HealthManager.Instance.InsideObstacle();
                OnEnteredObstacle?.Invoke();
            }
        }
        detections.Clear();

        var newJob = new DetectionJob
        {
            CameraPosition = Camera.main.transform.position,
            Output = detections.AsParallelWriter(),
            WorldRenderBoundsType = GetComponentTypeHandle<WorldRenderBounds>(true)
        };
        job = newJob.ScheduleParallel(obstacleQuery, Dependency);
        Dependency = JobHandle.CombineDependencies(Dependency, job);
    }

    [BurstCompile]
    struct DetectionJob : IJobChunk
    {
        [ReadOnly]
        public float3 CameraPosition;
        [ReadOnly]
        public ComponentTypeHandle<WorldRenderBounds> WorldRenderBoundsType;

        public NativeQueue<bool>.ParallelWriter Output;


        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<WorldRenderBounds> renderBounds = chunk.GetNativeArray(WorldRenderBoundsType);

            for (int i = 0; i < chunk.Count; i++)
            {
                if (renderBounds[i].Value.Contains(CameraPosition))
                {
                    Output.Enqueue(true);
                    return;
                }
            }
        }
    }
}
