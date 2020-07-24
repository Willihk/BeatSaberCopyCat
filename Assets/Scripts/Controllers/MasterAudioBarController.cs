using UnityEngine;
using System.Collections;
using System;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

public class MasterAudioBarController : MonoBehaviour
{
    public int barCount;
    public Vector3 gap = new Vector3(0, 0, 10);
    public GameObject bar;
    public float3 scaleMultiplier;

    private void Awake()
    {
        SpawnBars();
    }

    private void SpawnBars()
    {
        var entity = EntityPrefabManager.Instance.ConvertGameObjectToEntity(bar, false);
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var entities = new NativeArray<Entity>(barCount, Allocator.TempJob);
        entityManager.Instantiate(entity, entities);

        for (int i = 0; i < entities.Length; i++)
        {
            int frequencyBand = 0;
            if (AudioSpectrumManager.Instance.FrequencyBands >= barCount)
                frequencyBand = i;
            else
            {
                double barsPerFrequency = Math.Ceiling((double)barCount / (AudioSpectrumManager.Instance.FrequencyBands - 1));
                if (i == 0)
                    frequencyBand = i;
                else
                    frequencyBand = (int)math.floor(i / barsPerFrequency);
            }

            if (frequencyBand >= AudioSpectrumManager.Instance.FrequencyBands)
                frequencyBand = AudioSpectrumManager.Instance.FrequencyBands - 1;

            var position = bar.transform.position + gap * i;
            entityManager.SetComponentData(entities[i], new Translation { Value = position });
            entityManager.SetComponentData(entities[i], new AudioVisualizationData
            {
                FrequencyBand = frequencyBand,
                BaseScale = bar.transform.localScale,
                ScaleMultiplier = scaleMultiplier
            });
        }
        entities.Dispose();
    }
}
