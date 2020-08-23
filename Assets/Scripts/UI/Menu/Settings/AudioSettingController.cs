using UnityEngine;
using System.Collections;
using BeatGame.Logic.Managers;
using UnityEngine.UI;
using TMPro;
using BeatGame.Utility.Math;

namespace BeatGame.UI.Controllers
{
    public class AudioSettingController : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI AudioValueText;

        [SerializeField]
        string audioSettingName = "MasterVolume";
        [SerializeField]
        Slider slider;

        private void OnEnable()
        {
            if (SettingsManager.Instance.HasLoadedSettings == true)
            {
                slider.SetValueWithoutNotify(SettingsManager.Instance.Settings["Audio"][audioSettingName].FloatValue);
                UpdateText();
            }
        }

        void UpdateText()
        {
            int percent = (int)MathHelper.Map(slider.value, slider.minValue, slider.maxValue, 0, 100);
            AudioValueText.text = percent.ToString();
        }

        public void ValueChanged(float value)
        {
            UpdateText();
            SettingsManager.Instance.UpdateConfigSetting("Audio", audioSettingName, value);
        }
    }
}