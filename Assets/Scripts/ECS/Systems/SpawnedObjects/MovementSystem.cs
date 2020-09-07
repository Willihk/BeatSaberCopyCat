using BeatGame.Logic.Managers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class NoteMovementSystem : SystemBase
{
    EntityQuery objectsToMoveQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        objectsToMoveQuery = GetEntityQuery(new EntityQueryDesc
        {
            Any = new ComponentType[] { typeof(Note), typeof(Obstacle) }
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
        public ComponentTypeHandle<CustomSpeed> SpeedType;
        [ReadOnly]
        public ComponentTypeHandle<WorldRotation> WorldRotationType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Translation> translations = chunk.GetNativeArray(TranslationType);


            if (chunk.Has(SpeedType))
            {
                NativeArray<CustomSpeed> Speeds = chunk.GetNativeArray(SpeedType);

                for (int i = 0; i < chunk.Count; i++)
                {

                }
            }

            if (chunk.Has(WorldRotationType))
            {
                NativeArray<WorldRotation> WorldRotations = chunk.GetNativeArray(WorldRotationType);

                for (int i = 0; i < chunk.Count; i++)
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, WorldRotations[i].Value, Vector3.one);
                    float3 forward = matrix.MultiplyPoint(Vector3.forward);

                    translations[i] = Move(translations[i], forward, Speed);
                }
                return;
            }

            for (int i = 0; i < chunk.Count; i++)
            {
                translations[i] = Move(translations[i], new float3(0, 0, 1), Speed);
            }
        }

        public Translation Move(Translation translation, float3 forward, float speed)
        {
            translation.Value -= forward * (speed * DeltaTime);
            return translation;
        }
    }
}
