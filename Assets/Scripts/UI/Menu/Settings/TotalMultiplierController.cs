using BeatGame.Logic.Managers;
using UnityEngine;
using TMPro;

public class TotalMultiplierController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI multiplierText;

    private void OnEnable()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnConfigChanged += Refresh;
            Refresh();
        }
    }

    private void OnDestroy()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnConfigChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        multiplierText.text = ScoreManager.Instance.TotalMultiplier + "x";
    }
}
