using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((ref Translation translation, ref MoveSpeed moveSpeed) =>
        {
            translation.Value.z += moveSpeed.Value * deltaTime;
        }).Schedule();
    }
}
