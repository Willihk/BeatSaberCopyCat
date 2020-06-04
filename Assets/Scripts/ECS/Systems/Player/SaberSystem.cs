using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SaberSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        Entities.ForEach((ref SaberData saberData, ref Translation translation, ref Rotation rotation) =>
        {
            var hit = ECSRaycast.Raycast(translation.Value, translation.Value + (math.forward(rotation.Value) * saberData.Length));

            if (hit.Entity == Entity.Null)
                return;

            //if (Vector3.Angle(translation.Value - saberData.PreviousPosition, quaternion.rot) > 130)
            //{
            commandBuffer.DestroyEntity(hit.Entity);
            //}
        }).Run();
    }
}
