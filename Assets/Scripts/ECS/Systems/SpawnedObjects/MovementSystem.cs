using BeatGame.Logic.Managers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MovementSystem : SystemBase
{
    EntityQuery objectsToMoveQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        objectsToMoveQuery = GetEntityQuery(new EntityQueryDesc
        {
            Any = new ComponentType[] { typeof(Note), typeof(Obstacle) },
            None = new ComponentType[] { typeof(MoveOverTime) }
        });
    }

    protected override void OnUpdate()
    {
        new MoveJob
        {
            DeltaTime = Time.DeltaTime,
            Speed = CurrentSongDataManager.Instance.SelectedDifficultyMap.NoteJumpMovementSpeed,
            TranslationType = GetComponentTypeHandle<Translation>(),
            WorldRotationType = GetComponentTypeHandle<WorldRotation>(true),
            SpeedType = GetComponentTypeHandle<CustomSpeed>(true),
        }.Schedule(objectsToMoveQuery, Dependency).Complete();
    }

    [BurstCompile]
    struct MoveJob : IJobChunk
    {
        public float Speed;
        [ReadOnly]
        public float DeltaTime;

        public ComponentTypeHandle<Translation> TranslationType;
        [ReadOnly]
        public ComponentTypeHandle<WorldRotation> WorldRotationType;
        [ReadOnly]
        public ComponentTypeHandle<CustomSpeed> SpeedType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Translation> translations = chunk.GetNativeArray(TranslationType);
            NativeArray<CustomSpeed> speeds = default;
            NativeArray<WorldRotation> worldRotations = default;

            bool customSpeeds = false;
            bool customRotations = false;

            if (chunk.Has(SpeedType))
            {
                speeds = chunk.GetNativeArray(SpeedType);
                customSpeeds = true;
            }

            if (chunk.Has(WorldRotationType))
            {
                worldRotations = chunk.GetNativeArray(WorldRotationType);

                customRotations = true;
            }

            for (int i = 0; i < chunk.Count; i++)
            {
                float speed = Speed;
                float3 forward = new float3(0, 0, 1);

                if (customSpeeds)
                {
                    speed = speeds[i].Value;
                }

                if (customRotations)
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, worldRotations[i].Value, Vector3.one);
                    forward = matrix.MultiplyPoint(Vector3.forward);
                }

                translations[i] = Move(translations[i], forward, speed);
            }
        }

        public Translation Move(Translation translation, float3 forward, float speed)
        {
            translation.Value -= forward * (speed * DeltaTime);
            return translation;
        }
    }
}
