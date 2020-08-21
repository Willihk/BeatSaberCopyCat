using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[MaterialProperty("_Color", MaterialPropertyFormat.Float4)]
public struct ColorData : IComponentData
{
    public float4 Value;
}
