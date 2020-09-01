using UnityEngine;
using BeatGame.Logic.Managers;

namespace BeatGame.UI.Controllers
{
    public class CheckBoxSettingController : MonoBehaviour
    {
        [SerializeField]
        string section = "General";
        [SerializeField]
        string setting = "HitEffects";

        [SerializeField]
        GameObject checkMark;

        private void OnEnable()
        {
            if (SettingsManager.Instance != null && SettingsManager.Instance.Settings[section][setting].IntValue == 0)
            {
                checkMark.SetActive(false);
            }
        }

        public void Toggle()
        {
            if (SettingsManager.Instance.Settings[section][setting].IntValue == 0)
            {
                SettingsManager.Instance.UpdateConfigSetting(section, setting, 1);
                checkMark.SetActive(true);
            }
            else
            {
                SettingsManager.Instance.UpdateConfigSetting(section, setting, 0);
                checkMark.SetActive(false);
            }
        }
    }
}