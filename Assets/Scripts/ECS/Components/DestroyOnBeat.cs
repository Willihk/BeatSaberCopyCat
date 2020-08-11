using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct DestroyOnBeat : IComponentData
{
    public float Beat;
}
