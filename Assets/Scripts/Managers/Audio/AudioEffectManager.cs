using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.Security.Policy;
using Unity.Entities;

namespace BeatGame.Logic.Audio
{
    public class AudioEffectManager : MonoBehaviour
    {
        public static AudioEffectManager Instance;

        [SerializeField]
        AudioMixerSnapshot defaultSnapshot;
        [SerializeField]
        AudioMixerSnapshot insideObstacleSnapshot;

        bool isInsideObstacle;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void OnEnable()
        {
            var system = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InsideObstacleDetectionSystem>();
            system.OnEnteredObstacle += EnteredObstacle;
        }

        private void OnDisable()
        {
            var system = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InsideObstacleDetectionSystem>();
            if (system != null)
                system.OnEnteredObstacle += EnteredObstacle;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.O))
            {
                isInsideObstacle = true;
            }

            if (isInsideObstacle)
            {
                insideObstacleSnapshot.TransitionTo(.2f);

            }
            else
            {
                defaultSnapshot.TransitionTo(.2f);
            }

            isInsideObstacle = false;
        }

        void EnteredObstacle()
        {
            isInsideObstacle = true;
        }
    }
}