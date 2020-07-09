using Unity.Entities;
using Unity.Mathematics;

public struct ObstacleData
{
    public double Time { get; set; }

    public TransformData TransformData { get; set; }

    public float3 Color { get; set; }
}
