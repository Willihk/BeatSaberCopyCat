using BeatGame.Logic.Managers;
using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class InsideObstacleDetectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            return;

        float3 cameraPosition = Camera.main.transform.position;
        bool IsInsideObstacle = false;

        Entities.WithAll<Obstacle>().ForEach((in WorldRenderBounds renderBounds) =>
        {
            if (!IsInsideObstacle)
                IsInsideObstacle = renderBounds.Value.Contains(cameraPosition);

        }).Run();

        if (IsInsideObstacle)
            HealthManager.Instance.InsideObstacle();
    }
}
