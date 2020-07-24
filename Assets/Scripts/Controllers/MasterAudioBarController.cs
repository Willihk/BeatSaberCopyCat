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
            if (AudioSpectrumManager.Instance.frequencyBandCount >= barCount)
                frequencyBand = i;
            else
                frequencyBand = (int)math.floor(i / (int)math.floor(barCount / AudioSpectrumManager.Instance.frequencyBandCount - 1));

            if (frequencyBand >= AudioSpectrumManager.Instance.frequencyBandCount)
                frequencyBand = AudioSpectrumManager.Instance.frequencyBandCount - 1;

            var position = bar.transform.position + gap * i;
            entityManager.SetComponentData(entities[i], new Translation { Value = position });
            entityManager.SetComponentData(entities[i], new AudioVisualizationData
            {
                FrequencyBand = frequencyBand,
                BaseScale = bar.transform.localScale,
                ScaleMultiplier = scaleMultiplier * 10
            });
        }
        entities.Dispose();
    }
}
