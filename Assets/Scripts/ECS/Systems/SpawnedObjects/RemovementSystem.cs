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

        int missedCount = 0;
        Entities.WithAny<Note>().ForEach((Entity entity, in DestroyOnBeat destroyOnBeat) =>
        {
            if (destroyOnBeat.Beat + jumpDuration * 2.2f <= currentBeat)
            {
                missedCount++;
                commandBuffer.DestroyEntity(entity);
            }
        }).Run();


        for (int i = 0; i < missedCount; i++)
        {
            ScoreManager.Instance.MissedNote();
        }

        // Obstacles
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
