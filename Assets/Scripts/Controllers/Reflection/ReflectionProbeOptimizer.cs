using UnityEngine;
using BeatGame.Logic.Managers;

namespace BeatGame.Logic
{
    public class ReflectionProbeOptimizer : MonoBehaviour
    {
        [SerializeField]
        ReflectionProbe reflectionProbe;

        private void OnEnable()
        {
            reflectionProbe.enabled = CurrentSongDataManager.Instance.MapData.Events.Length != 0;
        }
    }
}