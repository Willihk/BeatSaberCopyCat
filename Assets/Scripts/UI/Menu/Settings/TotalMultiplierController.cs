using BeatGame.Logic.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
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
            SettingsManager.Instance.OnConfigChanged += CalcMultiplier;
        }
    }

    private void OnDestroy()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnConfigChanged -= CalcMultiplier;
        }
    }

    private void CalcMultiplier()
    {
        float multiplier = 1;
        var modifiers = SettingsManager.Instance.Settings["Modifiers"];

        if (modifiers["NoFail"].IntValue == 1)
        {
            multiplier += -0.50f;
        }
        if (modifiers["NoArrows"].IntValue == 1)
        {
            multiplier += -0.10f;
        }

        ScoreManager.Instance.TotalMultiplier = multiplier;
        multiplierText.text = multiplier.ToString("00") + "x";
    }
}
