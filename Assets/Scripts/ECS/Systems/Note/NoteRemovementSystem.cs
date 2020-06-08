using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class NoteRemovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        Entities.WithAll<Note>().ForEach((Entity entity, ref Translation translation) =>
        {
            if (translation.Value.z < -10)
            {
                commandBuffer.DestroyEntity(entity);
            }
        }).Schedule(Dependency).Complete();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
