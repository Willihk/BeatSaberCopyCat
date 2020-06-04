using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SaberData : IComponentData
{
    public float Length;
    public float3 PreviousPosition;
}
