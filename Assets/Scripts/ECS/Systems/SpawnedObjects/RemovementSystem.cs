using BeatGame.Logic.Managers;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class RemovementSystem : SystemBase
{
    BeginSimulationEntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer commandBuffer = entityCommandBufferSystem.CreateCommandBuffer();

        double currentBeat = GameManager.Instance.CurrentBeat;
        double jumpDuration = CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration;

        int missedCount = 0;
        Entities.ForEach((Entity entity, in DestroyOnBeat destroyOnBeat, in Note note) =>
        {
            if (destroyOnBeat.Beat + jumpDuration * 2.2f <= currentBeat)
            {
                if (note.Type != 3)
                    missedCount++;

                commandBuffer.DestroyEntity(entity);
            }
        }).Run();

        commandBuffer = entityCommandBufferSystem.CreateCommandBuffer();

        for (int i = 0; i < missedCount; i++)
        {
            ScoreManager.Instance.MissedNote();
            HealthManager.Instance.MissedNote();
        }

        // Obstacles
        Entities.WithAny<Note, Obstacle>().ForEach((Entity entity, ref DestroyOnBeat destroyOnBeat) =>
         {
             if (destroyOnBeat.Beat + jumpDuration * 4 <= currentBeat)
             {
                 commandBuffer.DestroyEntity(entity);
             }
         }).Schedule(Dependency).Complete();
    }
}
