using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using BeatGame.Utility.Physics;
using Valve.VR;
using BeatGame.Logic.Managers;
using UnityEngine.VFX;

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

        [SerializeField]
        VisualEffect hitVFX;

        bool IsVFXTurnedOn;

        float3 previousPosition;
        EntityManager EntityManager;

        private void Start()
        {
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source)
        {
            hapticAction.Execute(0, duration, frequency, amplitude, source);
        }

        void Update()
        {
            var hit = ECSRaycast.Raycast(transform.position, transform.forward * saberLength);
            if (hit.Entity == Entity.Null)
            {
                if (IsVFXTurnedOn)
                    hitVFX.SendEvent("Stop");
            }

            if (EntityManager.HasComponent<Note>(hit.Entity))
            {
                var note = EntityManager.GetComponentData<Note>(hit.Entity);
                if (note.Type == affectsNoteType)
                {
                    quaternion noteRotation = EntityManager.GetComponentData<Rotation>(hit.Entity).Value;
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, noteRotation, Vector3.one);

                    float angle = Vector3.Angle((float3)transform.position - previousPosition, matrix.MultiplyPoint(Vector3.up));

                    if (angle > 130 || note.CutDirection == 8)
                    {
                        ScoreManager.Instance.AddScore(100);

                        if (affectsNoteType == 1)
                        {
                            Pulse(.03f, 160, 1, SteamVR_Input_Sources.RightHand);
                        }
                        else
                        {
                            Pulse(.03f, 160, 1, SteamVR_Input_Sources.LeftHand);
                        }

                        hitVFX.gameObject.transform.position = hit.Position;
                        hitVFX.SendEvent("Burst");

                        SaberHitAudioManager.Instance.PlaySound();

                        DestroyNote(hit.Entity);
                    }
                }
                else if (note.Type == 3)
                {
                    // Hit Bomb
                    Debug.LogWarning("Saber hit a bomb");
                }
                else
                {
                    hitVFX.gameObject.transform.position = hit.Position;

                    if (!IsVFXTurnedOn)
                        hitVFX.SendEvent("Contact");
                }
            }
            previousPosition = transform.position;
        }

        private void DestroyNote(Entity entity)
        {
            EntityManager.DestroyEntity(entity);
        }
    }
}