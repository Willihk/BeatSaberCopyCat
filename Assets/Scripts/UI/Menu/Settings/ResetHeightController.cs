using UnityEngine;
using System.Collections;
using BeatGame.Logic.Managers;

namespace BeatGame.UI.Controllers
{
    public class ResetHeightController : MonoBehaviour
    {
        public void ResetHeight()
        {
            float heightOffset = 1.76f - Camera.main.transform.localPosition.y;
            SettingsManager.GlobalOffset.y = heightOffset;
            SettingsManager.Instance.UpdateConfigSetting("General", "HeightOffset", heightOffset);
            Debug.Log($"Manager Offset: {SettingsManager.GlobalOffset.y}, actual offset {heightOffset}");
        }
    }
}