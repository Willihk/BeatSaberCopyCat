using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace BeatGame.Logic.Audio
{
    public class AudioVisualizationSystem : SystemBase
    {
        EntityQuery audioBarQuery;
        NativeArray<float> frequencyBands;

        protected override void OnStartRunning()
        {
            audioBarQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(AudioVisualizationData), typeof(NonUniformScale) }
            });

            if (AudioSpectrumManager.Instance != null)
                frequencyBands = new NativeArray<float>(AudioSpectrumManager.Instance.FrequencyBands, Allocator.Persistent);
        }

        protected override void OnUpdate()
        {
            if (AudioSpectrumManager.Instance == null)
                return;

            frequencyBands.CopyFrom(AudioSpectrumManager.Instance.AudioBandBuffer);

            var job = new VisualizeJob
            {
                FrequencyBands = frequencyBands,
                AudioVisualizationDataType = GetComponentTypeHandle<AudioVisualizationData>(true),
                NonUniformScaleType = GetComponentTypeHandle<NonUniformScale>(),
            };

            Dependency = JobHandle.CombineDependencies(job.Schedule(audioBarQuery, Dependency), Dependency);
        }

        protected override void OnStopRunning()
        {
            Dependency.Complete();
            frequencyBands.Dispose();
        }

        [BurstCompile]
        struct VisualizeJob : IJobChunk
        {
            [ReadOnly]
            public ComponentTypeHandle<AudioVisualizationData> AudioVisualizationDataType;
            public ComponentTypeHandle<NonUniformScale> NonUniformScaleType;

            [ReadOnly]
            public NativeArray<float> FrequencyBands;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                NativeArray<AudioVisualizationData> audioVisualizationDatas = chunk.GetNativeArray(AudioVisualizationDataType);
                NativeArray<NonUniformScale> scaleDatas = chunk.GetNativeArray(NonUniformScaleType);

                for (int i = 0; i < chunk.Count; i++)
                {
                    var scale = scaleDatas[i];
                    var visualizationData = audioVisualizationDatas[i];

                    float3 endScale = math.lerp(
                        scale.Value,
                       (visualizationData.BaseScale + (new float3(0, 60, 0) * FrequencyBands[visualizationData.FrequencyBand])),
                        .45f);
                    scale.Value = endScale;
                    scaleDatas[i] = scale;
                }
            }
        }
    }
}