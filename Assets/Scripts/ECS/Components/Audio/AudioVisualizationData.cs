using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct AudioVisualizationData : IComponentData
{
    public int FrequencyBand;
    public float3 BaseScale;
    public float3 ScaleMultiplier;
}
