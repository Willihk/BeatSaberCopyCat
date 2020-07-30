using UnityEngine;
using System.Collections;
using VRTK;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using BeatGame.Utility.Physics;

namespace BeatGame.Logic.Saber
{
    public class SaberController : MonoBehaviour
    {
        [SerializeField]
        int affectsNoteType;
        [SerializeField]
        float saberLength = 1;

        float impactMagnifier = 120f;
        float collisionForce = 0f;
        float maxCollisionForce = 4000f;
        VRTK_ControllerReference controllerReference;

        float3 previousPosition;
        EntityManager EntityManager;

        private void Start()
        {
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var controllerEvent = GetComponentInChildren<VRTK_ControllerEvents>(true);
            if (controllerEvent != null && controllerEvent.gameObject != null)
            {
                controllerReference = VRTK_ControllerReference.GetControllerReference(controllerEvent.gameObject);
            }
        }

        private void Pulse()
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                collisionForce = VRTK_DeviceFinder.GetControllerVelocity(controllerReference).magnitude * impactMagnifier;
                var hapticStrength = collisionForce / maxCollisionForce;
                VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, hapticStrength, 0.5f, 0.01f);
            }
            else
            {
                var controllerEvent = GetComponentInChildren<VRTK_ControllerEvents>();
                if (controllerEvent != null && controllerEvent.gameObject != null)
                {
                    controllerReference = VRTK_ControllerReference.GetControllerReference(controllerEvent.gameObject);
                }
            }
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

                    //float angle = Vector3.Angle(translation.Value - saberData.PreviousPosition, matrix.MultiplyPoint(Vector3.up));

                    float angle = Vector3.Angle((float3)transform.position - previousPosition, matrix.MultiplyPoint(Vector3.up));

                    if (angle > 130 || note.CutDirection == 8)
                    {
                        // Reward player with points
                        DestroyNote(hit.Entity);
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
            Pulse();

            EntityManager.DestroyEntity(entity);
        }
    }
}