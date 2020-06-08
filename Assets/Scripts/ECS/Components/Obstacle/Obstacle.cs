using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Obstacle : IComponentData
{
    public ObstacleData Data;
}
