using UnityEngine;
using System.Collections;
using BeatGame.Logic.Managers;
using UnityEngine.UI;
using Unity.Mathematics;

namespace BeatGame.UI.Controllers
{
    public class HealthBarUIController : MonoBehaviour
    {
        [SerializeField]
        Image barImage;

        void OnEnable()
        {
            if (HealthManager.Instance != null)
                HealthManager.Instance.OnHealthChanged += UpdateBar;

            UpdateBar();
        }

        private void OnDisable()
        {
            if (HealthManager.Instance != null)
                HealthManager.Instance.OnHealthChanged -= UpdateBar;
        }

        private void UpdateBar()
        {
            barImage.fillAmount = math.lerp(barImage.fillAmount, HealthManager.Instance.Health / HealthManager.Instance.MaxHealth, .1f);
            StopAllCoroutines();
            StartCoroutine(SmoothUpdateBar());
        }

        IEnumerator SmoothUpdateBar()
        {
            while (barImage.fillAmount != HealthManager.Instance.Health / HealthManager.Instance.MaxHealth)
            {
                barImage.fillAmount = math.lerp(barImage.fillAmount, HealthManager.Instance.Health / HealthManager.Instance.MaxHealth, .1f);
                yield return null;
            }
        }
    }
}