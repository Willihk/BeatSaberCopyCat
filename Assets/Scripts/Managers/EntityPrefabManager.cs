using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using System.Collections.Generic;

namespace BeatGame.Logic.Managers
{
    public class EntityPrefabManager : MonoBehaviour
    {
        public static EntityPrefabManager Instance;

        public Dictionary<string, Entity> Prefabs = new Dictionary<string, Entity>();

        EntityManager EntityManager;

        BlobAssetStore assetStore;

        private void Awake()
        {
            if (Instance is null)
                Instance = this;
        }

        private void Start()
        {
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            assetStore = new BlobAssetStore();
            GameObject[] gameObjectPrefabs = Resources.LoadAll<GameObject>("Prefabs/");

            foreach (var item in gameObjectPrefabs)
            {
                ConvertGameObjectToEntity(item);
            }
        }

        private void OnDestroy()
        {
            assetStore.Dispose();
        }

        public Entity ConvertGameObjectToEntity(GameObject gameObject, bool addToPrefabs = true)
        {
            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, assetStore);

            var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObject, settings);

            if (addToPrefabs)
            {
                EntityManager.AddComponent<Prefab>(entity);
                Prefabs.Add(gameObject.name, entity);
            }

            return entity;
        }

        public Entity GetEntityPrefab(string name)
        {
            if (Prefabs.TryGetValue(name, out Entity prefab))
                return prefab;

            return Entity.Null;
        }

        public Entity SpawnEntityPrefab(string name)
        {
            if (Prefabs.TryGetValue(name, out Entity prefab))
            {
                Entity entity = EntityManager.Instantiate(prefab);
                EntityManager.RemoveComponent<Prefab>(entity);
                return entity;
            }

            return Entity.Null;
        }

        public Entity SpawnEntityPrefab(EntityCommandBuffer commandBuffer, string name)
        {
            var prefab = GetEntityPrefab(name);
            if (prefab != Entity.Null)
            {
                Entity entity = commandBuffer.Instantiate(prefab);
                commandBuffer.RemoveComponent<Prefab>(entity);
                return entity;
            }

            return Entity.Null;
        }

        public Entity SpawnVisualOnlyPrefab(string name)
        {
            var prefab = GetEntityPrefab(name);
            if (prefab != Entity.Null)
            {
                var entity = EntityManager.CreateEntity(
                    ComponentType.ReadOnly<Translation>(),
                    ComponentType.ReadOnly<Rotation>(),
                    ComponentType.ReadOnly<Scale>(),
                    ComponentType.ReadOnly<RenderMesh>(),
                    ComponentType.ReadOnly<RenderBounds>(),
                    ComponentType.ReadOnly<WorldRenderBounds>(),
                    ComponentType.ReadOnly<LocalToWorld>());

                if (EntityManager.HasComponent<NonUniformScale>(prefab))
                    EntityManager.AddComponentData(entity, EntityManager.GetComponentData<NonUniformScale>(prefab));
                else if (EntityManager.HasComponent<Scale>(prefab))
                    EntityManager.AddComponentData(entity, EntityManager.GetComponentData<Scale>(prefab));
                else
                    EntityManager.AddComponentData(entity, new Scale { Value = 1 });

                EntityManager.AddComponentData(entity, EntityManager.GetComponentData<Translation>(prefab));
                EntityManager.AddComponentData(entity, EntityManager.GetComponentData<Rotation>(prefab));
                EntityManager.AddComponentData(entity, EntityManager.GetComponentData<RenderBounds>(prefab));
                EntityManager.AddSharedComponentData(entity, EntityManager.GetSharedComponentData<RenderMesh>(prefab));
                return entity;
            }

            return Entity.Null;
        }
    }
}