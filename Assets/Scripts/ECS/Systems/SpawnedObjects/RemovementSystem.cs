using BeatGame.Logic.Managers;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class NoteRemovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        double currentBeat = GameManager.Instance.CurrentBeat;
        double jumpDuration = CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration;
        Entities.WithAny<Note, Obstacle>().ForEach((Entity entity, ref Translation translation, ref DestroyOnBeat destroyOnBeat) =>
        {
            if (destroyOnBeat.Beat + jumpDuration * 4 <= currentBeat)
            {
                commandBuffer.DestroyEntity(entity);
            }
        }).Schedule(Dependency).Complete();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
