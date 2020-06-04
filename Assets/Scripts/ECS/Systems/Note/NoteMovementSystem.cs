using System;
using Unity.Entities;
using Unity.Transforms;

public class NoteMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float moveSpeed = CurrentSongDataManager.Instance.SelectedDifficultyMap.NoteJumpMovementSpeed;
        Entities.WithAll<Note>().ForEach((ref Translation translation) =>
        {
            translation.Value.z -= moveSpeed * deltaTime;
        }).Schedule(Dependency).Complete();
    }
}
