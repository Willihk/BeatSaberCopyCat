using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Transforms;

public class SaberMouseSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));

        Entities.ForEach((ref SaberData saberData, ref Translation translation) =>
        {
            translation.Value = point;
        }).Run();
    }
}
