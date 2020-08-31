using UnityEngine;
using System.Collections;
using BeatGame.Logic.Managers;

namespace BeatGame.Utility
{
    public class UseGlobalOffset : MonoBehaviour
    {
        Vector3 defaultHeight;

        private void OnEnable()
        {
            defaultHeight = transform.position;
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.OnConfigChanged += OnConfigChanged;
                OnConfigChanged();
            }
        }

        private void OnConfigChanged()
        {
            if (transform != null)
                transform.position = defaultHeight + (Vector3)SettingsManager.GlobalOffset;
        }

        private void OnDisable()
        {
            if (SettingsManager.Instance != null)
                SettingsManager.Instance.OnConfigChanged -= OnConfigChanged;
        }
    }
}