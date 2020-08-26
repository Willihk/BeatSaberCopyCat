using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using BeatGame.Utility.Physics;
using Valve.VR;

namespace BeatGame.Logic.Saber
{
    public class SaberController : MonoBehaviour
    {
        [SerializeField]
        int affectsNoteType;
        [SerializeField]
        float saberLength = 1;
        [SerializeField]
        SteamVR_Action_Vibration hapticAction;

        float impactMagnifier = 120f;
        float collisionForce = 0f;
        float maxCollisionForce = 4000f;
        //VRTK_ControllerReference controllerReference;

        float3 previousPosition;
        EntityManager EntityManager;

        private void Start()
        {
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            //var controllerEvent = GetComponentInChildren<VRTK_ControllerEvents>(true);
            //if (controllerEvent != null && controllerEvent.gameObject != null)
            //{
            //    controllerReference = VRTK_ControllerReference.GetControllerReference(controllerEvent.gameObject);
            //}
        }

        private void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source)
        {
            hapticAction.Execute(0, duration, frequency, amplitude, source);
        }

        void Update()
        {
            var hit = ECSRaycast.Raycast(transform.position, transform.forward * saberLength);

            if (EntityManager.HasComponent<Note>(hit.Entity))
            {
                var note = EntityManager.GetComponentData<Note>(hit.Entity);
                if (note.Type == affectsNoteType)
                {
                    Debug.Log("Saber hit note");
                    quaternion noteRotation = EntityManager.GetComponentData<Rotation>(hit.Entity).Value;
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, noteRotation, Vector3.one);

                    float angle = Vector3.Angle((float3)transform.position - previousPosition, matrix.MultiplyPoint(Vector3.up));

                    if (angle > 130 || note.CutDirection == 8)
                    {
                        // Reward player with points
                        DestroyNote(hit.Entity);

                        if (affectsNoteType == 1)
                        {
                            Pulse(.5f, 150, 75, SteamVR_Input_Sources.RightHand);
                        }
                        else
                        {
                            Pulse(.5f, 150, 75, SteamVR_Input_Sources.LeftHand);
                        }
                    }
                }
                else if (note.Type == 3)
                {
                    // Hit Bomb
                    Debug.LogWarning("Saber hit a bomb");
                }
            }
            previousPosition = transform.position;
        }

        private void DestroyNote(Entity entity)
        {
            //Pulse();

            EntityManager.DestroyEntity(entity);
        }
    }
}