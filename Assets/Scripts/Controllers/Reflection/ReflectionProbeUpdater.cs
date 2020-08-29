using UnityEngine;
using System.Collections;
using BeatGame.Data;
using BeatGame.Logic.Managers;

namespace BeatGame.Logic
{
    public class ReflectionProbeUpdater : MonoBehaviour
    {
        [SerializeField]
        ReflectionProbe reflectionProbe;

        private void OnEnable()
        {
            reflectionProbe.enabled = CurrentSongDataManager.Instance.MapData.Events.Length != 0;
            Debug.Log(CurrentSongDataManager.Instance.MapData.Events.Length != 0);
        }
    }
}