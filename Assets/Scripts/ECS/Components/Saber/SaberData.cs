using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SaberData : IComponentData
{
    public int AffectsType;
    public float Length;
    public float3 PreviousPosition;
}
