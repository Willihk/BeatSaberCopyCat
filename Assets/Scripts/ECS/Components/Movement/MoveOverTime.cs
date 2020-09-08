using System;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MoveOverTime : IComponentData
{
    public float Duration;
    public float3 StartPosition;
    public float3 EndPosition;
    public float Elapsed;
}
