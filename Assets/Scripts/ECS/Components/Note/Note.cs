using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Note : IComponentData
{
    public NoteSpawnData Data;
}
