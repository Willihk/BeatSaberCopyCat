using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ObstacleSpawningSystem : SystemBase
{
    public NativeList<ObstacleData> obstaclesToSpawn;

    private const float SpawnTimeOffset = 5;

    // Needs to be here to run the system
    EntityQuery defaultQuery;

    float3 spawnPointOffset = new float3(.8f, .8f, 0);

    protected override void OnCreate()
    {
        obstaclesToSpawn = new NativeList<ObstacleData>(Allocator.Persistent);

        defaultQuery = GetEntityQuery(new EntityQueryDesc { All = new ComponentType[] { typeof(Entity) } });
    }

    protected override void OnDestroy()
    {
        obstaclesToSpawn.Dispose();
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
        if (obstaclesToSpawn.IsCreated == false)
            return;

        for (int i = 0; i < obstaclesToSpawn.Length; i++)
        {
            var obstacle = obstaclesToSpawn[i];

            if (obstacle.Time - SpawnTimeOffset <= GameManager.Instance.CurrentSongTime)
            {
                SpawnObstacle(obstacle);

                obstaclesToSpawn.RemoveAtSwapBack(i);
                i--;
            }
        }
    }

    private void SpawnObstacle(ObstacleData obstacle)
    {
        var noteEntity = EntityPrefabManager.Instance.SpawnEntityPrefab("Wall");

        float4x4 scale = new float4x4
        {
            c0 = new float4(obstacle.Width, 0, 0, 0),
            c1 = new float4(0, obstacle.Type == 0 ? spawnPointOffset.y * 3 : spawnPointOffset.y * 2, 0, 0),
            c2 = new float4(0, 0, (float)obstacle.Duration, 0),
            c3 = new float4(0, 0, 0, 1)
        };
        float lineIndex = obstacle.LineIndex + (obstacle.Width / 2);
        float lineLayer = 0;

        if (obstacle.Type == 0)
        {
            lineLayer = 1;
        }
        else if (obstacle.Type == 1)
        {
            lineLayer = 2;
        }

        EntityManager.SetComponentData(noteEntity, new Translation { Value = (GetSpawnPosition(lineIndex, lineLayer) + new float3(0, 0, GetNeededOffset(obstacle.Time))) });


        EntityManager.SetComponentData(noteEntity, new CompositeScale { Value = scale });
        EntityManager.SetComponentData(noteEntity, new Obstacle { Data = obstacle });
    }

    float3 GetSpawnPosition(float lineIndex, float lineLayer)
    {
        return new float3(lineIndex * spawnPointOffset.x, lineLayer * spawnPointOffset.y, 0);
    }

    float GetNeededOffset(double timeToSpawn)
    {
        return (float)(SpawnTimeOffset * CurrentSongDataManager.Instance.SelectedDifficultyMap.NoteJumpMovementSpeed + CurrentSongDataManager.Instance.SelectedDifficultyMap.NoteJumpStartBeatOffset);
    }
}
