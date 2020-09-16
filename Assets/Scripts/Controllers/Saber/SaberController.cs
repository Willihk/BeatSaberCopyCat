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
using Unity.Physics;

namespace BeatGame.Logic.Saber
{
    public class SaberController : MonoBehaviour
    {
        [SerializeField]
        int affectsNoteType;
        [SerializeField]
        float hitAngle = 130;
        [SerializeField]
        float minCutVelocity = 0.012f;
        [SerializeField]
        float saberLength = 1;
        [SerializeField]
        SteamVR_Action_Vibration hapticAction;

        [SerializeField]
        Transform fakeNoteTransform;
        [SerializeField]
        Transform tipPoint;
        [SerializeField]
        Transform basePoint;
        [SerializeField]
        VisualEffect hitVFX;
        [SerializeField]
        Transform[] raycastPoints;

        [SerializeField]
        float velocity;

        bool isInContact;

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

        private void OnDisable()
        {
            isInContact = false;
            hitVFX.SendEvent("Stop");
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
            velocity = (tipPoint.position - (Vector3)previousTipPosition).magnitude;

            var obstacleCast = ECSRaycast.Raycast(raycastPoints[0].position, raycastPoints[0].forward * 1.25f);
            if (Physics.Raycast(raycastPoints[0].position, raycastPoints[0].forward, out UnityEngine.RaycastHit raycastHit, 1.25f) || EntityManager.HasComponent<Obstacle>(obstacleCast.Entity))
            {
                if (!isInContact)
                {
                    isInContact = true;
                    hitVFX.SendEvent("Contact");
                }

                hitVFX.transform.position = raycastHit.point;
            }
            else
            {
                if (isInContact)
                {
                    isInContact = false;
                    hitVFX.SendEvent("Stop");
                }
            }

            if (velocity >= minCutVelocity)
            {
                for (int i = 0; i < raycastPoints.Length; i++)
                {
                    ECSRaycast.RaycastAll(raycastPoints[i].position, raycastPoints[i].forward * saberLength, ref raycastHits);

                    for (int j = 0; j < raycastHits.Length; j++)
                    {
                        var hit = raycastHits[j];
                        if (hit.Entity != Entity.Null && EntityManager.HasComponent<Note>(hit.Entity))
                        {
                            // Hit Note
                            var note = EntityManager.GetComponentData<Note>(hit.Entity);
                            if (note.Type == affectsNoteType)
                            {
                                quaternion noteRotation = EntityManager.GetComponentData<Rotation>(hit.Entity).Value;
                                fakeNoteTransform.rotation = noteRotation;
                                fakeNoteTransform.position = EntityManager.GetComponentData<Translation>(hit.Entity).Value;

                                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, noteRotation, Vector3.one);
                                float tipAngle = Vector3.Angle((float3)tipPoint.position - previousTipPosition, matrix.MultiplyPoint(Vector3.up));
                                float baseAngle = Vector3.Angle((float3)basePoint.position - previousBasePosition, matrix.MultiplyPoint(Vector3.up));

                                Vector3 tipCutDir = fakeNoteTransform.InverseTransformVector(tipPoint.position - (Vector3)previousTipPosition);
                                Vector3 baseCutDir = fakeNoteTransform.InverseTransformVector(basePoint.position - (Vector3)previousBasePosition);
                                if (OkCut(tipCutDir, out _) || OkCut(baseCutDir, out _) || baseAngle > hitAngle || tipAngle > hitAngle || note.CutDirection == 8)
                                {
                                    ScoreManager.Instance.AddScore(100);

                                    if (affectsNoteType == 1 || SettingsManager.Instance.Settings["Modifiers"]["DoubleSaber"].IntValue == 1)
                                        Pulse(.03f, 160, 1, SteamVR_Input_Sources.RightHand);
                                    else
                                        Pulse(.03f, 160, 1, SteamVR_Input_Sources.LeftHand);


                                    if (SettingsManager.Instance.Settings["General"]["HitEffects"].IntValue == 1)
                                    {
                                        hitVFX.gameObject.transform.position = hit.Position;
                                        hitVFX.SendEvent("Burst");
                                    }

                                    SaberHitAudioManager.Instance.PlaySound();

                                    HealthManager.Instance.HitNote();

                                    DestroyNote(hit.Entity);
                                }
                            }
                            else if (note.Type == 3)
                            {
                                // Hit Bomb
                                HealthManager.Instance.HitBomb();
                                ScoreManager.Instance.MissedNote();
                                ScoreManager.Instance.MissedNote();
                                ScoreManager.Instance.MissedNote();
                            }
                        }
                    }
                    raycastHits.Clear();
                }
            }

            previousTipPosition = tipPoint.position;
            previousBasePosition = basePoint.position;
        }

        public bool OkCut(Vector3 to, out float angle)
        {
            angle = Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg;

            bool goodEnoughCut = angle > -150 && angle < -30;

            if (goodEnoughCut)
                return true;

            return false;
        }

        private void DestroyNote(Entity entity)
        {
            // Slice Mesh

            ThreePointsToBox(tipPoint.position, basePoint.position, previousTipPosition + previousBasePosition * 0.5f, out Vector3 center, out Vector3 halfSize, out Quaternion orientation);

            Vector3 direction = orientation * Vector3.up;

            SaberSliceManager.Instance.Slice(fakeNoteTransform, direction, transform.parent.right, affectsNoteType, velocity * 12);

            EntityManager.DestroyEntity(entity);
        }


        public void ThreePointsToBox(Vector3 p0, Vector3 p1, Vector3 p2, out Vector3 center, out Vector3 halfSize, out Quaternion orientation) //https://github.com/hrincarp/GGJ2017-cart/
        {
            Vector3 up = Vector3.Cross(p1 - p2, p0 - p2).normalized;

            // Continue only if normal exists
            if (up.sqrMagnitude > 0.00001f)
            {

                Vector3 forward = (p0 - p1).normalized;
                Vector3 left = Vector3.Cross(forward, up);

                orientation = new Quaternion();
                orientation.SetLookRotation(forward, up);

                float a = Mathf.Abs((new UnityEngine.Plane(left, p0)).GetDistanceToPoint(p2));
                float b = Vector3.Magnitude(p0 - p1);

                Vector3 pc = (p0 + p1) * 0.5f;

                center = pc - left * a * 0.5f;
                halfSize = new Vector3(a * 0.5f, 0.0f, b * 0.5f);
            }
            else
            {
                center = Vector3.zero;
                halfSize = Vector3.zero;
                orientation = Quaternion.identity;
            }
        }
    }
}