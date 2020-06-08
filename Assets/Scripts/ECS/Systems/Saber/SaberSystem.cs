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

            var note = EntityManager.GetComponentData<Note>(hit.Entity);
            if (note.Data.Type == saberData.AffectsType)
            {
                quaternion noteRotation = EntityManager.GetComponentData<Rotation>(hit.Entity).Value;
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, noteRotation, Vector3.one);

                float angle = Vector3.Angle(translation.Value - saberData.PreviousPosition, matrix.MultiplyPoint(Vector3.up));
                if (angle > 130 || note.Data.CutDirection == 8)
                {
                    // TODO Reward player with points

                    commandBuffer.DestroyEntity(hit.Entity);
                }
            }
            else if (note.Data.Type == 3)
            {
                // Hit Bomb
                Debug.LogWarning("Saber hit a bomb");
            }

            saberData.PreviousPosition = translation.Value;
        }).WithoutBurst().Run();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
