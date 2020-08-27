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
        VisualEffect hitVFX;


        float3 previousPosition;
        EntityManager EntityManager;

        NativeList<RaycastHit> raycastHits;

        private void Start()
        {
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            raycastHits = new NativeList<RaycastHit>(4,Allocator.Persistent);
        }

        void OnDestroy()
        {
            raycastHits.Dispose();
        }

        private void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source)
        {
            hapticAction.Execute(0, duration, frequency, amplitude, source);
        }

        void Update()
        {
            ECSRaycast.RaycastAll(transform.position, transform.position + transform.forward * saberLength, ref raycastHits);

            for (int i = 0; i < raycastHits.Length; i++)
            {
                var hit = raycastHits[i];
                if (hit.Entity != Entity.Null && EntityManager.HasComponent<Note>(hit.Entity))
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
            previousPosition = transform.position;
        }

        private void DestroyNote(Entity entity)
        {
            EntityManager.DestroyEntity(entity);
        }
    }
}