using BeatGame.Logic.Managers;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class NoteMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float speed = CurrentSongDataManager.Instance.SelectedDifficultyMap.NoteJumpMovementSpeed;
        Entities.WithAny<Note, Obstacle>().WithNone<WorldRotation>().ForEach((ref Translation translation) =>
        {
            translation.Value.z -= speed * deltaTime;
        }).Schedule(Dependency).Complete();

        Entities.WithAll<Obstacle>().ForEach((ref Translation translation, in WorldRotation worldRotation) =>
        {
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, worldRotation.Value, Vector3.one);

            float3 forward = matrix.MultiplyPoint(Vector3.forward);
            translation.Value -= forward * (speed * deltaTime);
        }).Schedule(Dependency).Complete();
    }
}
