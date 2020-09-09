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
            if (SettingsManager.Instance.Settings["General"]["Reflections"].IntValue == 1)
            {
                reflectionProbe.enabled = true;
                reflectionProbe.enabled = CurrentSongDataManager.Instance.MapData.Events.Length != 0;
            }
            else
            {
                reflectionProbe.enabled = false;
            }

        }
    }
}