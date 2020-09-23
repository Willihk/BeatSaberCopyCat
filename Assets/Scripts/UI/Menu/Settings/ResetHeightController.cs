using UnityEngine;
using System.Collections;
using BeatGame.Logic.Managers;

namespace BeatGame.UI.Controllers
{
    public class ResetHeightController : MonoBehaviour
    {
        public void ResetHeight()
        {
            float heightOffset = Camera.main.transform.localPosition.y - 1.7f;
            SettingsManager.GlobalOffset.y = heightOffset;
            SettingsManager.Instance.UpdateConfigSetting("General", "HeightOffset", heightOffset);
            Debug.Log(SettingsManager.GlobalOffset.y);
        }
    }
}