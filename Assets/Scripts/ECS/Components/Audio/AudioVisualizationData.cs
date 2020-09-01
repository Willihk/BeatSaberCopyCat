using System;
using Unity.Entities;
using Unity.Mathematics;

namespace BeatGame.Logic.Audio
{
    [Serializable]
    [GenerateAuthoringComponent]
    public struct AudioVisualizationData : IComponentData
    {
        public int FrequencyBand;
        public float3 BaseScale;
        public float3 ScaleMultiplier;
    }
}