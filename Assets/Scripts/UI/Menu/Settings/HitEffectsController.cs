using UnityEngine;
using System.Collections;
using BeatGame.Logic.Managers;

namespace BeatGame.UI.Controllers
{
    public class HitEffectsController : MonoBehaviour
    {
        [SerializeField]
        GameObject checkMark;

        private void OnEnable()
        {
            if (SettingsManager.Instance != null && SettingsManager.Instance.Settings["General"]["HitEffects"].IntValue == 0)
            {
                checkMark.SetActive(false);
            }
        }

        public void Toggle()
        {
            if (SettingsManager.Instance.Settings["General"]["HitEffects"].IntValue == 0)
            {
                SettingsManager.Instance.UpdateConfigSetting("General", "HitEffects", 1);
                checkMark.SetActive(true);
            }
            else
            {
                SettingsManager.Instance.UpdateConfigSetting("General", "HitEffects", 0);
                checkMark.SetActive(false);
            }
        }
    }
}