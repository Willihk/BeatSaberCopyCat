using BeatGame.Logic.Managers;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class NoteMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float speed = CurrentSongDataManager.Instance.SelectedDifficultyMap.NoteJumpMovementSpeed;
        Entities.WithAny<Note, Obstacle>().ForEach((ref Translation translation) =>
        {
            translation.Value.z -=  speed * deltaTime;
        }).Schedule(Dependency).Complete();
    }
}
