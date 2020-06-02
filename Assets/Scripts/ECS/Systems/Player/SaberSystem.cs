using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SaberSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation translation) =>
        {
            math.atan2()
        }).Schedule();
    }
}
