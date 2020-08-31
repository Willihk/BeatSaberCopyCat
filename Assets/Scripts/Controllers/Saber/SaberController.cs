using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using BeatGame.Utility.Physics;
using Valve.VR;
using BeatGame.Logic.Managers;
using UnityEngine.VFX;
using Unity.Collections;
using RaycastHit = Unity.Physics.RaycastHit;

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
        Transform tipPoint;
        [SerializeField]
        Transform basePoint;
        [SerializeField]
        VisualEffect hitVFX;
        [SerializeField]
        Transform[] raycastPoints;

        float3 previousTipPosition;
        float3 previousBasePosition;
        EntityManager EntityManager;

        NativeList<RaycastHit> raycastHits;

        private void Start()
        {
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            raycastHits = new NativeList<RaycastHit>(4, Allocator.Persistent);
        }

        void OnDestroy()
        {
            raycastHits.Dispose();
        }

        private void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source)
        {
            hapticAction.Execute(0, duration, frequency, amplitude, source);
        }

        void OnDrawGizmosSelected()
        {
            for (int i = 0; i < raycastPoints.Length; i++)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(raycastPoints[i].position, basePoint.forward * saberLength);
            }
        }

        void Update()
        {
            for (int i = 0; i < raycastPoints.Length; i++)
            {
                ECSRaycast.RaycastAll(raycastPoints[i].position, raycastPoints[i].forward * saberLength, ref raycastHits);

                for (int j = 0; j < raycastHits.Length; j++)
                {
                    var hit = raycastHits[j];
                    if (hit.Entity != Entity.Null && EntityManager.HasComponent<Note>(hit.Entity))
                    {
                        var note = EntityManager.GetComponentData<Note>(hit.Entity);
                        if (note.Type == affectsNoteType)
                        {
                            quaternion noteRotation = EntityManager.GetComponentData<Rotation>(hit.Entity).Value;
                            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, noteRotation, Vector3.one);

                            float tipAngle = Vector3.Angle((float3)tipPoint.position - previousTipPosition, matrix.MultiplyPoint(Vector3.up));
                            float baseAngle = Vector3.Angle((float3)basePoint.position - previousBasePosition, matrix.MultiplyPoint(Vector3.up));

                            if (baseAngle > 170 || tipAngle > 170 || note.CutDirection == 8)
                            {
                                ScoreManager.Instance.AddScore(100);

                                if (affectsNoteType == 1)
                                    Pulse(.03f, 160, 1, SteamVR_Input_Sources.RightHand);
                                else
                                    Pulse(.03f, 160, 1, SteamVR_Input_Sources.LeftHand);

                                if (SettingsManager.Instance.Settings["General"]["HitEffects"].IntValue == 1)
                                {
                                    hitVFX.gameObject.transform.position = hit.Position;
                                    hitVFX.SendEvent("Burst");
                                }

                                SaberHitAudioManager.Instance.PlaySound();

                                DestroyNote(hit.Entity);
                            }
                        }
                        else if (note.Type == 3)
                        {
                            // Hit Bomb
                            Debug.LogWarning("Saber hit a bomb");
                            ScoreManager.Instance.MissedNote();
                            ScoreManager.Instance.MissedNote();
                            ScoreManager.Instance.MissedNote();
                        }
                    }
                }
                raycastHits.Clear();
            }

            previousTipPosition = tipPoint.position;
            previousBasePosition = basePoint.position;
        }

        private void DestroyNote(Entity entity)
        {
            EntityManager.DestroyEntity(entity);
        }
    }
}