using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Note : IComponentData
{
    public int Type;
    public int CutDirection;
}
